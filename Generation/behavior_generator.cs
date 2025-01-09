using Godot;
using System;
using System.Collections.Generic;
using BioLibrary;
using BioStructures;

public partial class BehaviorGenerator : Node
{
	public static BehaviorGenerator Instance { get; private set; }

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

	public void GenerateBehavior(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		creature.TrophicLevel = DetermineTrophicLevel();
		creature.Locomotion = DetermineLocomotion(creature, habitatInfo);
		GenerateSenses(creature, habitatInfo);
		GenerateIntelligenceAndBehavior(creature, habitatInfo);
	}

	private string DetermineTrophicLevel()
	{
		return Roll.Seek(BioData.Trophic_Level()["Non-sapient"]);
	}

	private string DetermineLocomotion(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int locomotionRoll = Roll.Dice(2);
		locomotionRoll += CalculateLocomotionModifiers(creature);
		
		var locomotionOptions = BioData.Primary_Locomotion()[habitatInfo.habitat];
		return Roll.Seek(locomotionOptions, locomotionRoll);
	}

	private int CalculateLocomotionModifiers(Creature creature)
	{
		int modifier = 0;
		
		switch (creature.TrophicLevel)
		{
			case "Pouncing Carnivore":
			case "Chasing Carnivore":
			case "Omnivore":
			case "Gathering Herbivore":
			case "Scavenger":
				modifier += 1;
				break;
		}

		return modifier;
	}

	private void GenerateSenses(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Determine primary sense
		int senseRoll = Roll.Dice(3);
		senseRoll += CalculatePrimarySenseModifiers(creature, habitatInfo);
		
		var primarySense = Roll.Seek(BioData.Senses()["Primary Sense"], senseRoll);
		
		// Generate detailed sense information
		var senseDetails = new List<string> { primarySense };
		
		// Vision
		if (primarySense == "Vision" || Roll.Dice(3) + 4 >= 11)
		{
			int visionRoll = Roll.Dice(3) + (primarySense == "Vision" ? 4 : 0);
			visionRoll += CalculateVisionModifiers(creature, habitatInfo);
			senseDetails.Add(Roll.Seek(BioData.Senses()["Vision"], visionRoll));
		}

		// Hearing
		if (primarySense == "Hearing" || Roll.Dice(3) + 4 >= 11)
		{
			int hearingRoll = Roll.Dice(3) + (primarySense == "Hearing" ? 4 : 0);
			hearingRoll += CalculateHearingModifiers(creature, habitatInfo);
			senseDetails.Add(Roll.Seek(BioData.Senses()["Hearing"], hearingRoll));
		}

		// Special senses
		foreach (var specialSense in BioData.Senses()["Special Senses"])
		{
			if (IsSpecialSenseViable(specialSense.Key, creature, habitatInfo))
			{
				int specialRoll = Roll.Dice(2) + CalculateSpecialSenseModifiers(specialSense.Key, creature, habitatInfo);
				if (specialRoll >= 11)
				{
					senseDetails.Add(specialSense.Key);
				}
			}
		}

		creature.Senses = string.Join(", ", senseDetails);
	}

	private int CalculatePrimarySenseModifiers(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;
		
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) modifier -= 2;
		if (creature.TrophicLevel.Contains("autotroph")) modifier += 2;

		return modifier;
	}

	private int CalculateVisionModifiers(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;
		
		if (creature.Locomotion == "Digging") modifier -= 4;
		if (creature.Locomotion == "Climbing") modifier += 2;
		if (creature.Locomotion == "Winged flight") modifier += 3;
		if (habitatInfo.habitat == "Deep Ocean") modifier -= 4;
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) modifier -= 2;
		if (creature.TrophicLevel == "Filter Feeder") modifier -= 2;
		if (creature.TrophicLevel.Contains("Carnivore") || creature.TrophicLevel == "Gathering Herbivore") modifier += 2;

		return modifier;
	}

	private int CalculateHearingModifiers(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;
		
		// Add hearing modifiers based on other senses and conditions
		if (creature.Senses.Contains("Blind")) modifier += 2;
		if (creature.Senses.Contains("Bad Sight")) modifier += 1;
		if (habitatInfo.zone == HabitatGenerator.HabitatZone.Water) modifier += 1;

		return modifier;
	}

	private bool IsSpecialSenseViable(string sense, Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		switch (sense)
		{
			case "Ultravision":
				return habitatInfo.zone != HabitatGenerator.HabitatZone.Water && creature.ChemicalBasis != "Ammonia-Based";
			case "Detect (Heat)":
				return habitatInfo.zone != HabitatGenerator.HabitatZone.Water;
			case "Detect (Electric Fields)":
				return habitatInfo.zone == HabitatGenerator.HabitatZone.Water;
			case "Perfect Balance":
				return habitatInfo.zone == HabitatGenerator.HabitatZone.Land;
			default:
				return true;
		}
	}

	private int CalculateSpecialSenseModifiers(string sense, Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;
		
		switch (sense)
		{
			case "360° Vision":
			case "Peripheral Vision":
				if (habitatInfo.habitat == "Plains" || habitatInfo.habitat == "Desert") modifier += 1;
				if (creature.TrophicLevel.Contains("Herbivore")) modifier += 1;
				if (creature.Symmetry == "Radial" || creature.Symmetry == "Spherical") modifier += 1;
				break;
			case "Absolute Direction":
				if (habitatInfo.habitat == "Ocean") modifier += 1;
				if (creature.Locomotion == "Winged flight" || creature.Locomotion == "Digging") modifier += 1;
				break;
		}

		return modifier;
	}

	private void GenerateIntelligenceAndBehavior(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Animal Intelligence
		int intelligenceRoll = Roll.Dice(2) + CalculateIntelligenceModifiers(creature);
		creature.AnimalIntelligence = Roll.Seek(BioData.AnimalIntelligence()["Animal Intelligence"], intelligenceRoll);

		// Mental Qualities
		creature.MentalQualities = DetermineMentalQualities(creature, habitatInfo);
	}

	private int CalculateIntelligenceModifiers(Creature creature)
	{
		int modifier = 0;
		
		if (creature.TrophicLevel == "Autotroph" || 
			creature.TrophicLevel == "Filter Feeder" || 
			creature.TrophicLevel == "Grazing Herbivore") 
			modifier -= 1;
		
		if (creature.TrophicLevel == "Gathering Herbivore" || 
			creature.TrophicLevel == "Omnivore")
			modifier += 1;
		
		if (creature.SizeCategory == "Small") modifier -= 1;

		return modifier;
	}

	private string DetermineMentalQualities(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		var qualities = new List<string>();
		
		foreach (var quality in BioData.MentalQualities())
		{
			int baseRoll = Roll.Dice(1) - Roll.Dice(1);
			int modifier = CalculateMentalQualityModifier(quality.Key, creature, habitatInfo);
			
			int totalModifier = baseRoll + modifier;
			qualities.Add($"{quality.Key}: {DetermineMentalQualityLevel(totalModifier)}");
		}

		return string.Join(", ", qualities);
	}

	private int CalculateMentalQualityModifier(string quality, Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		int modifier = 0;
		
		switch (quality)
		{
			case "Curiosity":
				if (creature.TrophicLevel == "Omnivore") modifier += 1;
				if (creature.TrophicLevel == "Grazing Herbivore" || creature.TrophicLevel == "Filter Feeder") modifier -= 1;
				break;
			case "Gregariousness":
				if (creature.TrophicLevel == "Pouncing Carnivore" || 
					creature.TrophicLevel == "Scavenger" || 
					creature.TrophicLevel == "Filter Feeder" || 
					creature.TrophicLevel.Contains("Herbivore")) 
					modifier -= 1;
				break;
		}

		return modifier;
	}

	private string DetermineMentalQualityLevel(int modifier)
	{
		switch (modifier)
		{
			case int n when n >= 3: return "Very High";
			case int n when n == 2: return "High";
			case int n when n == 1: return "Above Average";
			case 0: return "Average";
			case int n when n == -1: return "Below Average";
			case int n when n == -2: return "Low";
			default: return "Very Low";
		}
	}
}
