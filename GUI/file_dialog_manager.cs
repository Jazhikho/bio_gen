using Godot;
using System;

public partial class FileDialogManager : Node
{
	private FileDialog saveDialog;
	private FileDialog openDialog;

	public void Initialize(FileDialog save, FileDialog open)
	{
		saveDialog = save;
		openDialog = open;
	}

	public void ShowSaveDialog()
	{
		if (saveDialog != null)
			saveDialog.Show();
		else
			GD.PrintErr("Save dialog is null!");
	}

	public void ShowOpenDialog()
	{
		if (openDialog != null)
			openDialog.Show();
		else
			GD.PrintErr("Open dialog is null!");
	}
}
