using Godot;
using System;

public partial class AutoLight : Area3D
{

	SpotLight3D _light;

	public override void _Ready() {
		_light = GetNode("SpotLight3D")as SpotLight3D;
		BodyEntered += OnEnter;
		BodyExited  += OnExit;
	}

	public void OnEnter(Node3D node) {
		if (node.Name == "Player") {
			_light.Show();
		}
	}

	public void OnExit(Node3D node) {
		if (node.Name == "Player") {
			_light.Hide();
		}
	}

}
