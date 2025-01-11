using Godot;
using System;
using System.Linq;
using BioLibrary;
using BioStructures;

public partial class SizeGenerator : Node
{
	public static SizeGenerator Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}

	public override void _ExitTree()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void GenerateSize(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		try
		{
			// Roll 1d6 for basic size category
			int sizeRoll = Roll.Dice(1);
			
			// Apply gravity modifiers
			if (settings.Gravity <= 0.4f) sizeRoll += 2;
			else if (settings.Gravity <= 0.75f) sizeRoll += 1;
			else if (settings.Gravity >= 1.5f && settings.Gravity <= 2.0f) sizeRoll -= 1;
			else if (settings.Gravity > 2.0f) sizeRoll -= 2;
			
			// Apply habitat modifiers
			if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) sizeRoll += 1;
			if (habitatInfo.habitat == "Ocean" || habitatInfo.habitat == "Shallows") sizeRoll += 1;
			if (habitatInfo.habitat == "Lagoon" || habitatInfo.habitat == "River") sizeRoll -= 1;
			if (habitatInfo.habitat == "Plain") sizeRoll += 1;
			if (habitatInfo.habitat == "Coastal" || habitatInfo.habitat == "Desert" || habitatInfo.habitat == "Mountain") sizeRoll -= 1;
			
			// Apply trophic level modifiers
			if (creature.TrophicLevel == "Grazing Herbivore") sizeRoll += 1;
			if (creature.TrophicLevel == "Parasite") sizeRoll -= 4;
			
			// Apply locomotion modifiers
			if (creature.Locomotion == "slithering") sizeRoll -= 1;
			if (creature.Locomotion == "winged flight") sizeRoll -= 3;
			
			// Determine size category based on modified roll
			creature.SizeCategory = sizeRoll <= 2 ? "Small" : sizeRoll <= 4 ? "Medium" : "Large";
			
			// Determine specific size from table
			var sizesInCategory = BioData.SizeCategory()[creature.SizeCategory];
			creature.SpecificSize = Roll.Vary(Roll.Seek(sizesInCategory));
			
			// Apply gravity size multiplier
			var GravitySizeMulti = BioData.GravitySizeMultiplier();
			creature.GravitySizeMultiplier = Roll.Search(GravitySizeMulti, settings.Gravity);
			
			// Calculate final size and weight
			float finalSize = creature.SpecificSize * creature.GravitySizeMultiplier * 3;
			float baseWeight = (float)(Math.Pow(finalSize / 2, 3) * 200);
			
			// Adjust weight based on chemical basis
			if (creature.ChemicalBasis == "Silicon-Based") baseWeight *= 2;
			if (creature.ChemicalBasis == "Hydrogen-Based") baseWeight /= 10;
			
			// Final weight adjusted for local gravity
			creature.SpecificSize = finalSize;
			creature.WeightInPounds = baseWeight * settings.Gravity;
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GenerateSize: {e.Message}\n{e.StackTrace}");
		}
	}
}
