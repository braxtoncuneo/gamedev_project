using Godot;
using System;


public partial class Player : RigidBody3D
{
	
	Camera3D CurrentCamera;
	
	[Export]
	float Acceleration = 4.0f;
	
	bool debugMovement;
	
	[Export]
	bool DebugMovement {
		get { return debugMovement; }
		set {
			var node = GetNode("DebugMovement") as Node3D;
			debugMovement = value;
			if (node is null) {
				return;
			} else if (value) {
				node.Show();
			} else {
				node.Hide();
			}
		}
	}
	
	public override void _Ready()
	{
		CurrentCamera = GetViewport().GetCamera3D();
		Vector4 red   = new Vector4(1,0,0,1);
		Vector4 green = new Vector4(0,1,0,1);
		Vector4 blue  = new Vector4(0,0,1,1);
		Vector4 pink  = new Vector4(1,0,1,1);
		VectorReadout.SetColor(GetNode("DebugMovement/Right"),  red);
		VectorReadout.SetColor(GetNode("DebugMovement/Up"),     green);
		VectorReadout.SetColor(GetNode("DebugMovement/Forward"),blue);
		VectorReadout.SetColor(GetNode("DebugMovement/Force"),  pink);
		DebugMovement = DebugMovement;
	}
	
	public override void _Process(double delta)
	{
		Vector3 movement_force = new Vector3(0,0,0);
		Vector3 up = new Vector3(0,1,0);
		Vector3 offset = Transform.Origin - CurrentCamera.GlobalPosition;
		Vector3 outward = offset.Normalized();
		Vector3 right = offset.Cross(up);
		Vector3 forward = up.Cross(right);
		
		if (Input.IsActionPressed("move_right")) {
			movement_force += right;
		}
		if (Input.IsActionPressed("move_left")) {
			movement_force -= right;
		}
		if (Input.IsActionPressed("move_forward")) {
			movement_force += forward;
		}
		if (Input.IsActionPressed("move_backward")) {
			movement_force -= forward;
		}
		float scale = (float)delta*Acceleration;
		ApplyCentralImpulse(movement_force*new Vector3(scale,scale,scale));
		
		if (DebugMovement) {
			VectorReadout.SetVector(GetNode("DebugMovement/Right"),right);
			VectorReadout.SetVector(GetNode("DebugMovement/Up"),up);
			VectorReadout.SetVector(GetNode("DebugMovement/Forward"),forward);
			VectorReadout.SetVector(GetNode("DebugMovement/Force"),movement_force);
		}
		
	}
	
}
