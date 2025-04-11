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
	float NearPush;
	[Export]
	float Approach = 1;
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
		float speed_toward = toward.Dot(LinearVelocity);

		float target_speed = speed_toward;
		if (dist < MinTether) {
			target_speed = (dist-MinTether) * Approach;
		} else if (dist > MaxTether) {
			target_speed = (dist-MaxTether) * Approach;
		}
		float correction = target_speed - speed_toward;
		force = toward * new Vector3(correction,correction,correction);
		if (offset.Y > 0) {
			force.Y += offset.Y*UnderLift;
		}
		float near_push = (float) (NearPush/(dist*dist));
		force -= toward.Normalized() * new Vector3(near_push,near_push,near_push);
		ApplyCentralImpulse(force);
	}

}
