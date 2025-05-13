using Godot;
using System;

public partial class Menu : Control
{
	
	Button startButton;
	
	private void StartGame() {
		Input.MouseMode = Input.MouseModeEnum.Hidden;
		Hide();
	}
	
	public override void _Ready() {
		startButton = GetNode("Button") as Button;
		startButton.Pressed += StartGame;
	}
	
	
	
}
