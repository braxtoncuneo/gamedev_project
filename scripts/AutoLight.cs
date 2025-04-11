using Godot;
using System;

public partial class AutoLight : Area3D
{

	SpotLight3D Light;
	
	public override void _Ready() {
		Light = GetNode("SpotLight3D")as SpotLight3D;
		BodyEntered += OnEnter;
		BodyExited  += OnExit;
	}
	
	public void OnEnter(Node3D node) {
		if (node.Name == "Player") {
			Light.Show();
		}
	}
	
	public void OnExit(Node3D node) {
		if (node.Name == "Player") {
			Light.Hide();
		}
	}
	
}
