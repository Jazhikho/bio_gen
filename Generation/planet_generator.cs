using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BioStructures;
using BioLibrary;

public partial class PlanetGenerator : Node
{
	public static PlanetGenerator Instance { get; private set; }

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

	public Planet GeneratePlanet(SettingsManager.PlanetSettings settings)
	{
		try
		{
			var planet = new Planet
			{
				Name = NamingGenerator.Instance.NameBiome(settings.PlanetType.ToString()),
				Settings = settings,
				LandMasses = new List<LandMass>(),
				WaterBodies = new List<WaterBody>()
			};

			if (settings.Hydrology < 100)
			{
				GenerateLandMasses(planet);
			}
			
			if (settings.Hydrology > 0)
			{
				GenerateWaterBodies(planet);
			}

			return planet;
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GeneratePlanet: {e.Message}\n{e.StackTrace}");
			return null;
		}
	}

	private void GenerateLandMasses(Planet planet)
	{
		int actualLandMasses = DetermineLandMassCount(planet.Settings);
		for (int i = 0; i < actualLandMasses; i++)
		{
			var landMass = new LandMass
			{
				Name = NamingGenerator.Instance.NameBiome("Continent"),
				Biomes = new List<Ecosystem>()
			};

			int biomeCount = DetermineBiomeCount(planet.Settings, true);
			for (int j = 0; j < biomeCount; j++)
			{
				var ecosystem = GenerateBiome(planet.Settings, HabitatGenerator.HabitatZone.Land);
				if (ecosystem != null)
				{
					landMass.Biomes.Add(ecosystem);
				}
			}

			if (landMass.Biomes.Any())
			{
				planet.LandMasses.Add(landMass);
			}
		}
	}

	private int DetermineLandMassCount(SettingsManager.PlanetSettings settings)
	{
		if (settings.Hydrology >= 100) return 0;
		
		int baseLandMasses = settings.LandMasses;
		if (settings.Hydrology > 80)
		{
			baseLandMasses = Math.Max(1, baseLandMasses - 1);
		}
		if (settings.Hydrology < 20)
		{
			baseLandMasses += 1;
		}
		
		return baseLandMasses;
	}

	private void GenerateWaterBodies(Planet planet)
	{
		int waterBodyCount = DetermineWaterBodyCount(planet.Settings);
		for (int i = 0; i < waterBodyCount; i++)
		{
			string waterType = DetermineWaterType(planet.Settings);
			var waterBody = new WaterBody
			{
				Name = NamingGenerator.Instance.NameBiome(waterType),
				Biomes = new List<Ecosystem>(),
				WaterType = waterType
			};

			int biomeCount = DetermineBiomeCount(planet.Settings, false);
			for (int j = 0; j < biomeCount; j++)
			{
				var ecosystem = GenerateBiome(planet.Settings, HabitatGenerator.HabitatZone.Water);
				if (ecosystem != null)
				{
					waterBody.Biomes.Add(ecosystem);
				}
			}

			if (waterBody.Biomes.Any())
			{
				planet.WaterBodies.Add(waterBody);
			}
		}
	}

	private int DetermineWaterBodyCount(SettingsManager.PlanetSettings settings)
	{
		if (settings.Hydrology <= 0) return 0;
		
		int baseCount = Math.Max(1, settings.LandMasses - 2);
		if (settings.Hydrology > 80)
		{
			baseCount += 1;
		}
		if (settings.Hydrology < 20)
		{
			baseCount = Math.Max(1, baseCount - 1);
		}
		
		return baseCount;
	}

	private int DetermineBiomeCount(SettingsManager.PlanetSettings settings, bool isLandMass)
	{
		int baseCount = isLandMass ? Roll.Dice(4) : Roll.Dice(1);
		
		switch (settings.PlanetType)
		{
			case SettingsManager.PlanetType.Gaian:
				baseCount += 1;
				break;
			case SettingsManager.PlanetType.Hydaean:
				baseCount = isLandMass ? Math.Max(1, baseCount - 1) : baseCount + 1;
				break;
			case SettingsManager.PlanetType.Arid:
				baseCount = isLandMass ? baseCount : Math.Max(1, baseCount - 1);
				break;
		}
		
		return baseCount;
	}

	private string DetermineWaterType(SettingsManager.PlanetSettings settings)
	{
		var waterTypes = new Dictionary<string, int>
		{
			{"Ocean", 6},
			{"Sea", 4},
			{"Lake", 2}
		};

		if (settings.Hydrology > 80)
		{
			waterTypes["Ocean"] = 8;
			waterTypes["Lake"] = Math.Max(1, waterTypes["Lake"] - 1);
		}
		else if (settings.Hydrology < 30)
		{
			waterTypes["Lake"] = 6;
			waterTypes["Ocean"] = Math.Max(1, waterTypes["Ocean"] - 1);
		}

		return Roll.Seek(waterTypes);
	}

	private Ecosystem GenerateBiome(SettingsManager.PlanetSettings settings, HabitatGenerator.HabitatZone zone)
	{
		if (settings.PlanetType != SettingsManager.PlanetType.Jovian && zone == HabitatGenerator.HabitatZone.Jovian)
		{
			zone = settings.Hydrology > 50 ? HabitatGenerator.HabitatZone.Water : HabitatGenerator.HabitatZone.Land;
		}

		var (habitatType, actualZone) = HabitatGenerator.Instance.DetermineHabitat(settings, zone);
		if (string.IsNullOrEmpty(habitatType)) return null;

		var ecosystem = new Ecosystem
		{
			HabitatType = habitatType,
			EcosystemID = Roll.Dice(3, 6),
			LocationID = Roll.Dice(3, 6)
		};

		int speciesCount = DetermineSpeciesCount(settings, zone);
		var creatures = new List<Creature>();
		
		for (int i = 0; i < speciesCount; i++)
		{
			var creature = GenerationManager.Instance.GenerateSingleSpecies(settings, ecosystem);
			if (creature != null)
			{
				creatures.Add(creature);
			}
		}

		ecosystem.Creatures = creatures.ToArray();
		return ecosystem;
	}

	private int DetermineSpeciesCount(SettingsManager.PlanetSettings settings, HabitatGenerator.HabitatZone zone)
	{
		int baseCount = Roll.Dice(6);
		
		// Modify based on planet type
		switch (settings.PlanetType)
		{
			case SettingsManager.PlanetType.Gaian:
				baseCount += 1;
				break;
			case SettingsManager.PlanetType.Arid:
			case SettingsManager.PlanetType.Snowball:
				baseCount = Math.Max(1, baseCount - 1);
				break;
		}
		
		// Modify based on zone and hydrology
		if (zone == HabitatGenerator.HabitatZone.Water && settings.Hydrology > 80)
		{
			baseCount += 1;
		}
		if (zone == HabitatGenerator.HabitatZone.Land && settings.Hydrology < 20)
		{
			baseCount = Math.Max(1, baseCount - 1);
		}
		
		return baseCount;
	}

	public void PopulateEcosystemTree(Tree tree, TreeItem root, Planet planet)
	{
		// Add land masses
		foreach (var landMass in planet.LandMasses)
		{
			var landMassItem = tree.CreateItem(root);
			landMassItem.SetText(0, landMass.Name);
			
			var landMassDict = new Godot.Collections.Dictionary
			{
				["ItemType"] = "LandMass",
				["Name"] = landMass.Name,
				["BiomeCount"] = landMass.Biomes.Count,
				["TotalSpecies"] = landMass.Biomes.Sum(b => b.Creatures.Length)
			};
			landMassItem.SetMetadata(0, landMassDict);

			AddBiomesToTree(tree, landMassItem, landMass.Biomes);
		}

		// Add water bodies
		foreach (var waterBody in planet.WaterBodies)
		{
			var waterBodyItem = tree.CreateItem(root);
			waterBodyItem.SetText(0, waterBody.Name);
			
			var waterBodyDict = new Godot.Collections.Dictionary
			{
				["ItemType"] = "WaterBody",
				["Name"] = waterBody.Name,
				["WaterType"] = waterBody.WaterType,
				["BiomeCount"] = waterBody.Biomes.Count,
				["TotalSpecies"] = waterBody.Biomes.Sum(b => b.Creatures.Length)
			};
			waterBodyItem.SetMetadata(0, waterBodyDict);

			AddBiomesToTree(tree, waterBodyItem, waterBody.Biomes);
		}
	}

	private void AddBiomesToTree(Tree tree, TreeItem parent, List<Ecosystem> biomes)
	{
		foreach (var biome in biomes)
		{
			var biomeItem = tree.CreateItem(parent);
			string biomeName = NamingGenerator.Instance.NameBiome(biome.HabitatType);
			biomeItem.SetText(0, biomeName);
			
			var biomeDict = new Godot.Collections.Dictionary
			{
				["ItemType"] = "Biome",
				["Name"] = biomeName,
				["Type"] = biome.HabitatType,
				["EcosystemID"] = biome.EcosystemID,
				["LocationID"] = biome.LocationID,
				["SpeciesCount"] = biome.Creatures.Length
			};
			biomeItem.SetMetadata(0, biomeDict);

			foreach (var creature in biome.Creatures)
			{
				AddCreatureToTree(tree, biomeItem, creature);
			}
		}
	}

	private void AddCreatureToTree(Tree tree, TreeItem parent, Creature creature)
	{
		var creatureItem = tree.CreateItem(parent);
		creatureItem.SetText(0, creature.Name);
		
		var creatureDict = new Godot.Collections.Dictionary
		{
			["ItemType"] = "Creature",
			["Name"] = creature.Name,
			["ChemicalBasis"] = creature.ChemicalBasis,
			["Habitat"] = creature.Habitat,
			["TrophicLevel"] = creature.TrophicLevel,
			["Locomotion"] = creature.Locomotion,
			["SizeCategory"] = creature.SizeCategory,
			["SpecificSize"] = creature.SpecificSize,
			["GravitySizeMultiplier"] = creature.GravitySizeMultiplier,
			["WeightInPounds"] = creature.WeightInPounds,
			["Symmetry"] = creature.Symmetry,
			["LimbStructure"] = creature.LimbStructure,
			["ActualLimbCount"] = creature.ActualLimbCount,
			["TailFeatures"] = creature.TailFeatures,
			["ManipulatorType"] = creature.ManipulatorType,
			["ActualManipulatorCount"] = creature.ActualManipulatorCount,
			["Skeleton"] = creature.Skeleton,
			["SkinCovering"] = creature.SkinCovering,
			["SkinType"] = creature.SkinType,
			["GrowthPattern"] = creature.GrowthPattern,
			["Sexes"] = creature.Sexes,
			["Gestation"] = creature.Gestation,
			["SpecialGestation"] = creature.SpecialGestation,
			["ReproductiveStrategy"] = creature.ReproductiveStrategy,
			["PrimarySense"] = creature.PrimarySense
		};

		// Add sense capabilities
		foreach (var sense in creature.SenseCapabilities)
		{
			creatureDict[$"Sense_{sense.Key}"] = sense.Value;
		}

		// Add special senses
		creatureDict["SpecialSenses"] = string.Join(", ", creature.SpecialSenses);

		// Add intelligence and behavior
		creatureDict["AnimalIntelligence"] = creature.AnimalIntelligence;
		creatureDict["MatingBehavior"] = creature.MatingBehavior;
		creatureDict["SocialOrganization"] = creature.SocialOrganization;

		// Add mental traits
		var mentalTraitsString = string.Join(", ", creature.MentalTraits.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
		creatureDict["MentalTraits"] = mentalTraitsString;

		creatureItem.SetMetadata(0, creatureDict);
	}
}
