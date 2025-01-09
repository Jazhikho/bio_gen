using Godot;
using System.Collections.Generic;
using System.Linq;
using BioStructures;

public partial class GenerationManager : Node
{
	public static GenerationManager Instance { get; private set; }

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

	public Creature GenerateSingleSpecies(SettingsManager.PlanetSettings settings = null, string specificHabitat = null)
	{
		settings ??= GetDefaultSettings();
		
		var habitatInfo = HabitatGenerator.Instance.DetermineHabitat(settings, specificHabitat);
		var creature = new Creature();
		
		// Coordinate the generation steps
		PhysiologyGenerator.Instance.GeneratePhysiology(creature, settings, habitatInfo);
		BehaviorGenerator.Instance.GenerateBehavior(creature, habitatInfo);
		NamingGenerator.Instance.NameCreature(creature);

		return creature;
	}

	public Ecosystem[] GenerateMultipleSpecies(int count, SettingsManager.PlanetSettings settings = null)
	{
		settings ??= GetDefaultSettings();
		
		var habitats = HabitatGenerator.Instance.DetermineAvailableHabitats(settings);
		var ecosystems = InitializeEcosystems(habitats);
		
		PopulateEcosystems(ecosystems, count, settings);
		
		return ConvertToEcosystemArray(ecosystems);
	}

	private SettingsManager.PlanetSettings GetDefaultSettings()
	{
		var settings = new SettingsManager.PlanetSettings();
		SettingsManager.Instance.SetPresetSettings(SettingsManager.PlanetType.Gaian);
		return settings;
	}

	private Dictionary<string, List<Creature>> InitializeEcosystems(List<string> habitats)
	{
		return habitats.ToDictionary(h => h, h => new List<Creature>());
	}

	private void PopulateEcosystems(Dictionary<string, List<Creature>> ecosystems, int totalCount, SettingsManager.PlanetSettings settings)
	{
		int habitatCount = ecosystems.Count;
		foreach (var habitat in ecosystems.Keys)
		{
			int habitatSpeciesCount = totalCount / habitatCount;
			for (int i = 0; i < habitatSpeciesCount; i++)
			{
				var creature = GenerateSingleSpecies(settings, habitat);
				ecosystems[habitat].Add(creature);
			}
		}
	}

	private Ecosystem[] ConvertToEcosystemArray(Dictionary<string, List<Creature>> ecosystems)
	{
		return ecosystems.Select(kvp => new Ecosystem
		{
			HabitatType = kvp.Key,
			Creatures = kvp.Value.ToArray(),
			EcosystemID = Roll.Dice(3, 6),
			LocationID = Roll.Dice(3, 6)
		}).ToArray();
	}
}
