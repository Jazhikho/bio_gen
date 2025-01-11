using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using BioStructures;

public partial class DataManager : Node
{
	public static DataManager Instance { get; private set; }

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

	public void SaveEcosystem(Godot.Collections.Dictionary dict, string filePath)
	{
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		if (file == null)
		{
			GD.PrintErr($"Failed to open file for writing: {filePath}");
			return;
		}

		var jsonString = Json.Stringify(dict);
		file.StoreString(jsonString);
	}

	public Ecosystem LoadEcosystem(string filePath)
	{
		if (!FileAccess.FileExists(filePath))
		{
			GD.PrintErr("File not found: " + filePath);
			return null;
		}

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			GD.PrintErr($"Failed to open file for reading: {filePath}");
			return null;
		}

		var jsonString = file.GetAsText();
		var json = new Json();
		var error = json.Parse(jsonString);
		
		if (error == Error.Ok)
		{
			var data = json.Data;
			if (data.Obj is Godot.Collections.Dictionary dict)
			{
				return DictionaryToEcosystem(dict);
			}
		}

		GD.PrintErr("Failed to parse JSON or convert to Ecosystem");
		return null;
	}

	public Godot.Collections.Dictionary PlanetToDictionary(Planet planet)
	{
		var dict = new Godot.Collections.Dictionary();
		dict["Name"] = planet.Name;
		
		// Save settings
		var settingsDict = new Godot.Collections.Dictionary();
		settingsDict["PlanetType"] = (int)planet.Settings.PlanetType;
		settingsDict["Temperature"] = planet.Settings.Temperature;
		settingsDict["Hydrology"] = planet.Settings.Hydrology;
		settingsDict["Gravity"] = planet.Settings.Gravity;
		settingsDict["LandMasses"] = planet.Settings.LandMasses;
		settingsDict["PrimaryChemistry"] = (int)planet.Settings.PrimaryChemistry;
		dict["Settings"] = settingsDict;
		
		// Save land masses
		var landMassesArray = new Godot.Collections.Array();
		foreach (var landMass in planet.LandMasses)
		{
			var landMassDict = new Godot.Collections.Dictionary();
			landMassDict["Name"] = landMass.Name;
			
			var biomesArray = new Godot.Collections.Array();
			foreach (var ecosystem in landMass.Biomes)
			{
				biomesArray.Add(EcosystemToDictionary(ecosystem));
			}
			landMassDict["Biomes"] = biomesArray;
			landMassesArray.Add(landMassDict);
		}
		dict["LandMasses"] = landMassesArray;
		
		// Save water bodies
		var waterBodiesArray = new Godot.Collections.Array();
		foreach (var waterBody in planet.WaterBodies)
		{
			var waterBodyDict = new Godot.Collections.Dictionary();
			waterBodyDict["Name"] = waterBody.Name;
			waterBodyDict["WaterType"] = waterBody.WaterType;
			
			var biomesArray = new Godot.Collections.Array();
			foreach (var ecosystem in waterBody.Biomes)
			{
				biomesArray.Add(EcosystemToDictionary(ecosystem));
			}
			waterBodyDict["Biomes"] = biomesArray;
			waterBodiesArray.Add(waterBodyDict);
		}
		dict["WaterBodies"] = waterBodiesArray;
		
		return dict;
	}

	public Planet DictionaryToPlanet(Godot.Collections.Dictionary dict)
	{
		var planet = new Planet();
		planet.Name = dict["Name"].AsString();
		
		// Load settings
		var settingsDict = dict["Settings"].AsGodotDictionary();
		var settings = new SettingsManager.PlanetSettings();
		settings.PlanetType = (SettingsManager.PlanetType)settingsDict["PlanetType"].AsInt32();
		settings.Temperature = settingsDict["Temperature"].AsSingle();
		settings.Hydrology = settingsDict["Hydrology"].AsSingle();
		settings.Gravity = settingsDict["Gravity"].AsSingle();
		settings.LandMasses = settingsDict["LandMasses"].AsInt32();
		settings.PrimaryChemistry = (SettingsManager.PlanetSettings.ChemistryBasis)settingsDict["PrimaryChemistry"].AsInt32();
		planet.Settings = settings;
		
		// Load land masses
		planet.LandMasses = new List<LandMass>();
		var landMassesArray = dict["LandMasses"].AsGodotArray();
		foreach (var lm in landMassesArray)
		{
			var landMassDict = lm.AsGodotDictionary();
			var landMass = new LandMass();
			landMass.Name = landMassDict["Name"].AsString();
			
			landMass.Biomes = new List<Ecosystem>();
			var biomesArray = landMassDict["Biomes"].AsGodotArray();
			foreach (var eco in biomesArray)
			{
				landMass.Biomes.Add(DictionaryToEcosystem(eco.AsGodotDictionary()));
			}
			planet.LandMasses.Add(landMass);
		}
		
		// Load water bodies
		planet.WaterBodies = new List<WaterBody>();
		var waterBodiesArray = dict["WaterBodies"].AsGodotArray();
		foreach (var wb in waterBodiesArray)
		{
			var waterBodyDict = wb.AsGodotDictionary();
			var waterBody = new WaterBody();
			waterBody.Name = waterBodyDict["Name"].AsString();
			waterBody.WaterType = waterBodyDict["WaterType"].AsString();
			
			waterBody.Biomes = new List<Ecosystem>();
			var biomesArray = waterBodyDict["Biomes"].AsGodotArray();
			foreach (var eco in biomesArray)
			{
				waterBody.Biomes.Add(DictionaryToEcosystem(eco.AsGodotDictionary()));
			}
			planet.WaterBodies.Add(waterBody);
		}
		
		return planet;
	}

	public Godot.Collections.Dictionary EcosystemToDictionary(Ecosystem ecosystem)
	{
		var dict = new Godot.Collections.Dictionary();
		dict["HabitatType"] = ecosystem.HabitatType;
		dict["EcosystemID"] = ecosystem.EcosystemID;
		dict["LocationID"] = ecosystem.LocationID;

		var creaturesList = new Godot.Collections.Array();
		foreach (var creature in ecosystem.Creatures)
		{
			creaturesList.Add(CreatureToDictionary(creature));
		}
		dict["Creatures"] = creaturesList;

		return dict;
	}

	public Godot.Collections.Dictionary CreatureToDictionary(Creature creature)
	{
		var dict = new Godot.Collections.Dictionary
		{
			["ItemType"] = "Creature", // Added for tree compatibility
			["Name"] = creature.Name,
			["ChemicalBasis"] = creature.ChemicalBasis,
			["Habitat"] = creature.Habitat,
			["TrophicLevel"] = creature.TrophicLevel,
			["SizeCategory"] = creature.SizeCategory,
			["SpecificSize"] = creature.SpecificSize,
			["GravitySizeMultiplier"] = creature.GravitySizeMultiplier,
			["WeightInPounds"] = creature.WeightInPounds,
			["Symmetry"] = creature.Symmetry,
			["Locomotion"] = creature.Locomotion,
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
			["PrimarySense"] = creature.PrimarySense,
			["AnimalIntelligence"] = creature.AnimalIntelligence,
			["MatingBehavior"] = creature.MatingBehavior,
			["SocialOrganization"] = creature.SocialOrganization
		};

		// Add sense capabilities
		var senseCapabilities = new Godot.Collections.Dictionary();
		foreach (var sense in creature.SenseCapabilities)
		{
			senseCapabilities[sense.Key] = sense.Value;
		}
		dict["SenseCapabilities"] = senseCapabilities;

		// Add special senses
		var specialSensesArray = new Godot.Collections.Array();
		foreach (var sense in creature.SpecialSenses)
		{
			specialSensesArray.Add(sense);
		}
		dict["SpecialSenses"] = specialSensesArray;

		// Add mental traits
		var mentalTraits = new Godot.Collections.Dictionary();
		foreach (var trait in creature.MentalTraits)
		{
			mentalTraits[trait.Key] = trait.Value;
		}
		dict["MentalTraits"] = mentalTraits;

		return dict;
	}

	public Ecosystem DictionaryToEcosystem(Godot.Collections.Dictionary dict)
	{
		var ecosystem = new Ecosystem
		{
			HabitatType = dict["HabitatType"].AsString(),
			EcosystemID = dict["EcosystemID"].AsInt32(),
			LocationID = dict["LocationID"].AsInt32()
		};

		var creaturesList = dict["Creatures"].AsGodotArray();
		ecosystem.Creatures = new Creature[creaturesList.Count];
		for (int i = 0; i < creaturesList.Count; i++)
		{
			ecosystem.Creatures[i] = DictionaryToCreature(creaturesList[i].AsGodotDictionary());
		}

		return ecosystem;
	}

	public Creature DictionaryToCreature(Godot.Collections.Dictionary dict)
	{
		var creature = new Creature();
		try
		{
			// Basic properties with null checks
			if (dict.ContainsKey("Name")) creature.Name = dict["Name"].AsString();
			if (dict.ContainsKey("ChemicalBasis")) creature.ChemicalBasis = dict["ChemicalBasis"].AsString();
			if (dict.ContainsKey("Habitat")) creature.Habitat = dict["Habitat"].AsString();
			if (dict.ContainsKey("TrophicLevel")) creature.TrophicLevel = dict["TrophicLevel"].AsString();
			if (dict.ContainsKey("SizeCategory")) creature.SizeCategory = dict["SizeCategory"].AsString();
			if (dict.ContainsKey("SpecificSize")) creature.SpecificSize = dict["SpecificSize"].AsSingle();
			if (dict.ContainsKey("GravitySizeMultiplier")) creature.GravitySizeMultiplier = dict["GravitySizeMultiplier"].AsSingle();
			if (dict.ContainsKey("WeightInPounds")) creature.WeightInPounds = dict["WeightInPounds"].AsSingle();
			if (dict.ContainsKey("Symmetry")) creature.Symmetry = dict["Symmetry"].AsString();
			if (dict.ContainsKey("Locomotion")) creature.Locomotion = dict["Locomotion"].AsString();
			if (dict.ContainsKey("LimbStructure")) creature.LimbStructure = dict["LimbStructure"].AsString();
			if (dict.ContainsKey("ActualLimbCount")) creature.ActualLimbCount = dict["ActualLimbCount"].AsInt32();
			if (dict.ContainsKey("TailFeatures")) creature.TailFeatures = dict["TailFeatures"].AsString();
			if (dict.ContainsKey("ManipulatorType")) creature.ManipulatorType = dict["ManipulatorType"].AsString();
			if (dict.ContainsKey("ActualManipulatorCount")) creature.ActualManipulatorCount = dict["ActualManipulatorCount"].AsInt32();
			if (dict.ContainsKey("Skeleton")) creature.Skeleton = dict["Skeleton"].AsString();
			if (dict.ContainsKey("SkinCovering")) creature.SkinCovering = dict["SkinCovering"].AsString();
			if (dict.ContainsKey("SkinType")) creature.SkinType = dict["SkinType"].AsString();
			if (dict.ContainsKey("GrowthPattern")) creature.GrowthPattern = dict["GrowthPattern"].AsString();
			if (dict.ContainsKey("Sexes")) creature.Sexes = dict["Sexes"].AsString();
			if (dict.ContainsKey("Gestation")) creature.Gestation = dict["Gestation"].AsString();
			if (dict.ContainsKey("SpecialGestation")) creature.SpecialGestation = dict["SpecialGestation"].AsString();
			if (dict.ContainsKey("ReproductiveStrategy")) creature.ReproductiveStrategy = dict["ReproductiveStrategy"].AsString();
			if (dict.ContainsKey("PrimarySense")) creature.PrimarySense = dict["PrimarySense"].AsString();
			if (dict.ContainsKey("AnimalIntelligence")) creature.AnimalIntelligence = dict["AnimalIntelligence"].AsString();
			if (dict.ContainsKey("MatingBehavior")) creature.MatingBehavior = dict["MatingBehavior"].AsString();
			if (dict.ContainsKey("SocialOrganization")) creature.SocialOrganization = dict["SocialOrganization"].AsString();

			// Load sense capabilities
			if (dict.ContainsKey("SenseCapabilities"))
			{
				var senseCapabilities = dict["SenseCapabilities"].AsGodotDictionary();
				foreach (var key in senseCapabilities.Keys)
				{
					creature.SenseCapabilities[key.AsString()] = senseCapabilities[key].AsString();
				}
			}
			else
			{
				// Fallback: look for individual sense keys
				foreach (var key in dict.Keys)
				{
					if (key.AsString().StartsWith("Sense_"))
					{
						var senseName = key.AsString().Replace("Sense_", "");
						creature.SenseCapabilities[senseName] = dict[key].AsString();
					}
				}
			}

			// Load special senses
			if (dict.ContainsKey("SpecialSenses"))
			{
				var specialSenses = dict["SpecialSenses"].AsGodotArray();
				creature.SpecialSenses = specialSenses.Select(s => s.AsString()).ToList();
			}

			// Load mental traits
			if (dict.ContainsKey("MentalTraits"))
			{
				var value = dict["MentalTraits"];
				
				// Check if MentalTraits is a dictionary
				if (value.Obj is Godot.Collections.Dictionary mentalTraitsDict)
				{
					foreach (var key in mentalTraitsDict.Keys)
					{
						creature.MentalTraits[key.AsString()] = mentalTraitsDict[key].AsString();
					}
				}
				// Check if MentalTraits is a string
				else if (value.VariantType == Variant.Type.String)
				{
					string traits = value.AsString();
					var traitPairs = traits.Split(',');
					foreach (var pair in traitPairs)
					{
						var keyValue = pair.Split(':');
						if (keyValue.Length == 2)
						{
							string key = keyValue[0].Trim();
							creature.MentalTraits[key] = keyValue[1].Trim();
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error converting dictionary to creature: {e.Message}");
			GD.Print("Dictionary contents:");
			foreach (var key in dict.Keys)
			{
				GD.Print($"  {key}: {dict[key]}");
			}
		}

		return creature;
	}

	public Godot.Collections.Dictionary CreateItemMetadata(string itemType, Dictionary<string, object> data)
	{
		var dict = new Godot.Collections.Dictionary { ["ItemType"] = itemType };
		foreach (var kvp in data)
		{
			// Handle different types
			if (kvp.Value is string strValue)
			{
				dict[kvp.Key] = strValue;
			}
			else if (kvp.Value is int intValue)
			{
				dict[kvp.Key] = intValue;
			}
			else if (kvp.Value is float floatValue)
			{
				dict[kvp.Key] = floatValue;
			}
			else if (kvp.Value is bool boolValue)
			{
				dict[kvp.Key] = boolValue;
			}
			else if (kvp.Value is null)
			{
				dict[kvp.Key] = new Variant();
			}
			else
			{
				dict[kvp.Key] = kvp.Value.ToString();
			}
		}
		return dict;
	}
	
	public void SavePlanetData(Godot.Collections.Dictionary planetDict, string filePath)
	{
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		if (file == null)
		{
			GD.PrintErr($"Failed to open file for writing: {filePath}");
			return;
		}

		var jsonString = Json.Stringify(planetDict);
		file.StoreString(jsonString);
	}

	public Godot.Collections.Dictionary LoadPlanetData(string filePath)
	{
		if (!FileAccess.FileExists(filePath))
		{
			GD.PrintErr("File not found: " + filePath);
			return null;
		}

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			GD.PrintErr($"Failed to open file for reading: {filePath}");
			return null;
		}

		var jsonString = file.GetAsText();
		var json = new Json();
		var error = json.Parse(jsonString);
		
		if (error == Error.Ok)
		{
			var data = json.Data;
			if (data.Obj is Godot.Collections.Dictionary dict)
			{
				return dict;
			}
		}

		GD.PrintErr("Failed to parse JSON or convert to Dictionary");
		return null;
	}
	
	public Godot.Collections.Dictionary LoadEcosystemAsDict(string filePath)
	{
		if (!FileAccess.FileExists(filePath))
		{
			GD.PrintErr("File not found: " + filePath);
			return null;
		}

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			GD.PrintErr($"Failed to open file for reading: {filePath}");
			return null;
		}

		var jsonString = file.GetAsText();
		var json = new Json();
		var error = json.Parse(jsonString);
		
		if (error == Error.Ok)
		{
			var data = json.Data;
			if (data.Obj is Godot.Collections.Dictionary dict)
			{
				return dict;
			}
		}

		GD.PrintErr("Failed to parse JSON or convert to Dictionary");
		return null;
	}
}
