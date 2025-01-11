using Godot;
using System;
using BioLibrary;
using BioStructures;

public partial class LocomotionGenerator : Node
{
	public static LocomotionGenerator Instance { get; private set; }

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

	public void GenerateLocomotion(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		try
		{
			creature.Locomotion = DetermineLocomotion(creature, habitatInfo);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GenerateLocomotion: {e.Message}\n{e.StackTrace}");
		}
	}

	private string DetermineLocomotion(Creature creature, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		var locomotionOptions = BioData.Primary_Locomotion();
		
		if (!locomotionOptions.ContainsKey(habitatInfo.habitat))
		{
			return Roll.Seek(locomotionOptions["Plain"]);
		}

		return Roll.Seek(locomotionOptions[habitatInfo.habitat]);
	}
}
