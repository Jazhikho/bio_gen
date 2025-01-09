using Godot;
using System;
using BioStructures;

public partial class settings_manager : Node
{
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

	private Godot.Collections.Dictionary EcosystemToDictionary(Ecosystem ecosystem)
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

	private Godot.Collections.Dictionary CreatureToDictionary(Creature creature)
	{
		return new Godot.Collections.Dictionary
		{
			["Name"] = creature.Name,
			["ChemicalBasis"] = creature.ChemicalBasis,
			["Habitat"] = creature.Habitat,
			["TrophicLevel"] = creature.TrophicLevel,
			["Locomotion"] = creature.Locomotion,
			["SizeCategory"] = creature.SizeCategory,
			["GravitySizeMultiplier"] = creature.GravitySizeMultiplier,
			["Symmetry"] = creature.Symmetry,
			["NumberOfLimbs"] = creature.NumberOfLimbs,
			["TailFeatures"] = creature.TailFeatures,
			["NumberOfManipulators"] = creature.NumberOfManipulators,
			["Skeleton"] = creature.Skeleton,
			["GrowthPattern"] = creature.GrowthPattern,
			["Sexes"] = creature.Sexes,
			["Gestation"] = creature.Gestation,
			["SpecialGestation"] = creature.SpecialGestation,
			["ReproductiveStrategy"] = creature.ReproductiveStrategy,
			["Skin"] = creature.Skin,
			["Senses"] = creature.Senses,
			["AnimalIntelligence"] = creature.AnimalIntelligence,
			["MentalQualities"] = creature.MentalQualities
		};
	}

	private Ecosystem DictionaryToEcosystem(Godot.Collections.Dictionary dict)
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

	private Creature DictionaryToCreature(Godot.Collections.Dictionary dict)
	{
		return new Creature
		{
			Name = dict["Name"].AsString(),
			ChemicalBasis = dict["ChemicalBasis"].AsString(),
			Habitat = dict["Habitat"].AsString(),
			TrophicLevel = dict["TrophicLevel"].AsString(),
			Locomotion = dict["Locomotion"].AsString(),
			SizeCategory = dict["SizeCategory"].AsString(),
			GravitySizeMultiplier = dict["GravitySizeMultiplier"].AsSingle(),
			Symmetry = dict["Symmetry"].AsString(),
			NumberOfLimbs = dict["NumberOfLimbs"].AsInt32(),
			TailFeatures = dict["TailFeatures"].AsString(),
			NumberOfManipulators = dict["NumberOfManipulators"].AsInt32(),
			Skeleton = dict["Skeleton"].AsString(),
			GrowthPattern = dict["GrowthPattern"].AsString(),
			Sexes = dict["Sexes"].AsString(),
			Gestation = dict["Gestation"].AsString(),
			SpecialGestation = dict["SpecialGestation"].AsString(),
			ReproductiveStrategy = dict["ReproductiveStrategy"].AsString(),
			Skin = dict["Skin"].AsString(),
			Senses = dict["Senses"].AsString(),
			AnimalIntelligence = dict["AnimalIntelligence"].AsString(),
			MentalQualities = dict["MentalQualities"].AsString()
		};
	}
}
