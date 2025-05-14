using Godot;
using System;

public partial class MusicBox : RigidBody3D
{
	
	[Export]
	AudioStream Stream;
	
	AudioStreamPlayer3D StreamPlayer;
	
	void BeginPlaying() {
		StreamPlayer.Play();
	}
	
	public override void _Ready() {
		StreamPlayer = GetNode("AudioStreamPlayer3D") as AudioStreamPlayer3D;
		StreamPlayer.SetStream(Stream);
		StreamPlayer.Finished += BeginPlaying;
		BeginPlaying();
	}
	
}
