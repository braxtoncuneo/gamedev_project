using Godot;
using System;
using System.Collections.Generic;

public partial class ProcGenTree : Node3D
{
	
	partial class TreeComponent : Node3D {
		
		List<TreeComponent> children;
		
		static Mesh branchMesh;
		static Mesh foliageMesh;
		
		RandomNumberGenerator rng;
		
		bool    isBranch;
		uint    depth;
		float   scale;
		Vector3 direction;
		
		MeshInstance3D mesh;
		
		public TreeComponent(uint seed,uint depth) {
			rng = new RandomNumberGenerator();
			rng.SetSeed((ulong)seed);
			direction = new Vector3(1.0f,0.0f,0.0f);
			scale = 0.5f;
			this.depth = depth;
			isBranch = false;
			children = new List<TreeComponent>();
		}
		
		public void Grow() {
			foreach (var child in children) {
				child.Grow();
				scale = Math.Max(scale,child.scale);
			}
			if ( (scale > 1.0f) && (!isBranch) && (depth<4)) {
				isBranch = true;
				int childCount = rng.RandiRange(2,4);
				for (int i=0; i<childCount; i++) {
					children.Add(new TreeComponent(rng.Randi(),depth+1));
				}
			}
			if (!isBranch) {
				float bump = 0.4f*depth;
				direction.X += rng.RandfRange(-bump,bump);
				direction.Y += rng.RandfRange(-bump,bump);
				direction.Z += rng.RandfRange(-bump,bump);
				direction = direction.Normalized();
			}
			if (scale < 1.2f) {
				scale += rng.RandfRange(0.0f,0.5f);
			}
		}
		
		public void Reify () {
		
			if (branchMesh is null) {
				var branch = new CapsuleMesh();
				branch.Height = 1.0f;
				branch.Radius = 1.0f/16.0f;
				branch.RadialSegments = 16;
				branch.Rings = 4;
				branchMesh = branch;
				var mat = new StandardMaterial3D();
				mat.AlbedoColor = new Color(0.4f,0.2f,0.1f,1.0f);
				branchMesh.SurfaceSetMaterial(0,mat);
			}
			
			if (foliageMesh is null) {
				var foliage = new CapsuleMesh();
				foliage.Height = 1.0f;
				foliage.Radius = 1.0f;
				foliage.RadialSegments = 16;
				foliage.Rings = 4;
				foliageMesh = foliage;
				var mat = new StandardMaterial3D();
				mat.AlbedoColor = new Color(0.3f,0.4f,0.1f,1.0f);
				foliageMesh.SurfaceSetMaterial(0,mat);
			}
				
			var mesh = new MeshInstance3D();
			var body = new StaticBody3D();
			var collisionShape = new CollisionShape3D();
			var capsuleShape = new CapsuleShape3D();
			body.AddChild(collisionShape);
			collisionShape.SetShape(capsuleShape);
			
			Mesh capsuleMesh = null;
			float radius = scale;
			if (isBranch) {
				capsuleMesh = branchMesh;
				radius /= (16.0f*(depth+1));
			} else {
				capsuleMesh = foliageMesh;
			}
			capsuleShape.Height = scale;
			capsuleShape.Radius = radius;
			mesh.SetMesh(capsuleMesh);
			
			float tipOffsetScale = 0.5f-1.0f/16.0f;
			Vector3 tipOffset = new Vector3(0.0f,scale*tipOffsetScale,0.0f);
			var offsetTform = mesh.Transform.TranslatedLocal(tipOffset);
			Transform3D rotateTform = Transform3D.Identity;
			if(direction.Dot(new Vector3(0.0f,1.0f,0.0f)) < 0.99) {
				rotateTform = Transform3D.Identity.LookingAt(direction);
			} else {
				rotateTform = Transform3D.Identity.LookingAt(
					direction,
					new Vector3(1.0f,0.0f,0.0f)
				);
			}
			AddChild(body);
			AddChild(mesh);
			mesh.SetOwner(this);
			body.SetOwner(this);
			Vector3 scaleVec = new Vector3(scale,scale,scale);
			var scaleTform = Transform3D.Identity.Scaled(scaleVec);
			
			mesh.Transform = rotateTform * offsetTform * scaleTform * mesh.Transform;
			body.Transform = rotateTform * offsetTform * mesh.Transform;
			
			foreach (var child in children) {
				child.Reify();
				AddChild(child);
				child.SetOwner(this);
				child.Transform = rotateTform * offsetTform * offsetTform * child.Transform;
			}
		}
	};
	
	
	TreeComponent trunk;
	
	
	public override void _Ready() {
		
		trunk = new TreeComponent((uint)GD.Hash(GlobalPosition),0);
		AddChild(trunk);
		trunk.SetOwner(this);
		
		for(int i=0; i < 15; i++) {
			trunk.Grow();
		}
		trunk.Reify();
		
	}
	
}
