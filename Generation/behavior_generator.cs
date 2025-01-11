using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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

	public void GenerateBehavior(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		try
		{
			GenerateIntelligenceAndBehavior(creature, settings, habitatInfo);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GenerateBehavior: {e.Message}\n{e.StackTrace}");
		}
	}

	private void GenerateIntelligenceAndBehavior(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Animal Intelligence (2d6)
		int intelligenceRoll = Roll.Dice(2);
		if (new [] {"Photosynthetic", "Chemosynthetic", "Filter Feeder", "Grazing Herbivore"}.Contains(creature.TrophicLevel)) intelligenceRoll -= 1;
		if ((creature.TrophicLevel ?? "") == "Gathering Herbivore" || (creature.TrophicLevel ?? "") == "Omnivore") intelligenceRoll += 1;
		if (creature.SizeCategory == "Small") intelligenceRoll -= 1;
		if ((creature.ReproductiveStrategy ?? "") == "Strong r-Strategy") intelligenceRoll -= 1;
		if ((creature.ReproductiveStrategy ?? "") == "Strong K-Strategy") intelligenceRoll += 1;
		
		var intelligenceTable = BioData.AnimalIntelligence()["Animal Intelligence"];
		creature.AnimalIntelligence = Roll.Seek(intelligenceTable, intelligenceRoll);

		// Mating Behavior (2d6)
		int matingRoll = Roll.Dice(2);
		if ((creature.Sexes ?? "") == "Hermaphrodite") matingRoll -= 2;
		if ((creature.Gestation ?? "") == "Spawning/Pollinating") matingRoll -= 1;
		if ((creature.Gestation ?? "") == "Live-Bearing") matingRoll += 1;
		
		var matingTable = BioData.AnimalIntelligence()["Mating Behavior"];
		creature.MatingBehavior = Roll.Seek(matingTable, matingRoll);

		// Social Organization (2d6)
		int socialRoll = Roll.Dice(2);
		if ((creature.TrophicLevel ?? "").Contains("Carnivore")) socialRoll -= 1;
		if ((creature.TrophicLevel ?? "") == "Grazing Herbivore") socialRoll += 1;
		if (creature.SizeCategory == "Large") socialRoll -= 1;
		if ((creature.MatingBehavior ?? "") == "Harem") socialRoll += 1;
		if ((creature.MatingBehavior ?? "") == "Mating only, no pair bond") socialRoll -= 1;
		
		var socialTable = BioData.AnimalIntelligence()["Social Organization Type"];
		creature.SocialOrganization = Roll.Seek(socialTable, socialRoll);

		// Mental Qualities
		GenerateMentalQualities(creature, settings, habitatInfo);
	}

	private void GenerateMentalQualities(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		creature.MentalTraits = new Dictionary<string, string>();

		// Initialize properties with default values if they are null
		creature.ReproductiveStrategy = creature.ReproductiveStrategy ?? "Balanced Strategy";
		creature.AnimalIntelligence = creature.AnimalIntelligence ?? "Instinctual";
		creature.SocialOrganization = creature.SocialOrganization ?? "Solitary";
		creature.Sexes = creature.Sexes ?? "Male/Female";
		creature.Gestation = creature.Gestation ?? "Live-Bearing";

		// Chauvinism
		int chauvScore = RollMentalTrait();
		var chauvTable = BioData.MentalQualities()["Chauvinism"];
		if ((creature.TrophicLevel ?? "") is "Photosynthetic" or "Chemosynthetic" or "Filter Feeder") chauvScore -= 1;
		if ((creature.TrophicLevel ?? "") is "Parasite" or "Scavenger") chauvScore -= 2;
		if (creature.SocialOrganization.Contains("group") || creature.SocialOrganization == "Hive") chauvScore += 2;
		if (creature.Sexes is "Asexual reproduction or Parthenogenesis" || creature.Gestation == "Spawning/Pollinating") chauvScore -= 1;
		if (creature.SocialOrganization is "Solitary" or "Pair-bonded") chauvScore -= 1;
		
		// Concentration
		int concScore = RollMentalTrait();
		var concTable = BioData.MentalQualities()["Concentration"];
		if ((creature.TrophicLevel ?? "") is "Pouncing Carnivore" or "Chasing Carnivore") concScore += 1;
		if ((creature.TrophicLevel ?? "").Contains("Herbivore")) concScore -= 1;
		if (creature.ReproductiveStrategy == "Strong K-Strategy") concScore += 1;
		
		// Curiosity
		int curScore = RollMentalTrait();
		var curTable = BioData.MentalQualities()["Curiosity"];
		if ((creature.TrophicLevel ?? "") == "Omnivore") curScore += 1;
		if ((creature.TrophicLevel ?? "") is "Grazing Herbivore" or "Filter Feeder") curScore -= 1;
		if ((creature.SenseCapabilities?["Vision"] ?? "") is "Blindness" or "Light Sense") curScore -= 1;
		if (creature.ReproductiveStrategy == "Strong r-Strategy") curScore -= 1;
		if (creature.ReproductiveStrategy == "Strong K-Strategy") curScore += 1;
		
		// Egoism
		int egoScore = RollMentalTrait();
		var egoTable = BioData.MentalQualities()["Egoism"];
		if (creature.SocialOrganization == "Solitary") egoScore += 1;
		if (creature.MatingBehavior == "Harem") egoScore += 1;
		if (creature.SocialOrganization == "Hive") egoScore -= 1;
		if (creature.ReproductiveStrategy == "Strong K-Strategy") egoScore += 1;
		if (creature.ReproductiveStrategy == "Strong r-Strategy") egoScore -= 1;
		
		// Empathy
		int empScore = RollMentalTrait();
		var empTable = BioData.MentalQualities()["Empathy"];
		if ((creature.TrophicLevel ?? "") == "Chasing Carnivore") empScore += 1;
		if ((creature.TrophicLevel ?? "") is "Photosynthetic" or "Chemosynthetic" or "Filter Feeder" or "Grazing Herbivore" or "Scavenger") empScore -= 1;
		if (creature.SocialOrganization is "Solitary" or "Pair-bonded") empScore -= 1;
		if (creature.SocialOrganization.Contains("group")) empScore += 1;
		if (creature.ReproductiveStrategy == "Strong K-Strategy") empScore += 1;
		
		// Gregariousness
		int gregScore = RollMentalTrait();
		var gregTable = BioData.MentalQualities()["Gregariousness"];
		if ((creature.TrophicLevel ?? "") is "Pouncing Carnivore" or "Scavenger" or "Filter Feeder" or "Photosynthetic" or "Chemosynthetic") gregScore -= 1;
		if ((creature.TrophicLevel ?? "").Contains("Herbivore")) gregScore -= 1;
		if (creature.SocialOrganization is "Solitary" or "Pair-bonded") gregScore -= 1;
		if (creature.SocialOrganization.Contains("Medium") || creature.SocialOrganization.Contains("Large")) gregScore += 1;
		if (creature.SocialOrganization == "Hive") gregScore += 2;
		if (creature.Sexes is "Asexual reproduction or Parthenogenesis" or "Hermaphrodite") gregScore += 1;
		if (creature.Gestation == "Spawning/Pollinating") gregScore -= 1;
		
		// Imagination
		int imagScore = RollMentalTrait();
		var imagTable = BioData.MentalQualities()["Imagination"];
		if ((creature.TrophicLevel ?? "") is "Pouncing Carnivore" or "Omnivore" or "Gathering Herbivore") imagScore += 1;
		if ((creature.TrophicLevel ?? "") is "Photosynthetic" or "Chemosynthetic" or "Filter Feeder" or "Grazing Herbivore") imagScore -= 1;
		if (creature.ReproductiveStrategy == "Strong K-Strategy") imagScore += 1;
		if (creature.ReproductiveStrategy == "Strong r-Strategy") imagScore -= 1;
		
		// Suspicion
		int suspScore = RollMentalTrait();
		var suspTable = BioData.MentalQualities()["Suspicion"];
		if ((creature.TrophicLevel ?? "").Contains("Carnivore")) suspScore -= 1;
		if ((creature.TrophicLevel ?? "") == "Grazing Herbivore") suspScore += 1;
		if ((creature.SenseCapabilities?["Vision"] ?? "") is "Blindness" or "Light Sense") suspScore += 1;
		if (creature.SizeCategory == "Large") suspScore -= 1;
		if (creature.SizeCategory == "Small") suspScore += 1;
		if (creature.SocialOrganization is "Solitary" or "Pair-bonded") suspScore += 1;
		
		// Playfulness
		int playScore = RollMentalTrait();
		var playTable = BioData.MentalQualities()["Playfulness"];
		if (creature.ReproductiveStrategy == "Strong K-Strategy") playScore += 2;
		else if (creature.ReproductiveStrategy.Contains("K-Strategy")) playScore += 1;
		if (creature.AnimalIntelligence != "Mindless" && creature.AnimalIntelligence != "Instinctual") playScore += 1;
		if (creature.SocialOrganization == "Solitary") playScore -= 1;
		if (creature.AnimalIntelligence is "Instinctual" or "Mindless") playScore -= 3;

		// Add all scores to mental traits
		creature.MentalTraits["Chauvinism"] = Roll.Seek(chauvTable, chauvScore);
		creature.MentalTraits["Concentration"] = Roll.Seek(concTable, concScore);
		creature.MentalTraits["Curiosity"] = Roll.Seek(curTable, curScore);
		creature.MentalTraits["Egoism"] = Roll.Seek(egoTable, egoScore);
		creature.MentalTraits["Empathy"] = Roll.Seek(empTable, empScore);
		creature.MentalTraits["Gregariousness"] = Roll.Seek(gregTable, gregScore);
		creature.MentalTraits["Imagination"] = Roll.Seek(imagTable, imagScore);
		creature.MentalTraits["Suspicion"] = Roll.Seek(suspTable, suspScore);
		creature.MentalTraits["Playfulness"] = Roll.Seek(playTable, playScore);
	}

	private int RollMentalTrait()
	{
		int die1 = Roll.Dice(1, 6);
		int die2 = Roll.Dice(1, 6);
		return die1 - die2;
	}
}
