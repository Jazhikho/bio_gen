using Godot;
using System;

public partial class OptionsManager : Node
{
	private Window optionsWindow;

	public void Initialize(Window options)
	{
		optionsWindow = options;
	}

	public void ShowOptions()
	{
		if (optionsWindow != null)
			optionsWindow.Show();
		else
			GD.PrintErr("Options window is null!");
	}
}
