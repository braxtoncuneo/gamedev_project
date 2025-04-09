using Godot;
using System;

public partial class CameraBody : RigidBody3D
{

	[Export]
	float MaxTether;
	[Export]
	float MinTether;
	[Export]
	float UnderLift;
	[Export]
	float NearLift;
	[Export]
	Node3D   Target;

	Camera3D Camera;

	public override void _Ready() {
		Camera = GetNode<Camera3D>("Camera3D");
	}

	public override void _PhysicsProcess(double delta) {

		// By default, apply no force
		Vector3 force = new Vector3(0,0,0);

		// Determine offset to, direction toward, and distance to target
		Vector3 offset = Target.GlobalPosition - GlobalPosition;
		Vector3 toward = offset.Normalized();
		float dist = offset.Length();

		float coef = (float) delta * 2;
		if (dist < MinTether) {
			coef *= -(float) Math.Pow(MinTether-dist,2);
		} else if (dist > MaxTether) {
			coef *= (float) Math.Pow(dist-MaxTether,2);
		}
		force = toward * new Vector3(coef,coef,coef);
		if (offset.Y > 0) {
			force.Y += offset.Y*UnderLift;
		}
		force.Y += (float) (NearLift/dist);
		ApplyCentralImpulse(force);
	}

}
