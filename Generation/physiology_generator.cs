using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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
		try
		{
			// Body Structure
			DetermineBodyStructure(creature, settings, habitatInfo);
			DetermineExternalFeatures(creature, settings, habitatInfo);
			DetermineBreathing(creature, settings, habitatInfo);
			DetermineTemperatureRegulation(creature, settings, habitatInfo);
			DetermineGrowthPattern(creature, settings, habitatInfo);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GeneratePhysiology: {e.Message}\n{e.StackTrace}");
		}
	}

	private void DetermineBodyStructure(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Symmetry
		int symmetryRoll = Roll.Dice(2, 6);
		var symmetryTable = BioData.Symmetry();
		creature.Symmetry = Roll.Seek(symmetryTable, symmetryRoll);

		// Additional rolls for Radial and Spherical symmetry
		if (creature.Symmetry == "Radial")
		{
			creature.SymmetryNumber = Roll.Dice(1) + 3;  // 1d+3 sides
		}
		else if (creature.Symmetry == "Spherical")
		{
			int roll = Roll.Dice(1);
			creature.SymmetryNumber = roll switch
			{
				1 => 4,
				2 or 3 => 6,
				4 => 8,
				5 => 12,
				_ => 20
			};
		}

		// Limb Structure and count
		DetermineLimbsBasedOnSymmetry(creature);

		// Skeleton
		int skeletonRoll = Roll.Dice(2, 6);
		var skeletonTable = BioData.Skeleton();
		creature.Skeleton = Roll.Seek(skeletonTable, skeletonRoll);

		// Manipulators
		DetermineManipulators(creature);
	}

	private void DetermineLimbsBasedOnSymmetry(Creature creature)
	{
		var limbTable = BioData.NumberOfLimbs();
		
		if (creature.Symmetry == "Spherical")
		{
			// Spherical: one limb per side
			creature.LimbStructure = "One segment";
			creature.ActualLimbCount = creature.SymmetryNumber ?? 0;
		}
		else if (creature.Symmetry == "Asymmetric")
		{
			// Asymmetric: 2d-2 limbs
			creature.LimbStructure = "Asymmetric";
			creature.ActualLimbCount = Math.Max(0, Roll.Dice(2) - 2);
		}
		else
		{
			// Bilateral, Trilateral, or Radial
			int limbRoll = Roll.Dice(2, 6);
			limbRoll += creature.Symmetry switch
			{
				"Trilateral" => -1,
				"Radial" => -2,
				_ => 0
			};
			
			creature.LimbStructure = Roll.Seek(limbTable, limbRoll);
			
			// Calculate actual limbs based on structure and symmetry
			int segmentCount = creature.LimbStructure switch
			{
				"Limbless" => 0,
				"One segment" => 1,
				"Two segments" => 2,
				"1d segments" => Roll.Dice(1),
				"2d segments" => Roll.Dice(2),
				"3d segments" => Roll.Dice(3),
				_ => 0
			};

			int multiplier = creature.Symmetry switch
			{
				"Bilateral" => 2,
				"Trilateral" => 3,
				"Radial" => creature.SymmetryNumber ?? 0,
				_ => 1
			};

			creature.ActualLimbCount = segmentCount * multiplier;
		}
	}

	private void DetermineManipulators(Creature creature)
	{
		if (creature.ActualLimbCount == 0)
		{
			creature.ManipulatorType = "No manipulators";
			creature.ActualManipulatorCount = 0;
			return;
		}

		int manipulatorRoll = Roll.Dice(2, 6);
		
		// Apply modifiers
		if (creature.ActualLimbCount == 2) manipulatorRoll -= 1;
		else if (creature.ActualLimbCount > 4) manipulatorRoll += 1;
		if (creature.ActualLimbCount > 6) manipulatorRoll += 1;
		
		if (creature.Locomotion == "winged flight" || creature.Locomotion == "gliding") manipulatorRoll -= 1;
		if (creature.Locomotion == "swimming" && 
			(creature.Habitat == "Ocean" || creature.Habitat == "Deep Ocean" || creature.Habitat == "Jovian"))
			manipulatorRoll -= 2;
		if (creature.Locomotion == "brachiating") manipulatorRoll += 2;
		if (creature.TrophicLevel == "Gathering Herbivore") manipulatorRoll += 1;

		var manipulatorTable = BioData.NumberOfManipulators();
		creature.ManipulatorType = Roll.Seek(manipulatorTable, manipulatorRoll);

		// Calculate actual manipulator count
		int manipulatorMultiplier = creature.Symmetry switch
		{
			"Bilateral" => 2,
			"Trilateral" => 3,
			"Radial" => creature.SymmetryNumber ?? 0,
			"Spherical" => creature.SymmetryNumber ?? 0,
			"Asymmetric" => 1,
			_ => 1
		};

		// Determine number of sets based on ManipulatorType
		int sets = creature.ManipulatorType switch
		{
			"No manipulators" => 0,
			"1 set of manipulators, Bad Grip" => 1,
			"Prehensile tail or trunk" => 1,
			"1 set of manipulators with normal Grip" => 1,
			"2 sets of manipulators" => 2,
			"1d sets of manipulators" => Roll.Dice(1),
			_ => 0
		};

		creature.ActualManipulatorCount = sets * manipulatorMultiplier;

		// Special case for prehensile tail additional check
		if (creature.ManipulatorType == "Prehensile tail or trunk" && Roll.Dice(1) == 6)
		{
			// Roll again for additional manipulators
			manipulatorRoll = Roll.Dice(2, 6);
			string additionalType = Roll.Seek(manipulatorTable, manipulatorRoll);
			DetermineManipulators(creature); // Recursive call to add more manipulators
		}

		// Ensure manipulator count doesn't exceed limb count
		creature.ActualManipulatorCount = Math.Min(creature.ActualManipulatorCount, creature.ActualLimbCount);
	}

	private void DetermineExternalFeatures(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Tail Features
		DetermineTailFeatures(creature, habitatInfo);

		// Skin
		DetermineSkin(creature, settings, habitatInfo);
	}

	private void DetermineTailFeatures(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		if (creature.Symmetry == "Spherical")
		{
			creature.TailFeatures = "None";
			return;
		}

		int tailRoll = Roll.Dice(1);
		if (creature.Locomotion == "swimming") tailRoll += 1;
		
		if (tailRoll >= 5)
		{
			int featureRoll = Roll.Dice(2, 6);
			var tailTable = BioData.TailFeatures();
			creature.TailFeatures = Roll.Seek(tailTable, featureRoll);
			
			// Handle combination
			if (creature.TailFeatures == "Combination")
			{
				List<string> combinedFeatures = new List<string>();
				for (int i = 0; i < 2; i++)
				{
					int combinationRoll = Roll.Dice(1) + 5;
					string feature = Roll.Seek(tailTable, combinationRoll);
					if (feature != "Combination" && !combinedFeatures.Contains(feature))
						combinedFeatures.Add(feature);
				}
				creature.TailFeatures = string.Join(", ", combinedFeatures);
			}
		}
		else
		{
			creature.TailFeatures = "No features";
		}
	}

	private void DetermineSkin(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Roll 1d6 for covering type
		int coveringRoll = Roll.Dice(1, 6);
		
		// Force exoskeleton if creature has external skeleton
		if (creature.Skeleton == "External skeleton")
		{
			creature.SkinCovering = "Exoskeleton";
		}
		else
		{
			var coveringTable = BioData.Skin()["Covering"];
			creature.SkinCovering = Roll.Seek(coveringTable, coveringRoll);
		}
		
		// Roll for specific skin type
		int skinTypeRoll = Roll.Dice(2, 6);
		
		// Apply modifiers based on covering type
		switch (creature.SkinCovering)
		{
			case "Skin":
				if (habitatInfo.habitat == "Arctic") skinTypeRoll += 1;
				if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) skinTypeRoll += 1;
				if (habitatInfo.habitat == "Desert") skinTypeRoll -= 1;
				if ((creature.TrophicLevel ?? "").Contains("Herbivore")) skinTypeRoll += 1;
				if (creature.Locomotion == "winged flight") skinTypeRoll -= 5;
				break;
			case "Scales":
				if (habitatInfo.habitat == "Desert") skinTypeRoll += 1;
				if ((creature.TrophicLevel ?? "").Contains("Herbivore")) skinTypeRoll += 1;
				if (creature.Locomotion == "winged flight") skinTypeRoll -= 2;
				if (creature.Locomotion == "digging") skinTypeRoll -= 1;
				break;
			case "Fur":
				if (habitatInfo.habitat == "Desert") skinTypeRoll -= 1;
				if (habitatInfo.habitat == "Arctic") skinTypeRoll += 1;
				if (creature.Locomotion == "winged flight") skinTypeRoll -= 1;
				if ((creature.TrophicLevel ?? "").Contains("Herbivore")) skinTypeRoll += 1;
				break;
			case "Feathers":
				if (habitatInfo.habitat == "Desert") skinTypeRoll -= 1;
				if (habitatInfo.habitat == "Arctic") skinTypeRoll += 1;
				if (creature.Locomotion == "winged flight") skinTypeRoll += 1;
				break;
			case "Exoskeleton":
				skinTypeRoll = Roll.Dice(1, 6); // Use 1d6 for exoskeleton
				if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) skinTypeRoll += 1;
				if (creature.Locomotion == "immobile") skinTypeRoll += 1;
				if (creature.Locomotion == "winged flight") skinTypeRoll -= 2;
				break;
		}
		
		var skinTypeTable = BioData.Skin()[creature.SkinCovering];
		creature.SkinType = Roll.Seek(skinTypeTable, skinTypeRoll);
	}
	
	private void DetermineBreathing(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Land || creature.Locomotion == "winged flight")
		{
			creature.BreathingMethod = "Air-breathing";
			return;
		}
		
		if (habitatInfo.habitat == "Deep Ocean")
		{
			creature.BreathingMethod = "Water-breathing (Gills)";
			return;
		}
		
		// For other water dwellers, roll 2d6
		int breathingRoll = Roll.Dice(2, 6);
		
		// Apply modifiers
		if (new[] {"Arctic", "Swampland", "River", "Beach", "Lagoon"}.Contains(habitatInfo.habitat)) breathingRoll += 1;
		if (creature.Locomotion == "walking") breathingRoll += 1;
		if (new[] {"winged flight", "climbing"}.Contains(creature.Locomotion)) breathingRoll += 2;
		
		creature.BreathingMethod = breathingRoll >= 8 ? "Air-breathing" : "Water-breathing (Gills)";
	}
	
	private void DetermineTemperatureRegulation(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int tempRoll = Roll.Dice(2, 6);
		
		// Apply modifiers
		if ((creature.BreathingMethod ?? "").Contains("Air")) tempRoll += 1;
		if ((creature.BreathingMethod ?? "").Contains("Water")) tempRoll -= 1;
		if (creature.SizeCategory != "Small") tempRoll += 1;
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Land) tempRoll += 1;
		if (habitatInfo.habitat == "Woodland" || habitatInfo.habitat == "Mountain") tempRoll += 1;
		if (habitatInfo.habitat == "Arctic") tempRoll += 2;
		
		if (tempRoll >= 10)
			creature.TemperatureRegulation = "Warm-blooded";
		else if (tempRoll >= 7)
			creature.TemperatureRegulation = "Variable Temperature";
		else
			creature.TemperatureRegulation = "Cold-blooded";
	}
	
	private void DetermineGrowthPattern(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int growthRoll = Roll.Dice(2, 6);
		
		// Apply modifiers
		if (creature.Skeleton == "External skeleton") growthRoll -= 1;
		if (creature.SizeCategory == "Large") growthRoll += 1;
		if (creature.Locomotion == "immobile") growthRoll += 1;
		
		var growthTable = BioData.GrowthPattern();
		creature.GrowthPattern = Roll.Seek(growthTable, growthRoll);
	}
}
