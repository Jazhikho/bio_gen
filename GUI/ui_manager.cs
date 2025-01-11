using Godot;

public partial class UIManager : Node
{
	private Button generateButton;
	public Button generateSingleButton;
	public Button editButton;
	public Button saveButton;
	public Button deleteButton;
	private Button openFileButton;
	private Button optionsButton;

	public void Initialize(Button gen, Button genSingle, Button edit, Button save, Button delete, Button options, Button openFile)
	{
		generateButton = gen;
		generateSingleButton = genSingle;
		editButton = edit;
		saveButton = save;
		deleteButton = delete;
		openFileButton = openFile;
		optionsButton = options;

		SetButtonStates(false);
	}

	public void SetButtonStates(bool enabled)
{
	generateSingleButton.Disabled = !enabled;
	editButton.Disabled = !enabled;
	saveButton.Disabled = !enabled;
	deleteButton.Disabled = !enabled;
}
}
