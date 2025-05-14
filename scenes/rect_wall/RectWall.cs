using Godot;
using System;

public partial class RectWall : StaticBody3D
{
	CollisionShape3D _collisionShape;
	MeshInstance3D   _meshInstance;
	Vector3 _scale;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_scale = Scale;
		Scale = new Vector3(1,1,1);
		_meshInstance = FindChild("MeshInstance3D") as MeshInstance3D;
		_meshInstance.Scale = new Vector3(_scale.X,_scale.Y,_scale.Z);
		_collisionShape = FindChild("CollisionShape3D") as CollisionShape3D;
		BoxShape3D shape = new BoxShape3D();
		shape.Size = new Vector3(_scale.X,_scale.Y,_scale.Z);
		_collisionShape.Shape = shape;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
