using Godot;
using System;
using System.Collections.Generic;

public partial class SettingsManager : Node
{
	public static SettingsManager Instance { get; private set; }

	// Planet Settings
	public class PlanetSettings
	{
		// Core parameters
		public float Temperature { get; set; } = 287.0f; // in Kelvin
		public float Hydrology { get; set; } = 70.0f; // in percentage
		public float Gravity { get; set; } = 1.0f; // in G's
		public int LandMasses { get; set; } = 1;

		// Chemistry
		public enum ChemistryBasis
		{
			HydrogenBased,
			AmmoniaBased,
			HydrocarbonBased,
			CarbonBased,
			ChlorineBased,
			SiliconBased,
			SulfurBased,
			Machine
		}
		public ChemistryBasis PrimaryChemistry { get; set; } = ChemistryBasis.CarbonBased;

		// Additional suggested parameters
		public float AtmosphericPressure { get; set; } = 1.0f; // in Earth atmospheres
		public float DayLength { get; set; } = 24.0f; // in Earth hours
		public float YearLength { get; set; } = 365.0f; // in Earth days
		public bool HasMagneticField { get; set; } = true;
		public float OrbitalTilt { get; set; } = 23.5f; // in degrees
		public List<string> AtmosphericComposition { get; set; } = new List<string>();
		public float RadiationLevel { get; set; } = 1.0f; // 1.0 = Earth normal
		public bool HasSeasonalCycles { get; set; } = true;
		public float TectonicActivity { get; set; } = 1.0f; // 1.0 = Earth normal
	}

	private PlanetSettings currentSettings = new PlanetSettings();
	public PlanetSettings CurrentSettings => currentSettings;

	// Temperature ranges for reference
	public readonly float MinTemperature = 50.0f;
	public readonly float MaxTemperature = 150.0f;

	// Hydrology ranges (can exceed 100% for gas giants)
	public readonly float MinHydrology = 0.0f;
	public readonly float MaxHydrology = 120.0f;

	// Gravity ranges
	public readonly float MinGravity = 0.1f;
	public readonly float MaxGravity = 10.0f;

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

	// Randomize all settings
	public void RandomizeSettings()
	{
		Random rand = new Random();
		currentSettings.Temperature = (float)(rand.NextDouble() * (MaxTemperature - MinTemperature) + MinTemperature);
		currentSettings.Hydrology = (float)(rand.NextDouble() * MaxHydrology);
		currentSettings.Gravity = (float)(rand.NextDouble() * (MaxGravity - MinGravity) + MinGravity);
		currentSettings.LandMasses = rand.Next(1, 8);
		currentSettings.PrimaryChemistry = (PlanetSettings.ChemistryBasis)rand.Next(0, Enum.GetValues(typeof(PlanetSettings.ChemistryBasis)).Length);

		// Randomize additional parameters
		currentSettings.AtmosphericPressure = (float)(rand.NextDouble() * 5.0);
		currentSettings.DayLength = (float)(rand.NextDouble() * 100);
		currentSettings.YearLength = (float)(rand.NextDouble() * 1000 + 100);
		currentSettings.HasMagneticField = rand.NextDouble() > 0.3;
		currentSettings.OrbitalTilt = (float)(rand.NextDouble() * 90);
		currentSettings.RadiationLevel = (float)(rand.NextDouble() * 5);
		currentSettings.HasSeasonalCycles = rand.NextDouble() > 0.2;
		currentSettings.TectonicActivity = (float)(rand.NextDouble() * 3);
	}

	// Save settings to file
	public void SaveSettings(string filepath)
	{
		// Implement save functionality
		// Could use JSON serialization
	}

	// Load settings from file
	public void LoadSettings(string filepath)
	{
		// Implement load functionality
	}

	// Validate settings
	public bool ValidateSettings()
	{
		return currentSettings.Temperature >= MinTemperature &&
			   currentSettings.Temperature <= MaxTemperature &&
			   currentSettings.Hydrology >= MinHydrology &&
			   currentSettings.Hydrology <= MaxHydrology &&
			   currentSettings.Gravity >= MinGravity &&
			   currentSettings.Gravity <= MaxGravity &&
			   currentSettings.LandMasses > 0;
	}

	// Get climate zone based on temperature and other factors
	public string GetClimateZone()
	{
		// Implement climate zone calculation based on temperature, hydrology, and other factors
		return "Temperate"; // Placeholder
	}

	// Calculate habitability index
	public float CalculateHabitabilityIndex()
	{
		// Implement habitability calculation based on all parameters
		return 1.0f; // Placeholder
	}
	// Preset settings for different planet types
	public enum PlanetType
	{
		Jovian,
		Arean,
		Arid,
		Oceanic,
		Panthalassic,
		Promethean,
		Snowball,
		Gaian,
		Vesperian
	}

	public void SetPresetSettings(PlanetType planetType)
	{
		switch (planetType)
		{
			case PlanetType.Jovian:
				currentSettings.Temperature = 150.0f;
				currentSettings.Hydrology = 0.0f;
				currentSettings.Gravity = 2.5f;
				currentSettings.LandMasses = 0;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.HydrogenBased;
				currentSettings.AtmosphericPressure = 100.0f;
				currentSettings.DayLength = 10.0f;
				currentSettings.YearLength = 4333.0f;
				currentSettings.HasMagneticField = true;
				currentSettings.OrbitalTilt = 3.0f;
				currentSettings.RadiationLevel = 10.0f;
				currentSettings.HasSeasonalCycles = false;
				currentSettings.TectonicActivity = 0.0f;
				break;
			case PlanetType.Arean:
				currentSettings.Temperature = 250.0f;
				currentSettings.Hydrology = 5.0f;
				currentSettings.Gravity = 0.4f;
				currentSettings.LandMasses = 3;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.CarbonBased;
				currentSettings.AtmosphericPressure = 0.6f;
				currentSettings.DayLength = 24.6f;
				currentSettings.YearLength = 687.0f;
				currentSettings.HasMagneticField = false;
				currentSettings.OrbitalTilt = 25.0f;
				currentSettings.RadiationLevel = 2.0f;
				currentSettings.HasSeasonalCycles = true;
				currentSettings.TectonicActivity = 0.1f;
				break;
			case PlanetType.Arid:
				currentSettings.Temperature = 310.0f;
				currentSettings.Hydrology = 10.0f;
				currentSettings.Gravity = 1.0f;
				currentSettings.LandMasses = 2;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.CarbonBased;
				currentSettings.AtmosphericPressure = 0.8f;
				currentSettings.DayLength = 30.0f;
				currentSettings.YearLength = 365.0f;
				currentSettings.HasMagneticField = true;
				currentSettings.OrbitalTilt = 10.0f;
				currentSettings.RadiationLevel = 1.5f;
				currentSettings.HasSeasonalCycles = false;
				currentSettings.TectonicActivity = 0.5f;
				break;
			case PlanetType.Oceanic:
				currentSettings.Temperature = 290.0f;
				currentSettings.Hydrology = 100.0f;
				currentSettings.Gravity = 1.0f;
				currentSettings.LandMasses = 0;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.CarbonBased;
				currentSettings.AtmosphericPressure = 1.0f;
				currentSettings.DayLength = 24.0f;
				currentSettings.YearLength = 365.0f;
				currentSettings.HasMagneticField = true;
				currentSettings.OrbitalTilt = 23.5f;
				currentSettings.RadiationLevel = 1.0f;
				currentSettings.HasSeasonalCycles = true;
				currentSettings.TectonicActivity = 1.0f;
				break;
			case PlanetType.Panthalassic:
				currentSettings.Temperature = 280.0f;
				currentSettings.Hydrology = 100.0f;
				currentSettings.Gravity = 1.5f;
				currentSettings.LandMasses = 0;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.HydrocarbonBased;
				currentSettings.AtmosphericPressure = 5.0f;
				currentSettings.DayLength = 20.0f;
				currentSettings.YearLength = 500.0f;
				currentSettings.HasMagneticField = true;
				currentSettings.OrbitalTilt = 5.0f;
				currentSettings.RadiationLevel = 1.0f;
				currentSettings.HasSeasonalCycles = false;
				currentSettings.TectonicActivity = 0.2f;
				break;
			case PlanetType.Promethean:
				currentSettings.Temperature = 250.0f;
				currentSettings.Hydrology = 20.0f;
				currentSettings.Gravity = 0.8f;
				currentSettings.LandMasses = 1;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.SiliconBased;
				currentSettings.AtmosphericPressure = 0.9f;
				currentSettings.DayLength = 30.0f;
				currentSettings.YearLength = 400.0f;
				currentSettings.HasMagneticField = true;
				currentSettings.OrbitalTilt = 15.0f;
				currentSettings.RadiationLevel = 1.0f;
				currentSettings.HasSeasonalCycles = true;
				currentSettings.TectonicActivity = 1.5f;
				break;
			case PlanetType.Snowball:
				currentSettings.Temperature = 200.0f;
				currentSettings.Hydrology = 50.0f;
				currentSettings.Gravity = 1.0f;
				currentSettings.LandMasses = 1;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.CarbonBased;
				currentSettings.AtmosphericPressure = 1.0f;
				currentSettings.DayLength = 24.0f;
				currentSettings.YearLength = 365.0f;
				currentSettings.HasMagneticField = true;
				currentSettings.OrbitalTilt = 23.5f;
				currentSettings.RadiationLevel = 1.0f;
				currentSettings.HasSeasonalCycles = true;
				currentSettings.TectonicActivity = 1.0f;
				break;
			case PlanetType.Gaian:
				currentSettings.Temperature = 287.0f;
				currentSettings.Hydrology = 70.0f;
				currentSettings.Gravity = 1.0f;
				currentSettings.LandMasses = 1;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.CarbonBased;
				currentSettings.AtmosphericPressure = 1.0f;
				currentSettings.DayLength = 24.0f;
				currentSettings.YearLength = 365.0f;
				currentSettings.HasMagneticField = true;
				currentSettings.OrbitalTilt = 23.5f;
				currentSettings.RadiationLevel = 1.0f;
				currentSettings.HasSeasonalCycles = true;
				currentSettings.TectonicActivity = 1.0f;
				break;
			case PlanetType.Vesperian:
				currentSettings.Temperature = 300.0f;
				currentSettings.Hydrology = 50.0f;
				currentSettings.Gravity = 1.0f;
				currentSettings.LandMasses = 1;
				currentSettings.PrimaryChemistry = PlanetSettings.ChemistryBasis.CarbonBased;
				currentSettings.AtmosphericPressure = 1.0f;
				currentSettings.DayLength = 100.0f;
				currentSettings.YearLength = 365.0f;
				currentSettings.HasMagneticField = true;
				currentSettings.OrbitalTilt = 0.0f;
				currentSettings.RadiationLevel = 1.0f;
				currentSettings.HasSeasonalCycles = false;
				currentSettings.TectonicActivity = 0.5f;
				break;
		}
	}
}