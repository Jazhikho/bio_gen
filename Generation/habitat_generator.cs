using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using BioLibrary;

public partial class HabitatGenerator : Node
{
	public static HabitatGenerator Instance { get; private set; }

	public enum HabitatZone
	{
		Land,
		Water,
		Jovian
	}

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

	public (string habitat, HabitatZone zone) DetermineHabitat(SettingsManager.PlanetSettings settings, HabitatZone? preferredZone = null)
	{
		// Special case for gas giants
		if (settings.PlanetType == SettingsManager.PlanetType.Jovian)
		{
			return DetermineGasGiantHabitat();
		}

		// Determine zone if not specified
		HabitatZone selectedZone = preferredZone ?? DetermineZoneBasedOnSettings(settings);
		if (selectedZone == HabitatZone.Jovian)
		{
			selectedZone = settings.Hydrology > 50 ? HabitatZone.Water : HabitatZone.Land;
		}
		
		// Get habitat for the determined zone
		string habitat = DetermineSpecificHabitat(selectedZone, settings);

		return (habitat, selectedZone);
	}

	private HabitatZone DetermineZoneBasedOnSettings(SettingsManager.PlanetSettings settings)
	{
		// Handle extreme cases
		if (settings.Hydrology == 0) return HabitatZone.Land;
		if (settings.Hydrology == 100) return HabitatZone.Water;

		// Roll d6 and apply hydrology modifiers
		int roll = Roll.Dice(1, 6);
		if (settings.Hydrology <= 10) roll -= 2;
		else if (settings.Hydrology <= 30) roll -= 1;
		else if (settings.Hydrology >= 90) roll += 2;
		else if (settings.Hydrology >= 70) roll += 1;

		return roll <= 3 ? HabitatZone.Land : HabitatZone.Water;
	}

	private string DetermineSpecificHabitat(HabitatZone zone, SettingsManager.PlanetSettings settings)
	{
		// Get base roll (3d6)
		int roll = Roll.Dice();

		// Apply hydrology modifiers to the roll
		if (settings.Hydrology <= 10) roll -= 2;
		else if (settings.Hydrology <= 50) roll -= 1;
		else if (settings.Hydrology >= 90) roll += 2;
		else if (settings.Hydrology >= 80) roll += 1;

		// Get habitat table for the zone
		var habitatTable = BioData.Habitats()[zone.ToString()];
		
		// Find appropriate habitat based on roll
		foreach (var habitat in habitatTable.OrderBy(h => h.Value))
		{
			if (roll <= habitat.Value && IsHabitatViable(habitat.Key, settings))
			{
				return habitat.Key;
			}
		}

		// Default habitats if no match found
		return zone == HabitatZone.Land ? "Plain" : "Sea";
	}

	private (string habitat, HabitatZone zone) DetermineGasGiantHabitat()
	{
		var habitats = BioData.Habitats()["Jovian"];
		return (Roll.Seek(habitats), HabitatZone.Jovian);
	}

	private bool IsHabitatViable(string habitat, SettingsManager.PlanetSettings settings)
	{
		switch (habitat.ToLower())
		{
			case "arctic": return settings.Temperature < 273;
			case "desert": return settings.Temperature > 300 && settings.Hydrology < 20;
			case "beach": return settings.Hydrology > 0 && settings.Hydrology < 100;
			case "woodland": return settings.Temperature is >= 278 and <= 298 && 
								  settings.Hydrology is >= 30 and <= 80;
			case "swampland": return settings.Temperature > 283 && settings.Hydrology > 60;
			case "mountain": return settings.Hydrology < 90;
			case "plain": return settings.Temperature is >= 278 and <= 308 && 
							   settings.Hydrology is >= 10 and <= 70;
			case "jungle": return settings.Temperature > 293 && settings.Hydrology > 50;
			case "ocean":
			case "deep ocean":
			case "sea": return settings.Hydrology > 50;
			case "lake":
			case "river": return settings.Hydrology > 10;
			case "lagoon": return settings.Hydrology > 40 && settings.Temperature > 288;
			case "reef": return settings.Hydrology > 60 && settings.Temperature > 293;
			default: return true;
		}
	}

	public HabitatZone DetermineHabitatZone(string habitat)
	{
		var habitats = BioData.Habitats();
		if (habitats["Water"].ContainsKey(habitat)) return HabitatZone.Water;
		if (habitats["Jovian"].ContainsKey(habitat)) return HabitatZone.Jovian;
		return HabitatZone.Land;
	}
}
