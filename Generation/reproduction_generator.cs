using Godot;
using System;
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
		creature.GrowthPattern = DetermineGrowthPattern(creature);
		creature.Sexes = DetermineSexes(creature);
		(creature.Gestation, creature.SpecialGestation) = DetermineGestation(creature, settings);
		creature.ReproductiveStrategy = DetermineReproductiveStrategy(creature);
	}

	private string DetermineGrowthPattern(Creature creature)
	{
		int growthRoll = Roll.Dice(2);
		growthRoll += CalculateGrowthPatternModifiers(creature);
		
		return Roll.Seek(BioData.GrowthPattern(), growthRoll);
	}

	private int CalculateGrowthPatternModifiers(Creature creature)
	{
		int modifier = 0;
		
		if (creature.Skeleton == "External skeleton") modifier -= 1;
		if (creature.SizeCategory == "Large") modifier += 1;
		if (creature.Locomotion == "Immobile") modifier += 1;

		return modifier;
	}

	private string DetermineSexes(Creature creature)
	{
		int sexRoll = Roll.Dice(2);
		sexRoll += CalculateSexesModifiers(creature);
		
		return Roll.Seek(BioData.Sexes(), sexRoll);
	}

	private int CalculateSexesModifiers(Creature creature)
	{
		int modifier = 0;
		
		if (creature.Locomotion == "Immobile") modifier -= 1;
		if (creature.Symmetry == "Asymmetric") modifier -= 1;
		if (creature.TrophicLevel == "Autotroph" || 
			creature.TrophicLevel == "Chemosynthetic" || 
			creature.TrophicLevel == "Photosynthetic") 
			modifier -= 1;

		return modifier;
	}

	private (string gestation, string specialGestation) DetermineGestation(Creature creature, SettingsManager.PlanetSettings settings)
	{
		int gestationRoll = Roll.Dice(2);
		gestationRoll += CalculateGestationModifiers(creature, settings);
		
		string gestation = Roll.Seek(BioData.Gestation(), gestationRoll);
		
		// Check for special gestation
		string specialGestation = null;
		if (Roll.Dice(2) == 12)
		{
			int specialRoll = Roll.Dice(1);
			specialGestation = Roll.Seek(BioData.SpecialGestation(), specialRoll);
		}

		return (gestation, specialGestation);
	}

	private int CalculateGestationModifiers(Creature creature, SettingsManager.PlanetSettings settings)
	{
		int modifier = 0;
		
		if (creature.Locomotion == "Swimming" || creature.Locomotion == "Floating") modifier -= 1;
		if (creature.Locomotion == "Immobile") modifier -= 2;
		
		// Assuming we track temperature regulation. If not, we can remove this part
		bool isWarmBlooded = DetermineIfWarmBlooded(creature, settings);
		if (isWarmBlooded) modifier += 1;

		return modifier;
	}

	private bool DetermineIfWarmBlooded(Creature creature, SettingsManager.PlanetSettings settings)
	{
		// This is a placeholder. In reality, this would be based on various factors
		// If we're not tracking temperature regulation in the creature, we can remove this
		return Roll.Dice(1) > 3;
	}

	private string DetermineReproductiveStrategy(Creature creature)
	{
		int strategyRoll = Roll.Dice(2);
		strategyRoll += CalculateReproductiveStrategyModifiers(creature);
		
		return Roll.Seek(BioData.ReproductiveStrategy(), strategyRoll);
	}

	private int CalculateReproductiveStrategyModifiers(Creature creature)
	{
		int modifier = 0;
		
		if (creature.SizeCategory == "Large") modifier -= 2;
		if (creature.SizeCategory == "Small") modifier += 1;
		if (creature.Gestation == "Spawning") modifier += 2;

		return modifier;
	}
}
