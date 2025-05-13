using Godot;
using System;
using System.Collections.Generic;

public partial class Player : RigidThing
{
	
	[Export]
	float StepSize = 1.0f;
	
	[Export]
	float StandStillThreshold = 0.5f;

	Camera3D currentCamera;
	
	[Export]
	AudioStream StepStream;
	
	[Export]
	AudioStream BackgroundMusicStream;

	[Export]
	float Acceleration = 4.0f;
	
	[Export]
	Node3D MortalEnemy;
	
	NavigationAgent3D   navAgent;
	AudioStreamPlayer   BackgroundMusicPlayer;
	Vector3 lastStepPosition;
	
	Sprite3D sprite;
	bool step_phase;
	
	AtlasTexture spriteAtlas;

	bool debugMovement;

	[Export]
	bool DebugMovement {
		get { return debugMovement; }
		set {
			var node = GetNode("DebugMovement") as Node3D;
			debugMovement = value;
			if (node is null) {
				return;
			} else if (value) {
				node.Show();
			} else {
				node.Hide();
			}
		}
	}
	
	void PlayBackgroundMusic() {
		BackgroundMusicPlayer.Play();
	}
	
	void InitAudio() {
		lastStepPosition = GlobalPosition;
		
		BackgroundMusicPlayer = GetNode("BackgroundMusicPlayer") as AudioStreamPlayer;
		if (BackgroundMusicStream is not null){
			BackgroundMusicPlayer.Stream = BackgroundMusicStream;
			BackgroundMusicPlayer.Finished += PlayBackgroundMusic;
			BackgroundMusicPlayer.Play();
		}
	}
	
	void InitDebug() {
		Vector4 red   = new Vector4(1,0,0,1);
		Vector4 green = new Vector4(0,1,0,1);
		Vector4 blue  = new Vector4(0,0,1,1);
		Vector4 pink  = new Vector4(1,0,1,1);
		VectorReadout.SetColor(GetNode("DebugMovement/Right"),  red);
		VectorReadout.SetColor(GetNode("DebugMovement/Up"),     green);
		VectorReadout.SetColor(GetNode("DebugMovement/Forward"),blue);
		VectorReadout.SetColor(GetNode("DebugMovement/Force"),  pink);
		DebugMovement = DebugMovement;
	}
	
	void InitSprites() {
		var img = GD.Load("res://textures/player_sprites/player_atlas.png") as Image;
		var imgTex = new ImageTexture();
		imgTex.SetImage(img);
		spriteAtlas = new AtlasTexture();
		spriteAtlas.Atlas = imgTex;
		spriteAtlas.Region = new Rect2(0,0,128,128);
		sprite = GetNode("Sprite3D") as Sprite3D;
		sprite.Texture = spriteAtlas;
		step_phase = false;
	}
	
	void InitNavigation() {
		navAgent = GetNode("NavigationAgent3D") as NavigationAgent3D;
	}

	public override void _Ready()
	{
		RigidThingReady();
		currentCamera = GetViewport().GetCamera3D();
		InitDebug();
		InitAudio();
		InitSprites();
		InitNavigation();
	}
	
	
	private Vector3 CalculatemovementForce() {
				// Force vector to accumulate into
		Vector3 movementForce = new Vector3(0,0,0);
		
		// Determine unit vectors to act as the basis for our space
		Vector3 up = new Vector3(0,1,0);
		Vector3 offset = Transform.Origin - currentCamera.GlobalPosition;
		Vector3 outward = offset.Normalized();
		Vector3 right = offset.Cross(up);
		Vector3 forward = up.Cross(right);

		// Accumulate "bumps" into our force vector depending upon player inputs
		if (Input.IsActionPressed("move_right")) {
			movementForce += right;
		}
		if (Input.IsActionPressed("move_left")) {
			movementForce -= right;
		}
		if (Input.IsActionPressed("move_forward")) {
			movementForce += forward;
		}
		if (Input.IsActionPressed("move_backward")) {
			movementForce -= forward;
		}
		
		if (Input.IsActionPressed("auto_route")) {
			navAgent.TargetPosition = MortalEnemy.GlobalPosition;
			var nextStep = navAgent.GetNextPathPosition();
			var offsetToNextStep = nextStep - GlobalPosition;
			var dir = offsetToNextStep.Normalized();
			movementForce = dir;
		}
		
		return movementForce.Normalized();
		
	}	

	public override void _Process(double delta)
	{
		
		// Determine unit vectors to act as the basis for our space
		Vector3 up = new Vector3(0,1,0);
		Vector3 offset = Transform.Origin - currentCamera.GlobalPosition;
		Vector3 outward = offset.Normalized();
		Vector3 right = offset.Cross(up);
		Vector3 forward = up.Cross(right);

		Vector3 movementForce = CalculatemovementForce();
		// Apply force, scaled by the delta and the player's accelleration
		float alignment = LinearVelocity.Normalized().Dot(movementForce);
		float scale = (float)delta*Acceleration;
		scale /= (alignment*0.5f+1.0f);
		ApplyCentralImpulse(movementForce*new Vector3(scale,scale,scale));

		// If we are debugging movement, update the state of the vectors
		if (DebugMovement) {
			VectorReadout.SetVector(GetNode("DebugMovement/Right"),right);
			VectorReadout.SetVector(GetNode("DebugMovement/Up"),up);
			VectorReadout.SetVector(GetNode("DebugMovement/Forward"),forward);
			VectorReadout.SetVector(GetNode("DebugMovement/Force"),movementForce);
		}
		
		// Determine direction of movement, relative to the camera
		float relativeRightness   = LinearVelocity.Dot(right);
		float relativeForwardness = LinearVelocity.Dot(forward);
		// Calculate angle of movement, relative to camera
		float angleOfTravel       = (float)Math.Atan2(relativeRightness,relativeForwardness);
		// Update sprite to match orientation
		UpdateSpriteOrientation(angleOfTravel);
		
		// Offset sprite atlas location to the "still" row if the player
		// is not moving particularly fast
		if (LinearVelocity.Length() < StepSize*StandStillThreshold) {
			var orientation = spriteAtlas.Region.Position.X;
			spriteAtlas.Region = new Rect2(orientation,0,128,128);
		}
		
		// If the player has traveled 1 unit (an arbitrary step length),
		// then we will trigger a step sound and swap out the current sprite
		if ((lastStepPosition-GlobalPosition).Length() > StepSize) {
			var stepPlayer = new AudioStreamPlayer3D();
			AddChild(stepPlayer);
			stepPlayer.SetName("StepPlayer");
			stepPlayer.SetOwner(this);
			stepPlayer.Stream = StepStream;
			stepPlayer.Play();
			lastStepPosition = GlobalPosition;
			AnimateStep();
		}
		
		// Clean up any AudioStreamPlayers which have completed their sound
		foreach (Node n in FindChildren("StepPlayer")) {
			if (n is not AudioStreamPlayer3D) {
				continue;
			}
			var player = n as AudioStreamPlayer3D;
			if (! player.Playing) {
				RemoveChild(player);
				player.Free();
			}
		}

	}
	
	void UpdateSpriteOrientation(float angle) {
		int column = (int)((2.0f*angle/Math.PI)+2.5f)%4;
		var phase = spriteAtlas.Region.Position.Y;
		spriteAtlas.Region = new Rect2(column*128,phase,128,128);
	}
	
	void AnimateStep() {
		// Depending upon the step phase, have either the left or the
		// right leg forward (each phase has its own row in the atlas)
		var orientation = spriteAtlas.Region.Position.X;
		if (step_phase) {
			spriteAtlas.Region = new Rect2(orientation,128,128,128);
		} else {
			spriteAtlas.Region = new Rect2(orientation,256,128,128);
		}
		step_phase = !step_phase;
	}

}
