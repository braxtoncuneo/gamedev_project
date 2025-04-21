using Godot;
using System;

public partial class VectorReadout : Node3D
{
	Vector4 _color;

	[Export]
	public Vector4 Color {
		get { return _color; }
		set {
			_color=value;
			if (_vectorMaterial is not null){
				_vectorMaterial.SetShaderParameter("color",_color);
			}
		}
	}

	[Export]
	public Vector3 Vector;

	ShaderMaterial _vectorMaterial;
	ShaderMaterial _rodMaterial;

	public override void _Ready () {
		var cone = GetNode("VectorCone") as MeshInstance3D;
		var rod  = GetNode("VectorRod") as MeshInstance3D;
		_vectorMaterial = GD.Load("res://materials/flat_color.tres")
			.Duplicate() as ShaderMaterial;
		cone.SetSurfaceOverrideMaterial(0,_vectorMaterial);
		rod .SetSurfaceOverrideMaterial(0,_vectorMaterial);
		Color = Color;
	}

	public override void _Process(double delta) {
		Node3D parent = GetParent() as Node3D;
		Vector3 up = new Vector3(0,1,0);
		if (Vector.Length() < 0.00001) {
			Hide();
			return;
		} else {
			Show();
		}

		if (up.Dot(Vector) > 0.9) {
			up = new Vector3(1,0,0);
		}
		Transform = Transform3D.Identity
			.LookingAt(Vector,up)
			.Translated(parent.GlobalPosition);
	}

	public static void SetVector(Node readout, Vector3 vector) {
		(readout as VectorReadout).Vector = vector;
	}

	public static void SetColor(Node readout, Vector4 color) {
		(readout as VectorReadout).Color = color;
	}

}
