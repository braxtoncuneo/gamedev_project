using Godot;
using System;


public partial class Player : RigidBody3D
{
	
	Camera3D CurrentCamera;
	
	public override void _Ready()
	{
		CurrentCamera = GetViewport().GetCamera3D();
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
		float scale = (float)delta*10;
		ApplyCentralImpulse(movement_force*new Vector3(scale,scale,scale));
	}
	
}
