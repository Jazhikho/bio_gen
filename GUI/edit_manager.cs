using Godot;
using System;
using BioStructures;

public partial class EditManager : Node
{
	public static EditManager Instance { get; private set; }
	
	private Window editWindow;
	private TreeManager treeManager;
	private DisplayManager displayManager;

	public override void _EnterTree()
	{
		if (Instance == null) Instance = this;
		else QueueFree();
	}

	public void Initialize(Window editWindow, TreeManager treeManager, DisplayManager displayManager)
	{
		this.editWindow = editWindow;
		this.treeManager = treeManager;
		this.displayManager = displayManager;
	}

	public void ShowEditWindow(TreeItem selectedItem)
	{
		if (selectedItem == null) return;
		
		var metadata = (selectedItem.GetMetadata(0).As<Godot.Collections.Dictionary>());
		if (metadata == null) return;

		// Clear existing UI
		foreach (Node child in editWindow.GetChildren())
		{
			child.QueueFree();
		}

		var scrollContainer = new ScrollContainer();
		scrollContainer.CustomMinimumSize = new Vector2(400, 600);
		var vbox = new VBoxContainer();
		scrollContainer.AddChild(vbox);
		editWindow.AddChild(scrollContainer);

		string itemType = metadata.ContainsKey("ItemType") ? metadata["ItemType"].AsString() : "Unknown";

		switch (itemType)
		{
			case "Creature":
				CreateCreatureEditUI(vbox, metadata);
				break;
			case "Planet":
				CreatePlanetEditUI(vbox, metadata);
				break;
			case "LandMass":
				CreateLandMassEditUI(vbox, metadata);
				break;
			case "WaterBody":
				CreateWaterBodyEditUI(vbox, metadata);
				break;
			case "Ecosystem":
				CreateEcosystemEditUI(vbox, metadata);
				break;
			default:
				GD.PrintErr($"Unknown item type: {itemType}");
				return;
		}

		// Add Save Button
		var saveButton = new Button();
		saveButton.Text = "Save Changes";
		saveButton.Pressed += () => SaveChanges(selectedItem, metadata);
		vbox.AddChild(saveButton);
		
		// Add Cancel Button
		var cancelButton = new Button();
		cancelButton.Text = "Cancel";
		cancelButton.Pressed += () => editWindow.Hide();
		vbox.AddChild(cancelButton);

		editWindow.Title = $"Edit {itemType}";
		editWindow.Show();
	}

	private void CreateCreatureEditUI(VBoxContainer vbox, Godot.Collections.Dictionary metadata)
	{
		// Basic Properties
		vbox.AddChild(CreateLabeledLineEdit("Name", metadata["Name"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Chemical Basis", metadata["ChemicalBasis"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Habitat", metadata["Habitat"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Trophic Level", metadata["TrophicLevel"].AsString()));
		
		// Physical Characteristics
		vbox.AddChild(CreateSeparator("Physical Characteristics"));
		vbox.AddChild(CreateLabeledLineEdit("Size Category", metadata["SizeCategory"].AsString()));
		vbox.AddChild(CreateLabeledNumberEdit("Specific Size", metadata["SpecificSize"].AsSingle()));
		vbox.AddChild(CreateLabeledLineEdit("Symmetry", metadata["Symmetry"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Locomotion", metadata["Locomotion"].AsString()));
		
		// Limbs and Structure
		vbox.AddChild(CreateSeparator("Limbs and Structure"));
		vbox.AddChild(CreateLabeledLineEdit("Limb Structure", metadata["LimbStructure"].AsString()));
		vbox.AddChild(CreateLabeledNumberEdit("Actual Limb Count", metadata["ActualLimbCount"].AsInt32()));
		vbox.AddChild(CreateLabeledLineEdit("Tail Features", metadata["TailFeatures"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Manipulator Type", metadata["ManipulatorType"].AsString()));
		vbox.AddChild(CreateLabeledNumberEdit("Actual Manipulator Count", metadata["ActualManipulatorCount"].AsInt32()));
		
		// Body Structure
		vbox.AddChild(CreateSeparator("Body Structure"));
		vbox.AddChild(CreateLabeledLineEdit("Skeleton", metadata["Skeleton"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Skin Covering", metadata["SkinCovering"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Skin Type", metadata["SkinType"].AsString()));
		
		// Reproduction
		vbox.AddChild(CreateSeparator("Reproduction"));
		vbox.AddChild(CreateLabeledLineEdit("Growth Pattern", metadata["GrowthPattern"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Sexes", metadata["Sexes"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Gestation", metadata["Gestation"].AsString()));
		if(metadata.ContainsKey("SpecialGestation"))
			vbox.AddChild(CreateLabeledLineEdit("Special Gestation", metadata["SpecialGestation"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Reproductive Strategy", metadata["ReproductiveStrategy"].AsString()));
		
		// Senses and Intelligence
		vbox.AddChild(CreateSeparator("Senses and Intelligence"));
		vbox.AddChild(CreateLabeledLineEdit("Primary Sense", metadata["PrimarySense"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Animal Intelligence", metadata["AnimalIntelligence"].AsString()));
		
		// Social
		vbox.AddChild(CreateSeparator("Social"));
		vbox.AddChild(CreateLabeledLineEdit("Mating Behavior", metadata["MatingBehavior"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Social Organization", metadata["SocialOrganization"].AsString()));
		
		// Note: Add handlers for SenseCapabilities, SpecialSenses, and MentalTraits dictionaries if needed
	}

	private void CreatePlanetEditUI(VBoxContainer vbox, Godot.Collections.Dictionary metadata)
	{
		vbox.AddChild(CreateLabeledLineEdit("Name", metadata["Name"].AsString()));
		// Add other planet-specific fields
	}

	private void CreateLandMassEditUI(VBoxContainer vbox, Godot.Collections.Dictionary metadata)
	{
		vbox.AddChild(CreateLabeledLineEdit("Name", metadata["Name"].AsString()));
		// Add other landmass-specific fields
	}

	private void CreateWaterBodyEditUI(VBoxContainer vbox, Godot.Collections.Dictionary metadata)
	{
		vbox.AddChild(CreateLabeledLineEdit("Name", metadata["Name"].AsString()));
		vbox.AddChild(CreateLabeledLineEdit("Water Type", metadata["WaterType"].AsString()));
		// Add other waterbody-specific fields
	}

	private void CreateEcosystemEditUI(VBoxContainer vbox, Godot.Collections.Dictionary metadata)
	{
		vbox.AddChild(CreateLabeledLineEdit("Habitat Type", metadata["HabitatType"].AsString()));
		// Add other ecosystem-specific fields
	}

	private HBoxContainer CreateLabeledLineEdit(string labelText, string value)
	{
		var hbox = new HBoxContainer();
		var label = new Label();
		label.Text = labelText;
		label.CustomMinimumSize = new Vector2(250, 0);
		var lineEdit = new LineEdit();
		lineEdit.Text = value;
		lineEdit.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		hbox.AddChild(label);
		hbox.AddChild(lineEdit);
		return hbox;
	}

	private HBoxContainer CreateLabeledNumberEdit(string labelText, float value)
	{
		var hbox = new HBoxContainer();
		var label = new Label();
		label.Text = labelText;
		label.CustomMinimumSize = new Vector2(250, 0);
		var spinBox = new SpinBox();
		spinBox.Value = value;
		spinBox.MinValue = 0;
		spinBox.MaxValue = 1000;
		spinBox.Step = 0.1f;
		spinBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		hbox.AddChild(label);
		hbox.AddChild(spinBox);
		return hbox;
	}

	private HBoxContainer CreateLabeledNumberEdit(string labelText, int value)
	{
		var hbox = new HBoxContainer();
		var label = new Label();
		label.Text = labelText;
		label.CustomMinimumSize = new Vector2(250, 0);
		var spinBox = new SpinBox();
		spinBox.Value = value;
		spinBox.MinValue = 0;
		spinBox.MaxValue = 1000;
		spinBox.Step = 1;
		spinBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		hbox.AddChild(label);
		hbox.AddChild(spinBox);
		return hbox;
	}

	private Label CreateSeparator(string sectionTitle)
	{
		var label = new Label();
		label.Text = $"\n[b]{sectionTitle}[/b]";
		return label;
	}

	private void SaveChanges(TreeItem selectedItem, Godot.Collections.Dictionary originalMetadata)
	{
		var scrollContainer = editWindow.GetChild<ScrollContainer>(0);
		var vbox = scrollContainer.GetChild<VBoxContainer>(0);
		foreach (Node child in vbox.GetChildren())
		{
			if (child is HBoxContainer hbox && hbox.GetChildCount() > 1)
			{
				var label = hbox.GetChild<Label>(0);
				var control = hbox.GetChild(1);
				if (control is LineEdit lineEdit)
				{
					originalMetadata[label.Text] = lineEdit.Text;
				}
				else if (control is SpinBox spinBox)
				{
					originalMetadata[label.Text] = spinBox.Value;
				}
			}
		}

		selectedItem.SetMetadata(0, originalMetadata);
		displayManager.DisplayDetails(originalMetadata); // Update display
		editWindow.Hide();
	}
}
