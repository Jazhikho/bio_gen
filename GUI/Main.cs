using Godot;
using System;
using BioStructures;

public partial class Main : Node2D
{
	// Managers
	private UIManager uiManager;
	private DisplayManager displayManager;
	private TreeManager treeManager;
	private DataManager dataManager;
	private SaveLoadManager saveLoadManager;
	private FileDialogManager fileDialogManager;
	private OptionsManager optionsManager;

	// Other managers (already existing)
	private SettingsManager settingsManager;
	private GenerationManager generationManager;
	private HabitatGenerator habitatGenerator;
	private TrophicGenerator trophicGenerator;
	private LocomotionGenerator locomotionGenerator;
	private SizeGenerator sizeGenerator;
	private SensesGenerator sensesGenerator;
	private PhysiologyGenerator physiologyGenerator;
	private BehaviorGenerator behaviorGenerator;
	private ReproductionGenerator reproductionGenerator;
	private NamingGenerator namingGenerator;
	private PlanetGenerator planetGenerator;

	// UI References
	private Button generateButton;
	private Button generateSingleButton;
	private Button editButton;
	private Button saveButton;
	private Button deleteButton;
	private Button optionsButton;
	private Button openFileButton;
	private Tree ecosystemTree;
	private RichTextLabel speciesDetails;
	private GenerationSettingsWindow generationSettingsWindow;
	private Window editPopup;
	private ConfirmationDialog confirmationDialog;

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
		settingsManager = new SettingsManager();
		dataManager = new DataManager();
		uiManager = new UIManager();
		displayManager = new DisplayManager();
		treeManager = new TreeManager();
		saveLoadManager = new SaveLoadManager();
		fileDialogManager = new FileDialogManager();
		optionsManager = new OptionsManager();

		AddChild(settingsManager);
		AddChild(dataManager);
		AddChild(uiManager);
		AddChild(displayManager);
		AddChild(treeManager);
		AddChild(saveLoadManager);
		AddChild(fileDialogManager);
		AddChild(optionsManager);
		
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
		habitatGenerator = new HabitatGenerator();
		trophicGenerator = new TrophicGenerator();
		locomotionGenerator = new LocomotionGenerator();
		sizeGenerator = new SizeGenerator();
		physiologyGenerator = new PhysiologyGenerator();
		reproductionGenerator = new ReproductionGenerator();
		sensesGenerator = new SensesGenerator();
		behaviorGenerator = new BehaviorGenerator();
		namingGenerator = new NamingGenerator();
		planetGenerator = new PlanetGenerator();
		
		AddChild(generationManager);
		AddChild(habitatGenerator);
		AddChild(trophicGenerator);
		AddChild(locomotionGenerator);
		AddChild(sizeGenerator);
		AddChild(physiologyGenerator);
		AddChild(reproductionGenerator);
		AddChild(sensesGenerator);
		AddChild(behaviorGenerator);
		AddChild(namingGenerator);
		AddChild(planetGenerator);

		CallDeferred(nameof(InitializeUI));
	}

	private void InitializeUI()
	{
		try
		{
			// Get UI references
		var buttonContainer = GetNode<Control>("GUI/MenuPanel/ButtonContainer");
		generateButton = buttonContainer.GetNode<Button>("GenerateButton");
		generateSingleButton = buttonContainer.GetNode<Button>("GenerateSingleButton");
		editButton = buttonContainer.GetNode<Button>("EditButton");
		saveButton = buttonContainer.GetNode<Button>("SaveButton");
		deleteButton = buttonContainer.GetNode<Button>("DeleteButton");
		openFileButton = buttonContainer.GetNode<Button>("OpenFileButton");
		optionsButton = buttonContainer.GetNode<Button>("OptionsButton");

		var splitContainer = GetNode<Container>("GUI/ContentPanel/MarginContainer/VSplitContainer");
		ecosystemTree = splitContainer.GetNode<Tree>("EcosystemTree");
		speciesDetails = splitContainer.GetNode<RichTextLabel>("SpeciesDetails");

		editPopup = GetNode<Window>("EditPopup");
		confirmationDialog = GetNode<ConfirmationDialog>("ConfirmationDialog");

			// Pass UI references to managers
			uiManager.Initialize(generateButton, generateSingleButton, editButton, saveButton, deleteButton, openFileButton, optionsButton);
			displayManager.Initialize(speciesDetails);
			treeManager.Initialize(ecosystemTree);
			fileDialogManager.Initialize(GetNode<FileDialog>("SaveDialog"), GetNode<FileDialog>("OpenDialog"));
			optionsManager.Initialize(GetNode<Window>("OptionsWindow"));

			// Connect signals
			ConnectSignals();

			// Initialize generation settings window
			InitializeGenerationSettingsWindow();
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in InitializeUI: {e.Message}\n{e.StackTrace}");
		}
	}

	private void ConnectSignals()
	{
		generateButton.Pressed += OnGenerateButtonPressed;
		generateSingleButton.Pressed += OnGenerateSingleButtonPressed;
		editButton.Pressed += OnEditButtonPressed;
		saveButton.Pressed += OnSaveButtonPressed;
		deleteButton.Pressed += OnDeleteButtonPressed;
		openFileButton.Pressed += OnOpenFileButtonPressed;
		optionsButton.Pressed += OnOptionsButtonPressed;
		
		ecosystemTree.ItemSelected += OnEcosystemItemSelected;
		
		confirmationDialog.Confirmed += OnDeleteConfirmed;
	}

	private void InitializeGenerationSettingsWindow()
	{
		generationSettingsWindow = GetNode<GenerationSettingsWindow>("GenerationSettingsWindow");
		generationSettingsWindow.SettingsConfirmed += OnSettingsConfirmed;
	}

	private void OnGenerateButtonPressed()
	{
		generationSettingsWindow.Show();
	}

	private void OnSettingsConfirmed()
	{
		try
		{
			var planet = PlanetGenerator.Instance.GeneratePlanet(SettingsManager.Instance.CurrentSettings);
			if (planet == null)
			{
				GD.PrintErr("Generated planet is null!");
				return;
			}

			treeManager.DisplayPlanetData(planet, displayManager);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in OnSettingsConfirmed: {e.Message}\n{e.StackTrace}");
		}
	}

	private void OnGenerateSingleButtonPressed()
	{
		try
		{
			var selectedEcosystem = treeManager.GetCurrentSelectedEcosystem();
			var selectedBiomeItem = treeManager.GetCurrentSelectedBiomeItem();

			if (selectedEcosystem == null || selectedBiomeItem == null)
			{
				GD.PrintErr("No biome selected for new species generation!");
				return;
			}
			
			var creature = GenerationManager.Instance.GenerateSingleSpecies(
				SettingsManager.Instance.CurrentSettings, 
				selectedEcosystem);

			if (creature == null)
			{
				GD.PrintErr("Generated creature is null!");
				return;
			}

			treeManager.AddCreatureToTree(creature, selectedBiomeItem);
			displayManager.DisplayCreatureDetails(creature);
			treeManager.UpdateParentCounts(selectedBiomeItem);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in OnGenerateSingleButtonPressed: {e.Message}\n{e.StackTrace}");
		}
	}

	private void OnEditButtonPressed()
	{
		editPopup.Show();
	}

	private void OnSaveButtonPressed()
	{
		fileDialogManager.ShowSaveDialog();
	}

	private void OnDeleteButtonPressed()
	{
		confirmationDialog.Show();
	}

	private void OnOpenFileButtonPressed()
	{
		fileDialogManager.ShowOpenDialog();
	}

	private void OnOptionsButtonPressed()
	{
		optionsManager.ShowOptions();
	}

	private void OnEcosystemItemSelected()
	{
		treeManager.OnEcosystemItemSelected(displayManager, uiManager);
	}

	private void OnDeleteConfirmed()
	{
		treeManager.DeleteSelectedItem();
	}
}
