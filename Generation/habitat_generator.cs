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
		GasGiant
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

	public (string habitat, HabitatZone zone) DetermineHabitat(SettingsManager.PlanetSettings settings, string specificHabitat = null)
	{
		if (specificHabitat != null)
		{
			return (specificHabitat, DetermineHabitatZone(specificHabitat));
		}

		if (settings.PlanetType == SettingsManager.PlanetType.Jovian)
		{
			return DetermineGasGiantHabitat();
		}

		HabitatZone zone = DetermineHabitatZone(settings);
		string habitat = DetermineSpecificHabitat(zone, settings);

		return (habitat, zone);
	}

	private HabitatZone DetermineHabitatZone(SettingsManager.PlanetSettings settings)
	{
		int zoneRoll = Roll.Dice(1, 6);
		int modifier = CalculateHydrologyModifier(settings.Hydrology);
		
		zoneRoll += modifier;

		if (zoneRoll <= 3)
			return HabitatZone.Land;
		else
			return HabitatZone.Water;
	}

	private int CalculateHydrologyModifier(float hydrology)
	{
		if (hydrology <= 10) return -2;
		if (hydrology <= 50) return -1;
		if (hydrology >= 90) return 2;
		if (hydrology >= 80) return 1;
		return 0;
	}

	private string DetermineSpecificHabitat(HabitatZone zone, SettingsManager.PlanetSettings settings)
	{
		var habitats = BioData.Habitats();
		var availableHabitats = GetAvailableHabitats(zone, settings);

		if (!availableHabitats.Any())
		{
			return zone == HabitatZone.Land ? "Plain" : "Sea";
		}

		return Roll.Seek(availableHabitats.ToDictionary(h => h, h => habitats[zone.ToString()][h]));
	}

	private (string habitat, HabitatZone zone) DetermineGasGiantHabitat()
	{
		var habitats = BioData.Habitats()["Water"];
		return (Roll.Seek(habitats), HabitatZone.GasGiant);
	}

	private List<string> GetAvailableHabitats(HabitatZone zone, SettingsManager.PlanetSettings settings)
	{
		var allHabitats = BioData.Habitats()[zone.ToString()];
		return allHabitats.Keys.Where(h => IsHabitatViable(h, settings)).ToList();
	}

	public List<string> DetermineAvailableHabitats(SettingsManager.PlanetSettings settings)
	{
		var availableHabitats = new List<string>();
		
		if (settings.PlanetType == SettingsManager.PlanetType.Jovian)
		{
			return BioData.Habitats()["Water"].Keys.ToList();
		}

		foreach (var zoneStr in new[] { "Land", "Water" })
		{
			var zoneHabitats = BioData.Habitats()[zoneStr];
			foreach (var habitat in zoneHabitats.Keys)
			{
				if (IsHabitatViable(habitat, settings))
				{
					availableHabitats.Add(habitat);
				}
			}
		}

		// Handle edge cases
		if (settings.Hydrology == 0)
		{
			availableHabitats.RemoveAll(h => h != "Sea" && h != "Lake" && h != "River");
		}
		else if (settings.Hydrology == 100)
		{
			availableHabitats.RemoveAll(h => BioData.Habitats()["Land"].ContainsKey(h) && h != "Beach");
		}

		return availableHabitats;
	}

	private bool IsHabitatViable(string habitat, SettingsManager.PlanetSettings settings)
	{
		switch (habitat)
		{
			case "Arctic":
				return settings.Temperature < 273;
			case "Desert":
				return settings.Temperature > 300 && settings.Hydrology < 20;
			case "Beach":
				return settings.Hydrology > 0;
			case "Woodland":
				return settings.Temperature is >= 278 and <= 298 && settings.Hydrology is >= 30 and <= 80;
			case "Swampland":
				return settings.Temperature > 283 && settings.Hydrology > 60;
			case "Mountain":
				return settings.Hydrology < 90;
			case "Plain":
				return settings.Temperature is >= 278 and <= 308 && settings.Hydrology is >= 10 and <= 70;
			case "Jungle":
				return settings.Temperature > 293 && settings.Hydrology > 50;
			case "Ocean":
			case "Deep Ocean":
			case "Sea":
				return settings.Hydrology > 50;
			case "Lake":
			case "River":
				return settings.Hydrology > 10;
			case "Lagoon":
				return settings.Hydrology > 40 && settings.Temperature > 288;
			case "Reef":
				return settings.Hydrology > 60 && settings.Temperature > 293;
			default:
				return true;
		}
	}

	private HabitatZone DetermineHabitatZone(string habitat)
	{
		var habitats = BioData.Habitats();
		if (habitats["Water"].ContainsKey(habitat))
			return HabitatZone.Water;
		return HabitatZone.Land;
	}
}
