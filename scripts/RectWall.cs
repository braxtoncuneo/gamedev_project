using Godot;
using System;

public partial class RectWall : StaticBody3D
{
	CollisionShape3D CollisionShape;
	MeshInstance3D   MeshInstance;
	Vector3 scale;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scale = Scale;
		Scale = new Vector3(1,1,1);
		MeshInstance = FindChild("MeshInstance3D") as MeshInstance3D;
		MeshInstance.Scale = new Vector3(scale.X,scale.Y,scale.Z);
		CollisionShape = FindChild("CollisionShape3D") as CollisionShape3D;
		BoxShape3D shape = new BoxShape3D();
		shape.Size = new Vector3(scale.X,scale.Y,scale.Z);
		CollisionShape.Shape = shape;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
