using Godot;
using System;
using System.Collections.Generic;
using BioLibrary;
using BioStructures;

public partial class PhysiologyGenerator : Node
{
	public static PhysiologyGenerator Instance { get; private set; }

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

	public void GeneratePhysiology(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		creature.ChemicalBasis = DetermineChemicalBasis(settings);
		creature.SizeCategory = DetermineSizeCategory(settings, habitatInfo);
		creature.GravitySizeMultiplier = CalculateGravitySizeMultiplier(settings.Gravity, habitatInfo.zone);
		
		DetermineBodyStructure(creature, habitatInfo);
		DetermineExternalFeatures(creature, settings, habitatInfo);
		DetermineBreathingAndTemperature(creature, settings, habitatInfo);
	}

	private string DetermineChemicalBasis(SettingsManager.PlanetSettings settings)
	{
		return Roll.Seek(BioData.Chemical_Basis());
	}

	private string DetermineSizeCategory(SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int sizeRoll = Roll.Dice(1);
		sizeRoll += CalculateSizeModifiers(settings, habitatInfo);

		if (sizeRoll <= 2) return "Small";
		if (sizeRoll <= 4) return "Medium";
		return "Large";
	}

	private int CalculateSizeModifiers(SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;

		// Gravity modifiers
		if (settings.Gravity <= 0.4f) modifier += 2;
		else if (settings.Gravity <= 0.75f) modifier += 1;
		else if (settings.Gravity >= 1.5f && settings.Gravity < 2f) modifier -= 1;
		else if (settings.Gravity >= 2f) modifier -= 2;

		// Habitat modifiers
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) modifier += 1;
		switch (habitatInfo.habitat)
		{
			case "Ocean":
			case "Banks":
			case "Plains":
				modifier += 1;
				break;
			case "Lagoon":
			case "River":
			case "Beach":
			case "Desert":
			case "Mountain":
				modifier -= 1;
				break;
		}

		return modifier;
	}

	private float CalculateGravitySizeMultiplier(float gravity, HabitatGenerator.HabitatZone zoneType)
	{
		if (zoneType == HabitatGenerator.HabitatZone.Water) return 1.0f;
		return Roll.Search(BioData.GravitySizeMultiplier(), gravity);
	}

	private void DetermineBodyStructure(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Symmetry
		int symmetryRoll = Roll.Dice(2);
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.GasGiant) symmetryRoll += 1;
		creature.Symmetry = Roll.Seek(BioData.Symmetry(), symmetryRoll);

		// Number of Limbs
		creature.NumberOfLimbs = DetermineNumberOfLimbs(creature.Symmetry);

		// Skeleton
		int skeletonRoll = Roll.Dice(2);
		skeletonRoll += CalculateSkeletonModifiers(creature, habitatInfo);
		creature.Skeleton = Roll.Seek(BioData.Skeleton(), skeletonRoll);

		// Manipulators
		creature.NumberOfManipulators = DetermineNumberOfManipulators(creature, habitatInfo);
	}

	private int DetermineNumberOfLimbs(string symmetry)
	{
		switch (symmetry)
		{
			case "Asymmetric":
				return Math.Max(0, Roll.Dice(2) - 2);
			case "Spherical":
				return 1; // One limb per side, actual number determined by renderer
			default:
				int segmentRoll = Roll.Dice(2);
				if (symmetry == "Trilateral") segmentRoll -= 1;
				if (symmetry == "Radial") segmentRoll -= 2;
				
				if (segmentRoll <= 2) return 1;
				if (segmentRoll <= 4) return 2;
				return Roll.Dice(segmentRoll - 4);
		}
	}

	private int CalculateSkeletonModifiers(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;
		
		if (creature.SizeCategory == "Medium") modifier += 1;
		if (creature.SizeCategory == "Large") modifier += 2;
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Land) modifier += 1;
		if (creature.Symmetry == "Asymmetric") modifier -= 1;

		return modifier;
	}

	private int DetermineNumberOfManipulators(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int manipulatorRoll = Roll.Dice(2);
		manipulatorRoll += CalculateManipulatorModifiers(creature, habitatInfo);
		
		var result = Roll.Seek(BioData.NumberOfManipulators(), manipulatorRoll);
		return int.TryParse(result, out int number) ? number : 0;
	}

	private int CalculateManipulatorModifiers(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;
		
		if (creature.NumberOfLimbs == 2) modifier -= 1;
		if (creature.NumberOfLimbs > 4) modifier += 1;
		if (creature.NumberOfLimbs > 6) modifier += 1;
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water || 
			habitatInfo.zone == HabitatGenerator.HabitatZone.GasGiant) modifier -= 2;

		return modifier;
	}

	private void DetermineExternalFeatures(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Tail
		if (creature.Symmetry != "Spherical" && (Roll.Dice(1) + (habitatInfo.zone == HabitatGenerator.HabitatZone.Water ? 1 : 0)) >= 5)
		{
			int tailRoll = Roll.Dice(2);
			creature.TailFeatures = Roll.Seek(BioData.TailFeatures(), tailRoll);
		}
		else
		{
			creature.TailFeatures = "None";
		}

		// Skin
		DetermineSkin(creature, settings, habitatInfo);
	}

	private void DetermineSkin(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		string coveringType = Roll.Seek(BioData.Skin()["Covering"]);
		
		int skinRoll = Roll.Dice(2);
		skinRoll += CalculateSkinModifiers(coveringType, settings, habitatInfo);
		
		creature.Skin = Roll.Seek(BioData.Skin()[coveringType], skinRoll);
	}

	private int CalculateSkinModifiers(string coveringType, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;
		
		if (habitatInfo.habitat == "Arctic") modifier += 1;
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) modifier += 1;
		if (habitatInfo.habitat == "Desert") modifier -= 1;

		return modifier;
	}

	private void DetermineBreathingAndTemperature(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// This would be implemented if we need to track breathing method and temperature regulation
		// For now, we'll skip it as it's not in the Creature class
	}
}
