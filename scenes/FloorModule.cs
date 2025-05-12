using Godot;
using System;

public partial class FloorModule : NavigationRegion3D
{
	
	[Export]
	
	
	CollisionShape3D shape;
	MeshInstance3D   meshInst;
	
	public override void _Ready() {
		shape = GetNode("Body/Shape") as CollisionShape3D;
		meshInst = GetNode("Mesh") as MeshInstance3D;
	}
	
}
