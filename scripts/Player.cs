using Godot;
using System;


public partial class Player : RigidBody3D
{
	bool MoveForward;
	bool MoveBackward;
	bool MoveLeft;
	bool MoveRight;

	public override void _Ready()
	{
		MoveForward = false;
		MoveBackward = false;
		MoveRight = false;
		MoveLeft = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsAction("move_right"))
		{
			MoveRight = @event.IsPressed();
		} else if (@event.IsAction("move_left")) {
			MoveLeft = @event.IsPressed();
		} else if (@event.IsAction("move_forward")) {
			MoveForward= @event.IsPressed();
		} else if (@event.IsAction("move_backward")) {
			MoveBackward = @event.IsPressed();
		}
	}

	public override void _Process(double delta)
	{
		Vector3 movement_force = new Vector3(0,0,0);
		if (MoveRight) {
			movement_force.X += 1;
		}
		if (MoveLeft) {
			movement_force.X -= 1;
		}
		if (MoveForward) {
			movement_force.Z -= 1;
		}
		if (MoveBackward) {
			movement_force.Z += 1;
		}
		float scale = (float)delta*10;
		ApplyCentralImpulse(movement_force*new Vector3(scale,scale,scale));
	}

}
