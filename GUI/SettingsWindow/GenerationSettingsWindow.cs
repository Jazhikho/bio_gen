using Godot;
using System;

public partial class GenerationSettingsWindow : Window
{
	private OptionButton planetTypeOption;
	private SpinBox temperatureSpinBox;
	private SpinBox hydrologySpinBox;
	private SpinBox gravitySpinBox;
	private SpinBox landMassesSpinBox;
	private OptionButton chemistryOption;

	public override void _Ready()
	{
		// Get references to controls
		planetTypeOption = GetNode<OptionButton>("MarginContainer/VBoxContainer/PlanetTypeHBox/PlanetTypeOption");
		temperatureSpinBox = GetNode<SpinBox>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/TemperatureSpinBox");
		hydrologySpinBox = GetNode<SpinBox>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/HydrologySpinBox");
		gravitySpinBox = GetNode<SpinBox>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/GravitySpinBox");
		landMassesSpinBox = GetNode<SpinBox>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/LandMassesSpinBox");
		chemistryOption = GetNode<OptionButton>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/ChemistryOption");

		// Setup planet types
		foreach (SettingsManager.PlanetType type in Enum.GetValues(typeof(SettingsManager.PlanetType)))
		{
			planetTypeOption.AddItem(type.ToString());
		}

		// Setup chemistry types
		foreach (SettingsManager.PlanetSettings.ChemistryBasis chemistry in 
				Enum.GetValues(typeof(SettingsManager.PlanetSettings.ChemistryBasis)))
		{
			chemistryOption.AddItem(chemistry.ToString());
		}

		// Connect signals
		GetNode<Button>("MarginContainer/VBoxContainer/ButtonContainer/RandomizeButton").Pressed += OnRandomizePressed;
		GetNode<Button>("MarginContainer/VBoxContainer/ButtonContainer/GenerateButton").Pressed += OnGeneratePressed;
		GetNode<Button>("MarginContainer/VBoxContainer/ButtonContainer/CancelButton").Pressed += OnCancelPressed;
		planetTypeOption.ItemSelected += OnPlanetTypeSelected;

		// Set initial values
		UpdateUIFromSettings(SettingsManager.Instance.CurrentSettings);
	}

	private void UpdateUIFromSettings(SettingsManager.PlanetSettings settings)
	{
		temperatureSpinBox.Value = settings.Temperature;
		hydrologySpinBox.Value = settings.Hydrology;
		gravitySpinBox.Value = settings.Gravity;
		landMassesSpinBox.Value = settings.LandMasses;
		chemistryOption.Selected = (int)settings.PrimaryChemistry;
	}

	private void OnPlanetTypeSelected(long index)
	{
		SettingsManager.Instance.SetPresetSettings((SettingsManager.PlanetType)index);
		UpdateUIFromSettings(SettingsManager.Instance.CurrentSettings);
	}

	private void OnRandomizePressed()
	{
		SettingsManager.Instance.RandomizeSettings();
		UpdateUIFromSettings(SettingsManager.Instance.CurrentSettings);
	}

	private void OnGeneratePressed()
	{
		// Update settings from UI
		var settings = SettingsManager.Instance.CurrentSettings;
		settings.Temperature = (float)temperatureSpinBox.Value;
		settings.Hydrology = (float)hydrologySpinBox.Value;
		settings.Gravity = (float)gravitySpinBox.Value;
		settings.LandMasses = (int)landMassesSpinBox.Value;
		settings.PrimaryChemistry = (SettingsManager.PlanetSettings.ChemistryBasis)chemistryOption.Selected;

		// Emit signal or call method to generate with these settings
		Hide();
	}

	private void OnCancelPressed()
	{
		Hide();
	}
}
