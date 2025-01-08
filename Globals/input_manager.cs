using Godot;
using System;

public partial class InputManager : Node
{
	public static InputManager Instance { get; private set; }

	// Signal declarations
	[Signal]
	public delegate void ButtonPressedEventHandler(string buttonName);
	[Signal]
	public delegate void ValueChangedEventHandler(string settingName, float value);
	[Signal]
	public delegate void TextEnteredEventHandler(string fieldName, string text);
	[Signal]
	public delegate void ItemSelectedEventHandler(string listName, int index);

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

	// Button handling
	public void HandleButtonPress(string buttonName)
	{
		EmitSignal(SignalName.ButtonPressed, buttonName);
	}

	// Slider or SpinBox value changes
	public void HandleValueChange(string settingName, float value)
	{
		EmitSignal(SignalName.ValueChanged, settingName, value);
	}

	// Text input handling
	public void HandleTextEntered(string fieldName, string text)
	{
		EmitSignal(SignalName.TextEntered, fieldName, text);
	}

	// List selection handling
	public void HandleItemSelected(string listName, int index)
	{
		EmitSignal(SignalName.ItemSelected, listName, index);
	}
}
