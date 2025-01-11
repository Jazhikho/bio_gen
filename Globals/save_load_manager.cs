using Godot;
using System;
using BioStructures;

public partial class SaveLoadManager : Node
{
	public static SaveLoadManager Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance == null) Instance = this;
		else QueueFree();
	}

	public void SaveData(Ecosystem ecosystem, string filePath)
	{
		DataManager.Instance.SaveEcosystem(ecosystem, filePath);
	}

	public Ecosystem LoadData(string filePath)
	{
		return DataManager.Instance.LoadEcosystem(filePath);
	}
}
