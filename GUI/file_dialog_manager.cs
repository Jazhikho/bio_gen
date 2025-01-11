using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FileDialogManager : Node
{
	private FileDialog saveDialog;
	private FileDialog openDialog;

	public void Initialize(FileDialog save, FileDialog open)
	{
		saveDialog = save;
		openDialog = open;

		// Set up file dialogs
		ConfigureFileDialogs();
		
		// Initialize SaveLoadManager with the dialogs
		SaveLoadManager.Instance.Initialize(saveDialog, openDialog);
	}

	private void ConfigureFileDialogs()
	{
		// Configure save dialog
		saveDialog.Access = FileDialog.AccessEnum.Filesystem;
		saveDialog.Filters = new string[] { "*.json" };
		
		// Configure open dialog
		openDialog.Access = FileDialog.AccessEnum.Filesystem;
		openDialog.Filters = new string[] { "*.json" };

		// Set default paths
		string defaultPath = OS.GetUserDataDir() + "/ecosystems/";
		DirAccess.MakeDirRecursiveAbsolute(defaultPath);
		
		saveDialog.CurrentDir = defaultPath;
		openDialog.CurrentDir = defaultPath;
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
