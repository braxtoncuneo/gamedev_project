using Godot;
using System;

public class FocusPoint {
	public IThing  Thing { get; set; }
	public Vector3 Point { get; set; }
	
	public FocusPoint(IThing thing, Vector3 point) {
		Thing = thing;
		Point = point;
	}
	
	public static FocusPoint AttemptToSpot(Node3D fromNode, Node3D toNode, uint layer) {
		PhysicsDirectSpaceState3D spaceState = fromNode.GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
			fromNode.GlobalPosition,
			toNode.GlobalPosition,
			layer
		);
		Godot.Collections.Dictionary result = spaceState.IntersectRay(query);
		if (!result.ContainsKey("collider")) {
			return null;
		} else {
			IThing  thing = result["collider"].As<Node3D>() as IThing;
			Vector3 point = result["position"].As<Vector3>();
			if (thing is not null) {
				return new FocusPoint(thing,point);
			} else {
				return null;
			}
		}
	}
	
};
