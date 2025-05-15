using Godot;
using System;
using ProcGen;

public partial class ProcGenMaze : Node3D
{
	
	
	Grid2D<Node3D> grid;
	
	PackedScene wall;
	PackedScene floor;
	PackedScene tree;
	
	
	public override void _Ready() {
		
		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.SetSeed((uint)GD.Hash(GlobalPosition));
		
		wall  = GD.Load<PackedScene>("res://scenes/room_modules/wall_module.tscn" );
		floor = GD.Load<PackedScene>("res://scenes/room_modules/floor_module.tscn");
		tree  = GD.Load<PackedScene>("res://scenes/plant_modules/procgen_tree.tscn");
		
		var wallGrid = new Grid2D<bool>(100,100,true);
		for (int y=0; y<100; y++) {
			for (int  x=0; x<100; x++) {
				wallGrid.At(new Vector2I(x,y)).Data = false;
			}
		}
		wallGrid.At(new Vector2I(10,10)).Data = true;
		

		for (int i=0; i<100; i++) {
			wallGrid = wallGrid.Map((cell) => {
				int count = 0;
				var adjCell = cell as IAdjCell<Vector2I,bool>;
				foreach (var adj in adjCell.Adj()) {
					if (adj.Data) {
						count++;
					}
				}
				if (!cell.Data) {
					return (count==1);
				} else {
					return ((count>0)&&(count<5));
				}
			}) as Grid2D<bool>;
		}
		
		grid = wallGrid.Map((cell) => {
			Node3D node = null;
			if (cell.Data) {
				node = wall.Instantiate() as Node3D;
			} else {
				node = floor.Instantiate() as Node3D;
			}
			node.Transform = Transform3D.Identity
				.Translated(new Vector3(
					(float) cell.Location.X*4.0f,
					0.0f,
					(float) cell.Location.Y*4.0f
				));
			AddChild(node);
			if (!cell.Data) {
				if (rng.Randi()%100 == 0){
					var treeInst = tree.Instantiate() as Node3D;
					node.AddChild(treeInst);
				}
			}
			return node;
		}) as Grid2D<Node3D>;
		
	}
	
	
}
