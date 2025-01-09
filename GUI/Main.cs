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

	public override void _Ready()
	{
		// Get references to UI elements
		generateButton = GetNode<Button>("GUI/MenuPanel/ButtonContainer/GenerateButton");
		generateSingleButton = GetNode<Button>("GUI/MenuPanel/ButtonContainer/GenerateSingleButton");
		editButton = GetNode<Button>("GUI/MenuPanel/ButtonContainer/EditButton");
		saveButton = GetNode<Button>("GUI/MenuPanel/ButtonContainer/SaveButton");
		deleteButton = GetNode<Button>("GUI/MenuPanel/ButtonContainer/DeleteButton");
		optionsButton = GetNode<Button>("GUI/MenuPanel/ButtonContainer/OptionsButton");
		speciesList = GetNode<ItemList>("GUI/ContentPanel/MarginContainer/VSplitContainer/SpeciesList");
		speciesDetails = GetNode<RichTextLabel>("GUI/ContentPanel/MarginContainer/VSplitContainer/SpeciesDetails");

		// Load the generation settings window scene
		var generationSettingsScene = GD.Load<PackedScene>("res://GUI/SettingsWindow/generation_settings_window.tscn");
		generationSettingsWindow = generationSettingsScene.Instantiate<Window>();
		AddChild(generationSettingsWindow);
		generationSettingsWindow.Hide();

		// Connect signals
		generateButton.Pressed += OnGenerateButtonPressed;
		generateSingleButton.Pressed += OnGenerateSingleButtonPressed;
		editButton.Pressed += OnEditButtonPressed;
		saveButton.Pressed += OnSaveButtonPressed;
		deleteButton.Pressed += OnDeleteButtonPressed;
		optionsButton.Pressed += OnOptionsButtonPressed;
		speciesList.ItemSelected += OnSpeciesSelected;

		// Ensure the settings and input managers are in the scene
		EnsureManager<SettingsManager>("SettingsManager");
		EnsureManager<InputManager>("InputManager");
		
		// Configure the content panel
		var contentPanel = GetNode<Panel>("GUI/ContentPanel");
		var marginContainer = GetNode<MarginContainer>("GUI/ContentPanel/MarginContainer");
		var vSplitContainer = GetNode<VSplitContainer>("GUI/ContentPanel/MarginContainer/VSplitContainer");
		
		// Configure the species list
		speciesList = GetNode<ItemList>("GUI/ContentPanel/MarginContainer/VSplitContainer/SpeciesList");
		speciesList.ItemSelected += OnSpeciesSelected;
		
		// Configure the species details
		speciesDetails = GetNode<RichTextLabel>("GUI/ContentPanel/MarginContainer/VSplitContainer/SpeciesDetails");
		speciesDetails.BbcodeEnabled = true;
		
		// Set the split offset (adjust as needed)
		vSplitContainer.SplitOffset = 200;
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
		var creature = GenerationManager.Instance.GenerateSingleSpecies();
		AddCreatureToList(creature);
		DisplayCreatureDetails(creature);
	}
	
	private void AddCreatureToList(Creature creature)
	{
		speciesList.AddItem(creature.Name);
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
