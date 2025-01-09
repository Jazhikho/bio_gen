using System;
using System.Collections.Generic;
using Godot;
using BioStructures;

public partial class Main : Node2D
{
	private Button generateButton;
	private Button generateSingleButton;
	private Button editButton;
	private Button saveButton;
	private Button deleteButton;
	private Button optionsButton;
	private ItemList speciesList;
	private RichTextLabel speciesDetails;
	private Window generationSettingsWindow;
	
	private SettingsManager settingsManager;
	private GenerationManager generationManager;
	private HabitatGenerator habitatGenerator;
	private PhysiologyGenerator physiologyGenerator;
	private BehaviorGenerator behaviorGenerator;
	private ReproductionGenerator reproductionGenerator;
	private NamingGenerator namingGenerator;

	 public override void _Ready()
	{
		try
		{
			InitializeManagers();
			InitializeUI();
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in Main._Ready(): {e.Message}\n{e.StackTrace}");
		}
	}

	private void InitializeManagers()
	{
		// Initialize SettingsManager first
		settingsManager = new SettingsManager();
		AddChild(settingsManager);
		
		// Give it a frame to set up its Instance property
		CallDeferred(nameof(InitializeOtherManagers));
	}

	private void InitializeOtherManagers()
	{
		if (SettingsManager.Instance == null)
		{
			GD.PrintErr("SettingsManager.Instance is still null after initialization!");
			return;
		}

		generationManager = new GenerationManager();
		AddChild(generationManager);

		habitatGenerator = new HabitatGenerator();
		AddChild(habitatGenerator);

		physiologyGenerator = new PhysiologyGenerator();
		AddChild(physiologyGenerator);

		behaviorGenerator = new BehaviorGenerator();
		AddChild(behaviorGenerator);

		reproductionGenerator = new ReproductionGenerator();
		AddChild(reproductionGenerator);

		namingGenerator = new NamingGenerator();
		AddChild(namingGenerator);

		// Now that all managers are initialized, initialize UI
		CallDeferred(nameof(InitializeUI));
	}

	private void InitializeUI()
	{
		try
		{
			// Disconnect existing signals first
			if (generateButton != null)
			{
				generateButton.Pressed -= OnGenerateButtonPressed;
			}
			if (generateSingleButton != null)
			{
				generateSingleButton.Pressed -= OnGenerateSingleButtonPressed;
			}
			if (speciesList != null)
			{
				speciesList.ItemSelected -= OnSpeciesSelected;
			}

			// Initialize buttons
			generateButton = GetNodeOrNull<Button>("GUI/MenuPanel/ButtonContainer/GenerateButton");
			generateSingleButton = GetNodeOrNull<Button>("GUI/MenuPanel/ButtonContainer/GenerateSingleButton");
			
			// Initialize content panels
			speciesList = GetNodeOrNull<ItemList>("GUI/ContentPanel/MarginContainer/VSplitContainer/SpeciesList");
			speciesDetails = GetNodeOrNull<RichTextLabel>("GUI/ContentPanel/MarginContainer/VSplitContainer/SpeciesDetails");

			// Connect signals if nodes exist
			if (generateButton != null)
				generateButton.Pressed += OnGenerateButtonPressed;
			if (generateSingleButton != null)
				generateSingleButton.Pressed += OnGenerateSingleButtonPressed;
			if (speciesList != null)
				speciesList.ItemSelected += OnSpeciesSelected;

			// Load generation settings window
			if (generationSettingsWindow == null)
			{
				var generationSettingsScene = GD.Load<PackedScene>("res://GUI/SettingsWindow/generation_settings_window.tscn");
				if (generationSettingsScene != null)
				{
					generationSettingsWindow = generationSettingsScene.Instantiate<Window>();
					AddChild(generationSettingsWindow);
					generationSettingsWindow.Hide();
				}
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in InitializeUI: {e.Message}\n{e.StackTrace}");
		}
	}

	private void EnsureManager<T>(string nodeName) where T : Node, new()
	{
		if (!HasNode(nodeName))
		{
			T manager = new T();
			manager.Name = nodeName;
			AddChild(manager);
		}
	}

	private void OnGenerateButtonPressed()
	{
		generationSettingsWindow.Show();
	}
	
	private void OnGenerateSingleButtonPressed()
	{
		try
		{
			GD.Print("Generate Single Button Pressed");
			
			if (GenerationManager.Instance == null)
			{
				GD.PrintErr("GenerationManager.Instance is null!");
				return;
			}
			
			var creature = GenerationManager.Instance.GenerateSingleSpecies();
			if (creature == null)
			{
				GD.PrintErr("Generated creature is null!");
				return;
			}
			
			GD.Print($"Generated creature: {creature.Name}");
			
			AddCreatureToList(creature);
			DisplayCreatureDetails(creature);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in OnGenerateSingleButtonPressed: {e.Message}\n{e.StackTrace}");
		}
	}
	
	private void AddCreatureToList(Creature creature)
	{
		if (speciesList == null)
		{
			GD.PrintErr("SpeciesList is null!");
			return;
		}
		
		int previousCount = speciesList.ItemCount;
		speciesList.AddItem($"{creature.Name} ({creature.Habitat})");
		int newCount = speciesList.ItemCount;
		GD.Print($"Added creature to list. Items before: {previousCount}, after: {newCount}");
		
		int index = speciesList.ItemCount - 1;
		
		// Convert creature to a Godot Dictionary
		var creatureDict = new Godot.Collections.Dictionary
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
		
		speciesList.SetItemMetadata(index, creatureDict);
	}

	private void OnEditButtonPressed()
	{
		// Implement edit functionality
	}

	private void OnSaveButtonPressed()
	{
		// Implement save functionality
	}

	private void OnDeleteButtonPressed()
	{
		// Implement delete functionality
	}

	private void OnOptionsButtonPressed()
	{
		// Implement options functionality
	}

	private void OnSpeciesSelected(long index)
	{
		var metadata = speciesList.GetItemMetadata((int)index);
		if (metadata.Obj is Godot.Collections.Dictionary dict)
		{
			var creature = DictionaryToCreature(dict);
			DisplayCreatureDetails(creature);
		}
	}

	private void DisplayCreatureDetails(Creature creature)
	{
		if (speciesDetails == null)
		{
			GD.PrintErr("SpeciesDetails is null!");
			return;
		}
		
		string details = $"[b]{creature.Name}[/b]\n\n";
		
		// Basic Information
		details += "[u]Basic Information[/u]\n";
		details += $"Chemical Basis: {creature.ChemicalBasis}\n";
		details += $"Habitat: {creature.Habitat}\n";
		details += $"Trophic Level: {creature.TrophicLevel}\n";
		details += $"Size: {creature.SizeCategory} (x{creature.GravitySizeMultiplier:F2} gravity modifier)\n\n";
		
		// Physical Characteristics
		details += "[u]Physical Characteristics[/u]\n";
		details += $"Symmetry: {creature.Symmetry}\n";
		details += $"Number of Limbs: {creature.NumberOfLimbs}\n";
		details += $"Manipulators: {creature.NumberOfManipulators}\n";
		details += $"Locomotion: {creature.Locomotion}\n";
		details += $"Skeleton Type: {creature.Skeleton}\n";
		details += $"Skin: {creature.Skin}\n";
		if (creature.TailFeatures != "None")
		{
			details += $"Tail Features: {creature.TailFeatures}\n";
		}
		details += "\n";
		
		// Senses
		details += "[u]Senses[/u]\n";
		details += $"{creature.Senses}\n\n";
		
		// Reproduction
		details += "[u]Reproduction[/u]\n";
		details += $"Growth Pattern: {creature.GrowthPattern}\n";
		details += $"Sexes: {creature.Sexes}\n";
		details += $"Gestation: {creature.Gestation}\n";
		if (!string.IsNullOrEmpty(creature.SpecialGestation))
		{
			details += $"Special Gestation: {creature.SpecialGestation}\n";
		}
		details += $"Reproductive Strategy: {creature.ReproductiveStrategy}\n\n";
		
		// Intelligence and Behavior
		details += "[u]Intelligence and Behavior[/u]\n";
		details += $"Intelligence Level: {creature.AnimalIntelligence}\n";
		details += "Mental Qualities:\n";
		
		// Split mental qualities into individual traits
		var qualities = creature.MentalQualities.Split(',');
		foreach (var quality in qualities)
		{
			details += $"  • {quality.Trim()}\n";
		}

		speciesDetails.Text = details;
		GD.Print("Updated species details");
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
