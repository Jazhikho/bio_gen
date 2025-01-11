using Godot;
using System;

public partial class GenerationSettingsWindow : Window
{
	[Signal]
	public delegate void SettingsConfirmedEventHandler();

	private OptionButton planetTypeOption;
	private SpinBox temperatureSpinBox;
	private SpinBox hydrologySpinBox;
	private SpinBox gravitySpinBox;
	private SpinBox landMassesSpinBox;
	private OptionButton chemistryOption;

	public override void _Ready()
	{
		try
		{
			InitializeControls();
			SetupOptions();
			ConnectSignals();
			
			if (SettingsManager.Instance != null)
			{
				UpdateUIFromSettings(SettingsManager.Instance.CurrentSettings);
			}
			else
			{
				GD.PrintErr("SettingsManager.Instance is null in GenerationSettingsWindow._Ready()");
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GenerationSettingsWindow._Ready(): {e.Message}\n{e.StackTrace}");
		}
	}

	private void InitializeControls()
	{
		planetTypeOption = GetNodeOrNull<OptionButton>("MarginContainer/VBoxContainer/PlanetTypeHBox/PlanetTypeOption");
		temperatureSpinBox = GetNodeOrNull<SpinBox>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/TemperatureSpinBox");
		hydrologySpinBox = GetNodeOrNull<SpinBox>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/HydrologySpinBox");
		gravitySpinBox = GetNodeOrNull<SpinBox>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/GravitySpinBox");
		landMassesSpinBox = GetNodeOrNull<SpinBox>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/LandMassesSpinBox");
		chemistryOption = GetNodeOrNull<OptionButton>("MarginContainer/VBoxContainer/ScrollContainer/SettingsGrid/ChemistryOption");
	}

	private void SetupOptions()
	{
		if (planetTypeOption != null)
		{
			foreach (SettingsManager.PlanetType type in Enum.GetValues(typeof(SettingsManager.PlanetType)))
			{
				planetTypeOption.AddItem(type.ToString());
			}
		}

		if (chemistryOption != null)
		{
			foreach (SettingsManager.PlanetSettings.ChemistryBasis chemistry in 
					Enum.GetValues(typeof(SettingsManager.PlanetSettings.ChemistryBasis)))
			{
				chemistryOption.AddItem(chemistry.ToString());
			}
		}
	}

	private void ConnectSignals()
	{
		var randomizeButton = GetNodeOrNull<Button>("MarginContainer/VBoxContainer/ButtonContainer/RandomizeButton");
		var generateButton = GetNodeOrNull<Button>("MarginContainer/VBoxContainer/ButtonContainer/GenerateButton");
		var cancelButton = GetNodeOrNull<Button>("MarginContainer/VBoxContainer/ButtonContainer/CancelButton");

		if (randomizeButton != null) randomizeButton.Pressed += OnRandomizePressed;
		if (generateButton != null) generateButton.Pressed += OnGeneratePressed;
		if (cancelButton != null) cancelButton.Pressed += OnCancelPressed;
		if (planetTypeOption != null) planetTypeOption.ItemSelected += OnPlanetTypeSelected;
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
		var settings = SettingsManager.Instance.CurrentSettings;
		settings.Temperature = (float)temperatureSpinBox.Value;
		settings.Hydrology = (float)hydrologySpinBox.Value;
		settings.Gravity = (float)gravitySpinBox.Value;
		settings.LandMasses = (int)landMassesSpinBox.Value;
		settings.PrimaryChemistry = (SettingsManager.PlanetSettings.ChemistryBasis)chemistryOption.Selected;

		EmitSignal(SignalName.SettingsConfirmed);
		Hide();
	}

	private void OnCancelPressed()
	{
		Hide();
	}
}
