using Godot;
using System;
using System.Collections.Generic;



public class Branch
{
	static Mesh branchMesh;
	
	public RandomNumberGenerator rng;
	public uint    depth;
	public float   scale;
	public Vector3 direction;
	
	
	public Branch (uint seed,uint depth) {
		rng = new RandomNumberGenerator();
		rng.SetSeed((ulong)seed);
		direction = new Vector3(1.0f,0.0f,0.0f);
		scale = 0.5f;
		this.depth = depth;
	}
	
	
	public bool GrowBranch(float growthMin,float growthMax,float scaleMax) {
		if (scale < scaleMax) {
			scale += Math.Min(rng.RandfRange(growthMin,growthMax),scaleMax);
		}
		return (scale < scaleMax);
	}
	
	
	private void EnsureMeshInit() {
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
	}
	
	private void AddChildren(IEnumerable<Node3D> children,Transform3D tform) {
		foreach (var child in children) {
			AddChild(child);
			child.SetOwner(this);
			child.Transform = tform * child.Transform;
		}
	}
	
	private CollisionShape3D CreateShape() {
		var collisionShape = new CollisionShape3D();
		var capsuleShape = new CapsuleShape3D();
		capsuleShape.Height = scale;
		capsuleShape.Radius = radius;
		collisionShape.SetShape(capsuleShape);
	}
	
	public Node3D ReifyBranch (IEnumerable<Node3D> children) {
		
		// Make sure the branch mesh is known and available
		EnsureMeshInit();
		
		// Construct nodes for the composition of this plant component
		var root = new Node3D();
		var mesh = new MeshInstance3D();
		var body = new StaticBody3D();
		var collisionShape = CreateShape();
		body.AddChild(collisionShape);
		
		mesh.SetMesh(branchMesh);
		
		// Calculate transform which offsets the distance between the center
		// of the branch and the center of the tip of the branch (accounting
		// for the radius of the underlying capsule shape)
		float radiusCoeff = 1.0f / (16.0f*(depth+1));
		float radius = scale * radiusCoeff;
		float tipOffsetScale = 0.5f-radiusCoeff;
		Vector3 tipOffset = new Vector3(0.0f,scale*tipOffsetScale,0.0f);
		Transform3D offsetTform = mesh.Transform.TranslatedLocal(tipOffset);
		
		// Calculate rotation transform by making a transform "look at"
		// the direction of the branch
		Transform3D rotateTform = Transform3D.Identity;
		if(direction.Dot(new Vector3(0.0f,1.0f,0.0f)) < 0.99) {
			rotateTform = Transform3D.Identity.LookingAt(direction);
		} else {
			rotateTform = Transform3D.Identity.LookingAt(
				direction,
				new Vector3(1.0f,0.0f,0.0f)
			);
		}
		
		// Scaling affects all branch dimensions equally
		Vector3 scaleVec = new Vector3(scale,scale,scale);
		var scaleTform = Transform3D.Identity.Scaled(scaleVec);
		
		// The capsule mesh is pre-scaled, but the mesh is not (for re-use
		// under the flyweight pattern), so they have different transforms.
		mesh.Transform = rotateTform * offsetTform * scaleTform * mesh.Transform;
		body.Transform = rotateTform * offsetTform * mesh.Transform;

		// Assemble the final composition
		root.AddChild(body);
		root.AddChild(mesh);
		AddChildren(children,rotateTform * offsetTform * offsetTform);
		
		return root;
	}
	
}
