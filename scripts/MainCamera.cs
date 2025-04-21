using Godot;
using System;



public partial class MainCamera : Camera3D
{
	[Export]
	public Node3D Target;

	public override void _Ready()
	{
		Target = GetNode("/root/Level").FindChild("Player") as Node3D;
	}

	public override void _Process(double delta)
	{
		Vector3 direction = Target.GlobalPosition - GlobalPosition;
		Transform = Transform.LookingAt(direction);
	}

}
