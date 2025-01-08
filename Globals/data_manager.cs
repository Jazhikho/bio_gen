using Godot;
using System;

public partial class settings_manager : Node
{
	public void SaveEcosystem(Ecosystem ecosystem, string filePath)
	{
		var file = new File();
		file.Open(filePath, File.ModeFlags.Write);
		file.StoreLine(JSON.Print(ecosystem));
		file.Close();
	}

	public Ecosystem LoadEcosystem(string filePath)
	{
		var file = new File();
		if (!file.FileExists(filePath))
		{
			GD.PrintErr("File not found: " + filePath);
			return null;
		}

		file.Open(filePath, File.ModeFlags.Read);
		var json = file.GetAsText();
		file.Close();

		return JSON.Parse(json).Result as Ecosystem;
	}
}