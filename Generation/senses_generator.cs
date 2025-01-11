using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BioLibrary;
using BioStructures;

public partial class SensesGenerator : Node
{
	public static SensesGenerator Instance { get; private set; }

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

	public void GenerateSenses(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		try
		{
			DeterminePrimarySense(creature, settings, habitatInfo);
			GenerateDetailedSenses(creature, settings, habitatInfo);
			GenerateSpecialSenses(creature, settings, habitatInfo);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GenerateSenses: {e.Message}\n{e.StackTrace}");
		}
	}

	private void DeterminePrimarySense(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int roll = Roll.Dice();  // 3d6
		
		// Apply modifiers
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) roll -= 2;
		if (creature.TrophicLevel == "Photosynthetic" || creature.TrophicLevel == "Chemosynthetic") roll += 2;
		
		var senseTable = BioData.Senses()["Primary Sense"];
		creature.PrimarySense = Roll.Seek(senseTable, roll);
	}

	private void GenerateDetailedSenses(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		creature.SenseCapabilities = new Dictionary<string, string>();

		// Vision capabilities
		int visionRoll = Roll.Dice();
		if (creature.PrimarySense == "Vision") visionRoll += 4;
		if (creature.Locomotion == "digging") visionRoll -= 4;
		if (creature.Locomotion == "climbing") visionRoll += 2;
		if (creature.Locomotion == "winged flight") visionRoll += 3;
		if (creature.Locomotion == "immobile") visionRoll -= 4;
		if (habitatInfo.habitat == "Deep Ocean") visionRoll -= 4;
		if (creature.TrophicLevel == "Filter Feeder") visionRoll -= 2;
		if (creature.TrophicLevel.Contains("Carnivore") || creature.TrophicLevel == "Gathering Herbivore") visionRoll += 2;
		
		var visionTable = BioData.Senses()["Vision"];
		creature.SenseCapabilities["Vision"] = Roll.Seek(visionTable, visionRoll);

		// Hearing capabilities
		int hearingRoll = Roll.Dice();
		if (creature.PrimarySense == "Hearing") hearingRoll += 4;
		if (creature.SenseCapabilities["Vision"] == "Blindness" || creature.SenseCapabilities["Vision"] == "Light Sense") hearingRoll += 2;
		if (creature.SenseCapabilities["Vision"] == "Bad Sight and Colorblindness" || creature.SenseCapabilities["Vision"] == "Bad Sight") hearingRoll += 1;
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) hearingRoll += 1;
		if (creature.Locomotion == "immobile") hearingRoll -= 4;
		
		var hearingTable = BioData.Senses()["Hearing"];
		creature.SenseCapabilities["Hearing"] = Roll.Seek(hearingTable, hearingRoll);

		// Touch capabilities
		int touchRoll = Roll.Dice(2, 6);
		if (creature.PrimarySense == "Touch and Taste") touchRoll += 4;
		if (creature.Skeleton == "External skeleton") touchRoll -= 2;
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) touchRoll += 2;
		if (creature.Locomotion == "digging") touchRoll += 2;
		if (creature.Locomotion == "winged flight") touchRoll -= 2;
		if (creature.SenseCapabilities["Vision"] == "Blindness" || creature.SenseCapabilities["Vision"] == "Light Sense") touchRoll += 2;
		if (creature.TrophicLevel == "Trapping Carnivore") touchRoll += 1;
		if (creature.SizeCategory == "Small") touchRoll += 1;
		
		var touchTable = BioData.Senses()["Touch"];
		creature.SenseCapabilities["Touch"] = Roll.Seek(touchTable, touchRoll);

		// Taste/Smell capabilities
		int tasteSmellRoll = Roll.Dice(2, 6);
		if (creature.PrimarySense == "Touch and Taste") tasteSmellRoll += 4;
		if (creature.TrophicLevel == "Chasing Carnivore" || creature.TrophicLevel == "Gathering Herbivore") tasteSmellRoll += 2;
		if (new[] {"Filter Feeder", "Photosynthetic", "Chemosynthetic", "Trapping Carnivore"}.Contains(creature.TrophicLevel)) tasteSmellRoll -= 2;
		if (creature.Sexes != "Asexual reproduction or Parthenogenesis") tasteSmellRoll += 2;
		if (creature.Locomotion == "immobile") tasteSmellRoll -= 4;
		
		var tasteSmellTable = BioData.Senses()["Taste/Smell"];
		creature.SenseCapabilities["Taste/Smell"] = Roll.Seek(tasteSmellTable, tasteSmellRoll);
	}

	private void GenerateSpecialSenses(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		creature.SpecialSenses = new List<string>();

		// 360° Vision
		int vision360Roll = Roll.Dice(2, 6);
		if (new[] {"Plain", "Desert"}.Contains(habitatInfo.habitat)) vision360Roll += 1;
		if (creature.TrophicLevel.Contains("Herbivore")) vision360Roll += 1;
		if (new[] {"Radial", "Spherical"}.Contains(creature.Symmetry)) vision360Roll += 1;
		if (vision360Roll >= 11)
			creature.SpecialSenses.Add("360° Vision");

		// Absolute Direction
		int directionRoll = Roll.Dice(2, 6);
		if (habitatInfo.habitat == "Ocean") directionRoll += 1;
		if (creature.Locomotion == "winged flight") directionRoll += 1;
		if (creature.Locomotion == "digging") directionRoll += 1;
		if (directionRoll >= 11)
			creature.SpecialSenses.Add("Absolute Direction");

		// Discriminatory Hearing
		int discHearingRoll = Roll.Dice(2, 6);
		if (creature.SenseCapabilities["Hearing"] == "Ultrasonic Hearing") discHearingRoll += 2;
		if (discHearingRoll >= 11)
			creature.SpecialSenses.Add("Discriminatory Hearing");

		// Peripheral Vision
		int peripheralRoll = Roll.Dice(2, 6);
		if (new[] {"Plain", "Desert"}.Contains(habitatInfo.habitat)) peripheralRoll += 1;
		if (creature.TrophicLevel.Contains("Herbivore")) peripheralRoll += 2;
		if (peripheralRoll >= 11)
			creature.SpecialSenses.Add("Peripheral Vision");

		// Night Vision
		int nightVisionRoll = Roll.Dice(2, 6);
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) nightVisionRoll += 2;
		if (creature.TrophicLevel.Contains("Carnivore")) nightVisionRoll += 2;
		if (nightVisionRoll >= 11)
			creature.SpecialSenses.Add("Night Vision (1d+3)");

		// Ultravision
		if (habitatInfo.zone != HabitatGenerator.HabitatZone.Water && creature.ChemicalBasis != "Ammonia-Based")
		{
			int ultravisionRoll = Roll.Dice(2, 6);
			if (ultravisionRoll >= 11)
				creature.SpecialSenses.Add("Ultravision");
		}

		// Heat Detection
		if (habitatInfo.zone != HabitatGenerator.HabitatZone.Water)
		{
			int heatRoll = Roll.Dice(2, 6);
			if (creature.TrophicLevel.Contains("Carnivore")) heatRoll += 1;
			if (habitatInfo.habitat == "Arctic") heatRoll += 1;
			if (heatRoll >= 11)
				creature.SpecialSenses.Add("Heat Sensitive");
		}

		// Electric Field Detection
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water)
		{
			int electricRoll = Roll.Dice(2, 6);
			if (creature.TrophicLevel.Contains("Carnivore")) electricRoll += 1;
			if (electricRoll >= 11)
				creature.SpecialSenses.Add("Sensitive to Electric Fields");
		}

		// Perfect Balance
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Land)
		{
			int balanceRoll = Roll.Dice(2, 6);
			if (creature.Locomotion == "climbing") balanceRoll += 2;
			if (habitatInfo.habitat == "Mountain") balanceRoll += 1;
			if (settings.Gravity <= 0.5) balanceRoll -= 1;
			if (settings.Gravity >= 1.5) balanceRoll += 1;
			if (balanceRoll >= 11)
				creature.SpecialSenses.Add("Perfect Balance");
		}
	}
}
