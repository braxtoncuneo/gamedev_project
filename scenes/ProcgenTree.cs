using Godot;
using System;
using System.Collections.Generic;

public partial class ProcgenTree : Node3D
{
	
	partial class TreeComponent : MeshInstance3D {
		
		List<TreeComponent> children;
		
		RandomNumberGenerator rng;
		
		bool    isBranch;
		uint     depth;
		float   scale;
		Vector3 direction;
		
		public TreeComponent(uint seed,uint depth) {
			rng = new RandomNumberGenerator();
			rng.SetSeed((ulong)seed);
			direction = new Vector3(0.0f,1.0f,0.0f);
			scale = 0.1f;
			this.depth = depth;
			isBranch = false;
			children = new List<TreeComponent>();
		}
		
		public void Grow() {
			foreach (var child in children) {
				child.Grow();
			}
			if (scale > 1.0f) {
				isBranch = true;
				int childCount = rng.RandiRange(2,4);
				for (int i=0; i<childCount; i++) {
					children.Add(new TreeComponent(rng.Randi(),depth+1));
				}
			}
			if (!isBranch) {
				direction.X += rng.RandfRange(-0.2f,0.2f)*(float)depth;
				direction.Y += rng.RandfRange(-0.2f,0.2f)*(float)depth;
				direction.Z += rng.RandfRange(-0.2f,0.2f)*(float)depth;
				direction = direction.Normalized();
			}
			scale += rng.RandfRange(0.0f,0.5f);
		}
		
		public void Reify () {
			//var resultBody = new RigidBody3D();
			//var resultShape = new CollisionShape3D();
			//resultMesh.AddChild(resultBody);
			//resultBody.SetOwner(resultMesh);
			//resultBody.AddChild(resultShape);
			//resultShape.SetOwner(resultBody);
			var capsuleMesh = new CapsuleMesh();
			if (isBranch) {
				capsuleMesh.Height = scale;
				capsuleMesh.Radius = scale/16.0f;
			} else {
				scale *= 0.1f;
				capsuleMesh.Height = scale;
				capsuleMesh.Radius = scale;
			}
			SetMesh(capsuleMesh);
			
			Vector3 tipOffset = new Vector3(0.0f,scale/2.0f,0.0f);
			Transform = Transform.Translated(tipOffset);
			if(direction.Dot(new Vector3(0.0f,1.0f,0.0f)) < 0.99) {
				Transform = Transform * Transform3D.Identity.LookingAt(direction);
			}
			
			foreach (var child in children) {
				child.Reify();
				GD.Print("CHOILD");
				AddChild(child);
				child.SetOwner(this);
				child.Transform = child.Transform.Translated(tipOffset);
			}
		}
	};
	
	
	TreeComponent trunk;
	
	
	public override void _Ready() {
		
		trunk = new TreeComponent((uint)GD.Hash(GlobalPosition),0);
		AddChild(trunk);
		trunk.SetOwner(this);
		
		for(int i=0; i<10; i++) {
			trunk.Grow();
		}
		trunk.Reify();
		
	}
	
}
