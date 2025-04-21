using Godot;
using System;

[Tool]
public partial class ShaderTexture2d : MeshInstance3D
{

	SubViewport    _subViewInst;
	ColorRect      _colorRectInst;

	ShaderTexture2d _self = null;
	bool _updateModeDirty = true;
	SubViewport.UpdateMode _renderTargetUpdateMode;

	bool _shaderMatDirty;
	ShaderMaterial _shaderMat;

	[Export]
	public ShaderMaterial ShaderMat {
		get {
			return _shaderMat;
		}
		set {
			_shaderMatDirty = true;
			_shaderMat = value as ShaderMaterial;
		}
	}

	[Export]
	public SubViewport.UpdateMode RenderTargetUpdateMode {
		get {
			return _renderTargetUpdateMode;
		}
		set {
			_updateModeDirty = true;
			_renderTargetUpdateMode = value;
		}
	}


	void EnsureUpdate() {
		if (!Object.ReferenceEquals(this,_self)) {
			_subViewInst = null;
			_colorRectInst = null;
			_updateModeDirty = true;
			_self = this;
		}
		if (_subViewInst is null) {
			_subViewInst   = GetNode("SubViewport") as SubViewport;
		}
		if (_updateModeDirty && _subViewInst is not null) {
			_subViewInst.RenderTargetUpdateMode = _renderTargetUpdateMode;
			_updateModeDirty = false;
		}
		if (_colorRectInst is null){
			_colorRectInst = GetNode("SubViewport/ColorRect") as ColorRect;
		}
		if (_shaderMatDirty && _colorRectInst is not null) {
			_colorRectInst.SetMaterial(ShaderMat as ShaderMaterial);
			var mat = new StandardMaterial3D();
			mat.AlbedoTexture = _subViewInst.GetTexture();
			SetSurfaceOverrideMaterial(0,mat);
			_shaderMatDirty = false;
		}
	}

	public override void _Ready() {
		EnsureUpdate();
	}

	public override void _Process(double delta) {
		EnsureUpdate();
	}

}
