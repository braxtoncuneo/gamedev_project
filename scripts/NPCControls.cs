using Godot;
using System;

public partial class NPCControls : Control
{
	
	Button alwaysChase;
	
	private void SetAlwaysChase(bool toggle) {
		Core.AlwaysChase = toggle;
	}
	
	public override void _Ready() {
		alwaysChase = GetNode("AlwaysChase") as Button;
		alwaysChase.Toggled += SetAlwaysChase;
	}
}
