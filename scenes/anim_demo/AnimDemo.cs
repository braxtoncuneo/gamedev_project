using Godot;
using System;

public partial class AnimDemo : CharacterBody3D
{
	
	AnimationNodeStateMachinePlayback stateMachine;
	NavigationAgent3D navAgent;
	AnimationPlayer   animPlayer;
	AnimationTree     animTree;
	Node3D            head;
	
	[Export]
	Node3D            target;
	
	[Export]
	float             speed;
	
	public override void _Ready() {
		navAgent     = GetNode("NavigationAgent3D") as NavigationAgent3D;
		animPlayer   = GetNode("guy/AnimationPlayer")   as AnimationPlayer;
		animTree     = animPlayer.GetNode("AnimationTree")     as AnimationTree;
		head         = FindChild("Head",true,false) as Node3D;
		stateMachine = (AnimationNodeStateMachinePlayback) animTree.Get("parameters/playback");
		stateMachine.Start("Run");
	}
	
	public override void _PhysicsProcess(double delta) {
		Velocity = new Vector3(Velocity.X, 9.8f * (float) delta, Velocity.Z);
		Vector3 nextStep = target.GlobalPosition;
		if (false){
			navAgent.TargetPosition = target.GlobalPosition;
			nextStep = navAgent.GetNextPathPosition();
		}
		Vector3 offset = nextStep - GlobalPosition;
		Vector3 direction = offset.Normalized();
		Velocity = direction * new Vector3(speed,speed,speed);
		MoveAndSlide();
		stateMachine.Travel("Run");
		Transform = Transform.LookingAt(new Vector3(nextStep.X,0,nextStep.Z));
	}
	
}
