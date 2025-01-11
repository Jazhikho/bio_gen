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

			TrophicGenerator.Instance.GenerateTrophicLevel(creature, settings, habitatInfo);
			LocomotionGenerator.Instance.GenerateLocomotion(creature, settings, habitatInfo);
			SizeGenerator.Instance.GenerateSize(creature, settings, habitatInfo);
			PhysiologyGenerator.Instance.GeneratePhysiology(creature, settings, habitatInfo);
			ReproductionGenerator.Instance.GenerateReproduction(creature, settings);
			SensesGenerator.Instance.GenerateSenses(creature, settings, habitatInfo);
			BehaviorGenerator.Instance.GenerateBehavior(creature, settings, habitatInfo);
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
		return settings;
	}
}
