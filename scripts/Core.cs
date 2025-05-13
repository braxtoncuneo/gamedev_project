using Godot;
using System;


public partial class Core : RigidThing
{
	
	public static bool AlwaysChase;
	
	MeshInstance3D coreMesh;
	NavigationAgent3D navAgent;
	Node3D playerRoot;
	
	FocusPoint focus;
	
	Vector3 lastFacing = new Vector3(1,0,0);
	double  averageFacingAngularVelocity = 0.0;
	
	[Export]
	public float Acceleration = 4.0f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RigidThingReady();
		coreMesh  = GetNode("./CoreMesh") as MeshInstance3D;
		navAgent  = GetNode("./NavigationAgent3D") as NavigationAgent3D;
		playerRoot = GetTree()
			.GetRoot()
			.FindChild("Player",true,false)
			as Node3D;
	}
	
	public Vector3 CalculateMovementForce(double delta)
	{
		// If there is nothing to chase, slow down, otherwise
		// navigate to target.
		if ( navAgent.IsNavigationFinished() ) {
			return - LinearVelocity.Normalized() * Acceleration;
		} else {
			Vector3 nextPoint = navAgent.GetNextPathPosition();
			Vector3 offset = nextPoint - GlobalPosition;
			return offset.Normalized() * Acceleration;
		}
	}
	
	
	public void UpdateFacing(Vector3 facing, double delta) {
		bool exactly_up    = (Math.Abs(facing.Dot(new Vector3(0,1,0))) > 0.999);
		if (exactly_up) {
			return;
		}
		ShaderMaterial shader = coreMesh.GetActiveMaterial(0) as ShaderMaterial;
		Transform3D facing_transform = (new Transform3D()).LookingAt(facing);
		shader.SetShaderParameter("facing",facing_transform);
		double dot_product  = Math.Min(1.0,facing.Dot(lastFacing));
		double angle_change = Math.Acos(dot_product);
		double angular_velocity = angle_change / delta;
		averageFacingAngularVelocity = averageFacingAngularVelocity*0.6 + angular_velocity*0.4;
		shader.SetShaderParameter("facing_angular_velocity",averageFacingAngularVelocity);
		lastFacing = facing;
	}
	
	public void FaceTowardsMovement(double delta) {
		bool moving_substantially = (LinearVelocity.Length() > 0.001);
		if (moving_substantially) {
			UpdateFacing(LinearVelocity.Normalized(),delta);
		}
	}
	
	public void FaceTowardsFocus(double delta) {
		if ( focus != null ) {
			Vector3 point = focus.Point;
			Vector3 facing_towards_focus = (point - Transform.Origin).Normalized();
			UpdateFacing(facing_towards_focus,delta);
		}
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Try to spot the player
		focus = FocusPoint.AttemptToSpot(this,playerRoot,3);
		if (focus is not null) {
			navAgent.TargetPosition = focus.Point;
		}
		
		// Try to move toward player (if applicable)
		Vector3 movement_force = CalculateMovementForce(delta);
		float scale = (float)delta;
		ApplyCentralImpulse(movement_force*new Vector3(scale,scale,scale));
		
		// Change where the character is facing, based upon player visibility
		if (focus is not null) {
			FaceTowardsFocus(delta);
		} else {
			FaceTowardsMovement(delta);
		}
	}

	
}
