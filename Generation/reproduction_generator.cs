using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BioLibrary;
using BioStructures;

public partial class ReproductionGenerator : Node
{
	public static ReproductionGenerator Instance { get; private set; }

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

	public void GenerateReproduction(Creature creature, SettingsManager.PlanetSettings settings)
	{
		// Special case for machines
		if (creature.ChemicalBasis == "Machine")
		{
			creature.Sexes = "Asexual reproduction or Parthenogenesis";
			creature.Gestation = "Replication";
			creature.SpecialGestation = null;
			creature.ReproductiveStrategy = "Moderate r-Strategy";  // Default for machines
			return;
		}

		creature.Sexes = DetermineSexes(creature, settings);
		(creature.Gestation, creature.SpecialGestation) = DetermineGestation(creature, settings);
		creature.ReproductiveStrategy = DetermineReproductiveStrategy(creature, settings);
	}

	private string DetermineSexes(Creature creature, SettingsManager.PlanetSettings settings)
	{
		int roll = Roll.Dice(2, 6);
		
		// Apply modifiers
		if (creature.Locomotion == "immobile") roll -= 1;
		if (creature.Symmetry == "Asymmetric") roll -= 1;
		if (creature.TrophicLevel == "Photosynthetic" || creature.TrophicLevel == "Chemosynthetic") roll -= 1;
		
		var sexesTable = BioData.Sexes();
		return Roll.Seek(sexesTable, roll);
	}

	private (string gestation, string specialGestation) DetermineGestation(Creature creature, SettingsManager.PlanetSettings settings)
	{
		// Basic gestation
		int roll = Roll.Dice(2, 6);
		
		// Apply modifiers
		if (creature.Habitat.Contains("Ocean") || creature.Habitat == "Lake" || 
			creature.Habitat == "River" || creature.Habitat == "Lagoon" || 
			creature.Habitat == "Deep Ocean" || creature.Habitat == "Sea" || 
			creature.Habitat == "Reef") roll -= 1;
		if (creature.Locomotion == "immobile") roll -= 2;
		if (creature.TemperatureRegulation == "Warm-blooded") roll += 1;
		
		var gestationTable = BioData.Gestation();
		string gestation = Roll.Seek(gestationTable, roll);

		// Special gestation check
		string specialGestation = null;
		int specialRoll = Roll.Dice(2, 6);
		if (specialRoll == 12)
		{
			var specialGestationTable = BioData.SpecialGestation();
			specialGestation = Roll.Seek(specialGestationTable, Roll.Dice(2, 6));
		}

		return (gestation, specialGestation);
	}

	private string DetermineReproductiveStrategy(Creature creature, SettingsManager.PlanetSettings settings)
	{
		int roll = Roll.Dice(2, 6);
		
		// Apply modifiers
		if (creature.SizeCategory == "Large") roll -= 2;
		if (creature.SizeCategory == "Small") roll += 1;
		if (creature.Gestation == "Spawning/Pollinating") roll += 2;
		
		var strategyTable = BioData.ReproductiveStrategy();
		string strategy = Roll.Seek(strategyTable, roll);

		// Calculate number of young per litter for spawning organisms
		if (creature.Gestation == "Spawning/Pollinating")
		{
			int litterSize = Roll.Dice(2) * 10;  // 2d * 10
			// This could be stored in a new Creature property if needed
		}

		return strategy;
	}
}
