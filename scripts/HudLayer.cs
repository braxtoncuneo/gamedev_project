using Godot;
using System;

[GlobalClass]
public partial class HudLayer : CanvasLayer
{
	
	public const float INSPECT_DISTANCE = 65536;
	
	public FocusPoint Focus;
	
	bool          initialized;
	Camera3D      camera;
	RichTextLabel inspect;
	Panel         reticle;
	
	private bool AttemptInitialize() {
		Input.MouseMode = Input.MouseModeEnum.Hidden;
		var follow = GetTree()
			.GetRoot()
			.FindChild("FollowCamera",true,false)
			as Node;
		
		if (follow is not null) {
			camera  = follow.FindChild("Camera3D",true,false) as Camera3D;
			return (camera is not null);
		}
		return false;
	}
	
	public override void _Ready()
	{
		inspect = GetNode("./Inspect") as RichTextLabel;
		reticle = GetNode("./Reticle") as Panel;
	}

	public override void _Process(double delta)
	{
	}
	
	void UpdateFocus() {
		PhysicsDirectSpaceState3D spaceState = camera.GetWorld3D().DirectSpaceState;
		Vector2 mousePosition = GetViewport().GetMousePosition();
		Vector3 origin = camera.ProjectRayOrigin(mousePosition);
		Vector3 inspect_scale = new Vector3(
			INSPECT_DISTANCE,
			INSPECT_DISTANCE,
			INSPECT_DISTANCE
		);
		Vector3 end = origin + camera.ProjectRayNormal(mousePosition) * inspect_scale;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
			origin, end, ThingSubject.INSPECTION_LAYER
		);
		Godot.Collections.Dictionary result = spaceState.IntersectRay(query);
		if (!result.ContainsKey("collider")) {
			Focus = null;
		} else {
			IThing  thing = result["collider"].As<Node3D>() as IThing;
			Vector3 point = result["position"].As<Vector3>();
			Focus  = new FocusPoint(thing,point);
		}
	}
	
	void SetInspectText(String text) {
		//inspect.Text = "[center]"+text+"[/center]";
		inspect.Text = text;
	}
	
	void UpdateMouseReadout () {
		Vector2 mousePosition = GetViewport().GetMousePosition();
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		reticle.Position = mousePosition;
		inspect.Position = mousePosition-(mousePosition/viewportSize)*inspect.Size;
		if ( (Focus != null) && (Focus.Thing != null )&& (Focus.Thing.Subject != null) ) {
			SetInspectText(Focus.Thing.Subject.Name);
		} else {
			SetInspectText("???");
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (!initialized) {
			initialized = AttemptInitialize();
		}
		if (initialized) {
			UpdateFocus();
			UpdateMouseReadout();
		}
	}
}
