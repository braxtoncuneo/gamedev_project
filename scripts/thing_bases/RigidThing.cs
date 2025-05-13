using Godot;
using System;

public partial class RigidThing : RigidBody3D, IThing
{
	
	[Export]
	public ThingSubject Subject { get; set; }
	
	[Export]
	public ThingContent Content { get; set; }
	
	public void RigidThingReady() {
		SetCollisionLayerValue(2,true);
		SetCollisionMaskValue (2,true);
		if (Subject == null) {
			Subject = new ThingSubject();
			Subject.Name = Name;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RigidThingReady();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
