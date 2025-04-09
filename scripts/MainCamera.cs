using Godot;
using System;



public partial class MainCamera : Camera3D
{
	[Export]
	Node3D Target;
	
	public override void _Ready() {
		Target = GetNode("/root/Level/Player") as Node3D;
	}
	
	public override void _Process(double delta) {
		Vector3 direction = Target.Transform.Origin - GlobalPosition;
		Transform = Transform.LookingAt(direction);
	}
	
}
