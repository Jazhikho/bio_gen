using Godot;
using System;
using BioLibrary;
using BioStructures;

public partial class TrophicGenerator : Node
{
	public static TrophicGenerator Instance { get; private set; }

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

	public void GenerateTrophicLevel(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		try
		{
			creature.TrophicLevel = DetermineTrophicLevel(creature, settings, habitatInfo);
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in GenerateTrophicLevel: {e.Message}\n{e.StackTrace}");
		}
	}

	private string DetermineTrophicLevel(Creature creature, SettingsManager.PlanetSettings settings, (string habitat, HabitatGenerator.HabitatZone zone) habitatInfo)
	{
		// Check against the Non-sapient table
		var trophicTable = BioData.Trophic_Level()["Non-sapient"];
		return Roll.Seek(trophicTable);
	}
}
