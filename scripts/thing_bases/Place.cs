using Godot;
using System;

public partial class Place : Area3D, IThing
{

	public ThingSubject Subject { get; set; } = null;
	public ThingContent Content { get; set; } = null;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
