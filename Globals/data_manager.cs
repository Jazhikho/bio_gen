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

	public void SaveEcosystem(Ecosystem ecosystem, string filePath)
	{
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		if (file == null)
		{
			GD.PrintErr($"Failed to open file for writing: {filePath}");
			return;
		}

		var dict = EcosystemToDictionary(ecosystem);
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
						creature.MentalTraits[key.AsString()] = mentalTraitsDict[key].AsInt32();
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
							if (int.TryParse(keyValue[1].Trim(), out int traitValue))
							{
								creature.MentalTraits[key] = traitValue;
							}
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
}
