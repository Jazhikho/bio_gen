using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BioStructures;

public partial class TreeManager : Node
{
	public static TreeManager Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance == null) Instance = this;
		else QueueFree();
	}
	
	private Tree ecosystemTree;
	private TreeItem currentSelectedBiomeItem;
	private Ecosystem currentSelectedEcosystem;

	public void Initialize(Tree tree)
	{
		ecosystemTree = tree;
	}

	public void DisplayPlanetData(Planet planet, DisplayManager displayManager)
	{
		ecosystemTree.Clear();
		var root = ecosystemTree.CreateItem();
		root.SetText(0, planet.Name);

		var planetDict = new Godot.Collections.Dictionary
		{
			["ItemType"] = "Planet",
			["Name"] = planet.Name,
			["Temperature"] = planet.Settings.Temperature,
			["Hydrology"] = planet.Settings.Hydrology,
			["Gravity"] = planet.Settings.Gravity,
			["LandMasses"] = planet.LandMasses.Count,
			["WaterBodies"] = planet.WaterBodies.Count,
			["TotalEcosystems"] = planet.GetTotalEcosystemCount()
		};
		root.SetMetadata(0, planetDict);

		PlanetGenerator.Instance.PopulateEcosystemTree(ecosystemTree, root, planet);
		
		displayManager.DisplayPlanetDetails(planetDict);
	}

	public void OnEcosystemItemSelected(DisplayManager displayManager, UIManager uiManager)
	{
		var selected = ecosystemTree.GetSelected();
		if (selected == null) return;

		if (selected == ecosystemTree.GetRoot())
		{
			var rootMetadata = selected.GetMetadata(0).AsGodotDictionary();
			displayManager.DisplayPlanetDetails(rootMetadata);
			uiManager.SetButtonStates(true);
			currentSelectedEcosystem = null;
			currentSelectedBiomeItem = null;
			return;
		}

		var metadata = selected.GetMetadata(0);
		if (metadata.Obj is Godot.Collections.Dictionary dict)
		{
			switch (dict["ItemType"].AsString())
			{
				case "Creature":
					displayManager.DisplayCreatureDetails(DataManager.Instance.DictionaryToCreature(dict));;
					uiManager.generateSingleButton.Disabled = false;
					uiManager.editButton.Disabled = false;
					uiManager.deleteButton.Disabled = false;
					currentSelectedEcosystem = null;
					currentSelectedBiomeItem = null;
					break;
				case "Biome":
					displayManager.DisplayBiomeDetails(dict);
					uiManager.generateSingleButton.Disabled = false;
					uiManager.editButton.Disabled = false;
					uiManager.deleteButton.Disabled = true;
					currentSelectedEcosystem = GetEcosystemFromBiomeItem(selected);
					currentSelectedBiomeItem = selected;
					break;
				case "LandMass":
				case "WaterBody":
					displayManager.DisplayDetails(dict);
					uiManager.generateSingleButton.Disabled = false;
					uiManager.editButton.Disabled = false;
					uiManager.deleteButton.Disabled = true;
					currentSelectedEcosystem = null;
					currentSelectedBiomeItem = null;
					break;
			}
		}
	}

	public void AddCreatureToTree(Creature creature, TreeItem parent)
	{
		var item = parent != null ? ecosystemTree.CreateItem(parent) : ecosystemTree.CreateItem();
		item.SetText(0, $"{creature.Name} ({creature.Habitat})");
		
		var creatureDict = DataManager.Instance.CreatureToDictionary(creature);
		item.SetMetadata(0, creatureDict);
	}

	public void UpdateParentCounts(TreeItem biomeItem)
	{
		var parent = biomeItem.GetParent();
		if (parent == null || parent == ecosystemTree.GetRoot()) return;

		var parentMetadata = parent.GetMetadata(0).AsGodotDictionary();
		if (parentMetadata.ContainsKey("TotalSpecies"))
		{
			parentMetadata["TotalSpecies"] = parentMetadata["TotalSpecies"].AsInt32() + 1;
			parent.SetMetadata(0, parentMetadata);
		}
	}

	public Ecosystem GetCurrentSelectedEcosystem()
	{
		return currentSelectedEcosystem;
	}

	public TreeItem GetCurrentSelectedBiomeItem()
	{
		return currentSelectedBiomeItem;
	}

	private Ecosystem GetEcosystemFromBiomeItem(TreeItem biomeItem)
	{
		var biomeMetadata = biomeItem.GetMetadata(0).AsGodotDictionary();
		return new Ecosystem
		{
			HabitatType = biomeMetadata["Type"].AsString(),
			EcosystemID = biomeMetadata.ContainsKey("EcosystemID") ? biomeMetadata["EcosystemID"].AsInt32() : Roll.Dice(3, 6),
			LocationID = biomeMetadata.ContainsKey("LocationID") ? biomeMetadata["LocationID"].AsInt32() : Roll.Dice(3, 6),
			Creatures = new Creature[] { }
		};
	}
	
	public void DeleteSelectedItem()
	{
		var selected = ecosystemTree.GetSelected();
		if (selected == null || selected == ecosystemTree.GetRoot()) return;

		// Update parent counts if necessary
		var parent = selected.GetParent();
		if (parent != null && parent != ecosystemTree.GetRoot())
		{
			var parentMetadata = parent.GetMetadata(0).AsGodotDictionary();
			if (parentMetadata.ContainsKey("TotalSpecies"))
			{
				parentMetadata["TotalSpecies"] = parentMetadata["TotalSpecies"].AsInt32() - 1;
				parent.SetMetadata(0, parentMetadata);
			}
		}

		// Remove the item
		selected.Free();
	}
	
	public Godot.Collections.Dictionary GetCurrentPlanetData()
	{
		var planetDict = new Godot.Collections.Dictionary();
		var root = ecosystemTree.GetRoot();
		if (root == null) return null;

		// Get planet info
		planetDict["Name"] = root.GetText(0);
		
		// Create arrays for land masses and water bodies
		var landMasses = new Godot.Collections.Array();
		var waterBodies = new Godot.Collections.Array();

		var child = root.GetFirstChild();
		while (child != null)
		{
			var metadata = child.GetMetadata(0).AsGodotDictionary();
			if (metadata["ItemType"].AsString() == "LandMass")
			{
				landMasses.Add(ConvertLandMassToDict(child));
			}
			else if (metadata["ItemType"].AsString() == "WaterBody")
			{
				waterBodies.Add(ConvertWaterBodyToDict(child));
			}
			child = child.GetNext();
		}

		planetDict["LandMasses"] = landMasses;
		planetDict["WaterBodies"] = waterBodies;

		return planetDict;
	}
	
	private Variant GetSafeMetadata(TreeItem item)
	{
		var metadata = item.GetMetadata(0);
		return metadata.VariantType != Variant.Type.Nil ? metadata : new Godot.Collections.Dictionary();
	}

	public Planet GetCurrentPlanet()
	{
		var root = ecosystemTree.GetRoot();
		if (root == null) return null;

		var planet = new Planet();
		var rootMeta = GetSafeMetadata(root).AsGodotDictionary();
		
		planet.Name = root.GetText(0);
		
		// Create new settings
		var settings = new SettingsManager.PlanetSettings();
		if (rootMeta.Count > 0)
		{
			if (rootMeta.ContainsKey("Temperature")) settings.Temperature = rootMeta["Temperature"].AsSingle();
			if (rootMeta.ContainsKey("Hydrology")) settings.Hydrology = rootMeta["Hydrology"].AsSingle();
			if (rootMeta.ContainsKey("Gravity")) settings.Gravity = rootMeta["Gravity"].AsSingle();
		}
		planet.Settings = settings;

		// Get land masses and water bodies
		planet.LandMasses = new List<LandMass>();
		planet.WaterBodies = new List<WaterBody>();

		var child = root.GetFirstChild();
		while (child != null)
		{
			var metadata = GetSafeMetadata(child).AsGodotDictionary();
			if (metadata.Count > 0 && metadata.ContainsKey("ItemType"))
			{
				if (metadata["ItemType"].AsString() == "LandMass")
				{
					planet.LandMasses.Add(ConvertTreeItemToLandMass(child));
				}
				else if (metadata["ItemType"].AsString() == "WaterBody")
				{
					planet.WaterBodies.Add(ConvertTreeItemToWaterBody(child));
				}
			}
			child = child.GetNext();
		}

		return planet;
	}

	private LandMass ConvertTreeItemToLandMass(TreeItem item)
	{
		var landMass = new LandMass();
		var metadata = GetSafeMetadata(item).AsGodotDictionary();
		if (metadata.Count > 0 && metadata.ContainsKey("Name"))
		{
			landMass.Name = metadata["Name"].AsString();
		}
		landMass.Biomes = GetBiomesFromTreeItem(item);
		return landMass;
	}

	private WaterBody ConvertTreeItemToWaterBody(TreeItem item)
	{
		var waterBody = new WaterBody();
		var metadata = GetSafeMetadata(item).AsGodotDictionary();
		if (metadata.Count > 0)
		{
			if (metadata.ContainsKey("Name")) waterBody.Name = metadata["Name"].AsString();
			if (metadata.ContainsKey("WaterType")) waterBody.WaterType = metadata["WaterType"].AsString();
		}
		waterBody.Biomes = GetBiomesFromTreeItem(item);
		return waterBody;
	}

	private List<Ecosystem> GetBiomesFromTreeItem(TreeItem parentItem)
	{
		var biomes = new List<Ecosystem>();
		var child = parentItem.GetFirstChild();
		while (child != null)
		{
			var metadata = GetSafeMetadata(child).AsGodotDictionary();
			if (metadata.Count > 0 && metadata.ContainsKey("ItemType") && metadata["ItemType"].AsString() == "Biome")
			{
				var ecosystem = new Ecosystem();
				if (metadata.ContainsKey("Type")) ecosystem.HabitatType = metadata["Type"].AsString();
				if (metadata.ContainsKey("EcosystemID")) ecosystem.EcosystemID = metadata["EcosystemID"].AsInt32();
				if (metadata.ContainsKey("LocationID")) ecosystem.LocationID = metadata["LocationID"].AsInt32();
				ecosystem.Creatures = GetCreaturesFromBiomeItem(child);
				biomes.Add(ecosystem);
			}
			child = child.GetNext();
		}
		return biomes;
	}

	private Creature[] GetCreaturesFromBiomeItem(TreeItem biomeItem)
	{
		var creatures = new List<Creature>();
		var child = biomeItem.GetFirstChild();
		while (child != null)
		{
			var metadata = GetSafeMetadata(child).AsGodotDictionary();
			if (metadata.Count > 0 && metadata.ContainsKey("ItemType") && metadata["ItemType"].AsString() == "Creature")
			{
				creatures.Add(DataManager.Instance.DictionaryToCreature(metadata));
			}
			child = child.GetNext();
		}
		return creatures.ToArray();
	}
	
	private Godot.Collections.Dictionary ConvertLandMassToDict(TreeItem landMassItem)
	{
		var dict = new Godot.Collections.Dictionary();
		var metadata = landMassItem.GetMetadata(0).AsGodotDictionary();
		dict["Name"] = metadata["Name"];
		dict["ItemType"] = "LandMass";
		
		var biomes = new Godot.Collections.Array();
		var child = landMassItem.GetFirstChild();
		while (child != null)
		{
			biomes.Add(ConvertBiomeToDict(child));
			child = child.GetNext();
		}
		dict["Biomes"] = biomes;
		
		return dict;
	}

	private Godot.Collections.Dictionary ConvertWaterBodyToDict(TreeItem waterBodyItem)
	{
		var dict = new Godot.Collections.Dictionary();
		var metadata = waterBodyItem.GetMetadata(0).AsGodotDictionary();
		dict["Name"] = metadata["Name"];
		dict["ItemType"] = "WaterBody";
		dict["WaterType"] = metadata["WaterType"];
		
		var biomes = new Godot.Collections.Array();
		var child = waterBodyItem.GetFirstChild();
		while (child != null)
		{
			biomes.Add(ConvertBiomeToDict(child));
			child = child.GetNext();
		}
		dict["Biomes"] = biomes;
		
		return dict;
	}

	private Godot.Collections.Dictionary ConvertBiomeToDict(TreeItem biomeItem)
	{
		var dict = new Godot.Collections.Dictionary();
		var metadata = biomeItem.GetMetadata(0).AsGodotDictionary();
		dict["ItemType"] = "Biome";
		dict["Type"] = metadata["Type"];
		dict["EcosystemID"] = metadata["EcosystemID"];
		dict["LocationID"] = metadata["LocationID"];
		
		var creatures = new Godot.Collections.Array();
		var child = biomeItem.GetFirstChild();
		while (child != null)
		{
			var creatureMetadata = child.GetMetadata(0).AsGodotDictionary();
			if (creatureMetadata["ItemType"].AsString() == "Creature")
			{
				creatures.Add(creatureMetadata);
			}
			child = child.GetNext();
		}
		dict["Creatures"] = creatures;
		
		return dict;
	}
}
