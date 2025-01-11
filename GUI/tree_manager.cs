using Godot;
using BioStructures;

public partial class TreeManager : Node
{
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
					GD.Print("Selected Creature Dictionary Contents:");
					foreach (var key in dict.Keys)
					{
						GD.Print($"  {key}: {dict[key]}");
					}
					displayManager.DisplayCreatureDetails(DataManager.Instance.DictionaryToCreature(dict));;
					uiManager.SetButtonStates(true);
					currentSelectedEcosystem = null;
					currentSelectedBiomeItem = null;
					break;
				case "Biome":
					displayManager.DisplayBiomeDetails(dict);
					uiManager.SetButtonStates(false);
					currentSelectedEcosystem = GetEcosystemFromBiomeItem(selected);
					currentSelectedBiomeItem = selected;
					break;
				case "LandMass":
				case "WaterBody":
					displayManager.DisplayDetails(dict);
					uiManager.SetButtonStates(true);
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
}
