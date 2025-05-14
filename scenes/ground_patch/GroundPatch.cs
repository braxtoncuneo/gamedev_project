using Godot;
using System;
using System.Collections.Generic;


[Tool]
public partial class GroundPatch : StaticBody3D
{

	GroundPatch      _self;
	CollisionShape3D _shape;
	MeshInstance3D   _meshInst;
	ImageTexture     _normalTexture;
	Texture2D        _heightTexture;

	bool             _groundMaterialDirty;
	ShaderMaterial   _groundMaterial;

	bool     _shapeDirty;
	float    _horizontalScale = 1.0f;
	float    _verticalScale   = 1.0f;
	Vector2I _meshResolution  = new Vector2I(32,32);

	[Export]
	public float HorizontalScale {
		get {
			return _horizontalScale;
		}
		set {
			_horizontalScale = value;
			_shapeDirty = true;
		}
	}

	[Export]
	public float VerticalScale {
		get {
			return _verticalScale;
		}
		set {
			_verticalScale = value;
			_shapeDirty = true;
		}
	}

	[Export]
	public Vector2I  MeshResolution {
		get {
			return _meshResolution;
		}
		set {
			_meshResolution = value;
			_shapeDirty = true;
		}
	}

	[Export]
	public Texture2D HeightTexture {
		get {
			return _heightTexture;
		}
		set {
			_shapeDirty = true;
			_heightTexture = value;
		}
	}

	[Export]
	public ShaderMaterial GroundMaterial {
		get {
			return _groundMaterial;
		}
		set {
			_groundMaterialDirty = true;
			_groundMaterial = value;
		}
	}
	
	void ClearOnCopy() {
		if (!Object.ReferenceEquals(this,_self)) {
			_self = this as GroundPatch;
			_shape = null;
			_meshInst = null;
			_normalTexture = null;
			_groundMaterialDirty = true;
		}
	}

	void EnsureShapeKnown() {
		if (_shape is null) {
			_shape = GetNode("CollisionShape3D") as CollisionShape3D;
		}
	}

	void EnsureMeshKnown() {
		if (_meshInst is null) {
			_meshInst = GetNode("MeshInstance3D") as MeshInstance3D;
			//???
			GroundMaterial = _meshInst.GetActiveMaterial(0) as ShaderMaterial;
		}
	}
	
	void EnsureNormalTextureExists() {
		if (_normalTexture is null) {
			_normalTexture = new ImageTexture();
		}
	}
	
	void AttemptShapeUpdate() {
		
		if (_heightTexture is null) {
			return;
		}
		
		if ( ! _shapeDirty ) {
			return;
		}
		
		Image heightImage = null;
		if ( _heightTexture is null ) {
			return;
		}
		heightImage = Image.CreateEmpty(1,1,false,Image.Format.Rf);
		var heightTextureImage = _heightTexture.GetImage();
		if (heightTextureImage is null ) {
			return;
		}
		heightImage.CopyFrom(heightTextureImage);
		heightImage.Convert(Image.Format.Rf);
		if ( heightImage is null  || heightImage.IsEmpty()) {
			return;
		}
		

		// Get shape object from the collision shape
		var heightShape = _shape.GetShape() as HeightMapShape3D;

		// Generate height map and normal map
		var normalImage = new Image();
		normalImage.CopyFrom(heightImage);
		normalImage.BumpMapToNormalMap(_verticalScale/_horizontalScale);
		_normalTexture.SetImage(normalImage);
		heightImage.Resize(_meshResolution.X+1,_meshResolution.Y+1);
		normalImage.Resize(_meshResolution.X+1,_meshResolution.Y+1);

		// Update collision shape
		heightShape.UpdateMapDataFromImage(heightImage,0,_verticalScale/_horizontalScale);
		var shapeScale = new Vector3(
			_horizontalScale,
			_horizontalScale,
			_horizontalScale
		);
		var shapeOffset = new Vector3(
			(float) _meshResolution.X * _horizontalScale / 2.0f,
			0.0f,
			(float) _meshResolution.Y * _horizontalScale / 2.0f
		);
		_shape.Transform = Transform3D.Identity
			.Scaled(shapeScale)
			.Translated(shapeOffset);

		// Generate mesh from maps
		List<Vector3> verts = [];
		List<Vector2> uvs = [];
		List<Vector3> normals = [];

		// Generate vertex data
		for (int y=0; y<=_meshResolution.Y; y++) {
			for (int x=0; x<=_meshResolution.X; x++) {
				var uv = new Vector2(
					(float)x/(float)_meshResolution.X,
					(float)y/(float)_meshResolution.Y
				);
				var vertex = new Vector3(
					_horizontalScale*(float)x,
					_verticalScale*heightImage.GetPixel(x,y).R,
					_horizontalScale*(float)y
				);
				var normal_color = normalImage.GetPixel(x,y);
				var normal = new Vector3(
					normal_color.R,
					normal_color.B,
					normal_color.G
				);
				verts.Add(vertex);
				uvs.Add(uv);
				normals.Add(normal);
			}
		}

		// Generate indexes for vertex data, to use indexed rendering.
		// This allows us to re-use many of our vertexes six times!
		List<int> indices = [];
		for (int y=0; y<_meshResolution.Y; y++) {
			for (int x=0; x<_meshResolution.X; x++) {
				int x0 = x;
				int x1 = x+1;
				int y0 = (_meshResolution.X+1) * y;
				int y1 = (_meshResolution.X+1) * (y+1);
				// Lower-left half of quad
				indices.Add(x0+y0);
				indices.Add(x1+y1);
				indices.Add(x0+y1);
				// Upper-right half of quad
				indices.Add(x0+y0);
				indices.Add(x1+y0);
				indices.Add(x1+y1);
			}
		}

		// Array-of-arrays, used to pass mesh data to Godot
		Godot.Collections.Array surfaceArray = [];
		surfaceArray.Resize((int)Mesh.ArrayType.Max);

		surfaceArray[(int)Mesh.ArrayType.Vertex] = verts.ToArray();
		surfaceArray[(int)Mesh.ArrayType.TexUV]  = uvs.ToArray();
		surfaceArray[(int)Mesh.ArrayType.Normal] = normals.ToArray();
		surfaceArray[(int)Mesh.ArrayType.Index]  = indices.ToArray();

		if (_meshInst.GetMesh() is not ArrayMesh) {
			_meshInst.SetMesh(new ArrayMesh());
		}

		var arrayMesh = _meshInst.GetMesh() as ArrayMesh;
		arrayMesh.ClearSurfaces();
		arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

		// Update shader parameters to match new shape
		//ShaderMat.SetShaderParameter("height_map",heightTexture);
		//ShaderMat.SetShaderParameter("normal_map",NormalTexture);
		//ShaderMat.SetShaderParameter("dimensions",dimensions);
		_shapeDirty = false;
	}
	
	void AttemptMaterialUpdate() {
		if (_groundMaterialDirty && _groundMaterial is not null && _meshInst is not null) {
			_meshInst.GetMesh().SurfaceSetMaterial(0,_groundMaterial);
		}
	}

	void EnsureUpdate() {
		EnsureShapeKnown();
		EnsureMeshKnown();
		EnsureNormalTextureExists();
		AttemptShapeUpdate();
		AttemptMaterialUpdate();
	}

	public override void _Ready () {
		EnsureUpdate();
	}

	public override void _Process (double delta) {
		EnsureUpdate();
	}

}
