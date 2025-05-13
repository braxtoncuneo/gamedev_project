using Godot;
using System;

public partial class CameraBody : RigidBody3D
{

	[Export]
	public float MaxTether;

	[Export]
	public float MinTether;

	[Export]
	public float UnderLift;

	[Export]
	public float NearPush;

	[Export]
	public float Approach = 1;
	
	[Export]
	public float HorizontalDamp = 0.1f;

	[Export]
	Node3D   Target;

	Camera3D _camera;

	public override void _Ready() {
		_camera = GetNode<Camera3D>("Camera3D");
	}

	public override void _PhysicsProcess(double delta) {

		// By default, apply no force
		Vector3 force = new Vector3(0,0,0);

		// Determine offset to, direction toward, and distance to target
		Vector3 offset = Target.GlobalPosition - GlobalPosition;
		Vector3 toward = offset.Normalized();
		float dist = offset.Length();
		float speedToward = toward.Dot(LinearVelocity);

		float targetSpeed = speedToward;
		if (dist < MinTether) {
			targetSpeed = (dist-MinTether) * Approach;
		} else if (dist > MaxTether) {
			targetSpeed = (dist-MaxTether) * Approach;
		}
		float correction = targetSpeed - speedToward;
		force = toward * new Vector3(correction,correction,correction);
		if (offset.Y > 0) {
			force.Y += offset.Y*UnderLift;
		}
		Vector3 horizOffset = new Vector3(offset.X,0.0f,offset.Z);
		float horizDist = horizOffset.Length();
		float nearPush = (float) (NearPush/horizDist);
		force -= horizOffset.Normalized() * new Vector3(nearPush,nearPush,nearPush);
		
		Vector3 up = new Vector3(0,1,0);
		Vector3 horizontalDirection = toward.Cross(up);
		
		Vector3 horizontalSpeed = LinearVelocity.Project(horizontalDirection);
		float damp = 1.0f - (float)Math.Pow(HorizontalDamp,(float)delta);
		Vector3 horizontalDampScale = new Vector3 (damp,damp,damp);
		force -= horizontalSpeed * horizontalDampScale;
		
		Vector3 verticalSpeed = LinearVelocity.Project(up);
		damp = 1.0f - (float)Math.Pow(HorizontalDamp,(float)delta);
		force -= verticalSpeed * horizontalDampScale;
		
		ApplyCentralImpulse(force);
	}

}
