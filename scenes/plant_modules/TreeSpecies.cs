using Godot;
using System;
using System.Collections.Generic;



public class TreeSpecies
{

	public bool Grow (ProcGen.ITreeCell<List<int>,Object> cell) {
		foreach (var child in cell.Children()) {
			Grow(child);
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
	
	
	
	public Node3D Reify (ProcGen.ITreeCell<List<int>,Object> cell) {
		if (cell.Data is Branch) {
			List<Node3D> childNodes;
			foreach (var child in cell.Children()) {
				childNodes.Add(Reify(child));
			}
			return (cell as Branch).ReifyBranch(childNodes);
		} else {
			return (cell as Foliage).ReifyFoliage();
		}
		
	}
	
}
