using Godot;
using System;
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

	public Planet GeneratePlanet(SettingsManager.PlanetSettings settings = null)
	{
		try
		{
			return PlanetGenerator.Instance.GeneratePlanet(settings);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GeneratePlanet: {e.Message}\n{e.StackTrace}");
			return null;
		}
	}

	public Creature GenerateSingleSpecies(SettingsManager.PlanetSettings settings, Ecosystem targetEcosystem)
	{
		try
		{
			settings ??= GetDefaultSettings();
			
			var creature = new Creature();
			creature.Habitat = targetEcosystem.HabitatType;
			
			var habitatZone = HabitatGenerator.Instance.DetermineHabitatZone(targetEcosystem.HabitatType);
			var habitatInfo = (targetEcosystem.HabitatType, habitatZone);
			
			// Set chemical basis first
			creature.ChemicalBasis = settings.PrimaryChemistry.ToString().Replace("Based", "-Based");

			GD.Print("Generating trophic level...");
			TrophicGenerator.Instance.GenerateTrophicLevel(creature, settings, habitatInfo);

			GD.Print("Generating locomotion...");
			LocomotionGenerator.Instance.GenerateLocomotion(creature, settings, habitatInfo);

			GD.Print("Generating size...");
			SizeGenerator.Instance.GenerateSize(creature, settings, habitatInfo);

			GD.Print("Generating physiology...");
			PhysiologyGenerator.Instance.GeneratePhysiology(creature, settings, habitatInfo);
			
			GD.Print("Generating reproduction...");
			ReproductionGenerator.Instance.GenerateReproduction(creature, settings);

			GD.Print("Generating senses...");
			SensesGenerator.Instance.GenerateSenses(creature, settings, habitatInfo);
			
			GD.Print("Generating behavior...");
			BehaviorGenerator.Instance.GenerateBehavior(creature, settings, habitatInfo);
			
			GD.Print("Naming creature...");
			NamingGenerator.Instance.NameCreature(creature);

			return creature;
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GenerateSingleSpecies: {e.Message}\n{e.StackTrace}");
			return null;
		}
	}

	private SettingsManager.PlanetSettings GetDefaultSettings()
	{
		if (SettingsManager.Instance == null)
		{
			GD.PrintErr("SettingsManager.Instance is null in GetDefaultSettings");
			return new SettingsManager.PlanetSettings();
		}
		
		var settings = new SettingsManager.PlanetSettings();
		SettingsManager.Instance.SetPresetSettings(SettingsManager.PlanetType.Gaian);
		return settings;
	}
}
