using Godot;
using System;


public partial class Player : RigidBody3D
{
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
		Vector3 movement_force = new Vector3(0,0,0);
		if (Input.IsActionPressed("move_right")) {
			movement_force.X += 1;
		}
		if (Input.IsActionPressed("move_left")) {
			movement_force.X -= 1;
		}
		if (Input.IsActionPressed("move_forward")) {
			movement_force.Z -= 1;
		}
		if (Input.IsActionPressed("move_backward")) {
			movement_force.Z += 1;
		}
		float scale = (float)delta*10;
		ApplyCentralImpulse(movement_force*new Vector3(scale,scale,scale));
	}
	
}
