using Godot;
using System;
using ProcGen;

public partial class ProcGenMaze : Node3D
{
	
	static PackedScene wall;
	static PackedScene floor;
	static PackedScene tree;
	
	
	float tileScale;
	Grid2D<MazeTile> grid;
	
	
	private class MazeTile {
		public Node3D  Root;
		public bool    IsWall;
		public bool    HasTree;
		public Vector3 Position;
		
		public MazeTile(bool isWall, bool hasTree, Vector3 position) {
			IsWall = isWall;
			HasTree = hasTree;
			Position = position;
		}
		
		public void Clear () {
			if (Root is null) {
				return;
			}
			Root.QueueFree();
			Root = null;
		}
		
		public void Reify(Node3D parent) {
			if (Root is not null) {
				return;
			}
			if (IsWall) {
				Root = ProcGenMaze.wall.Instantiate() as Node3D;
			} else {
				Root = ProcGenMaze.floor.Instantiate() as Node3D;
			}
			Root.Transform = Transform3D.Identity.Translated(Position);
			parent.AddChild(Root);
			if (HasTree) {
				var treeInst = ProcGenMaze.tree.Instantiate() as Node3D;
				Root.AddChild(treeInst);
			}
		}

	}
			

		


	public override void _Ready() {
		
		tileScale = 4.0f;
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
				foreach (var adj in cell.AsAdj().Adj()) {
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
			Vector3 position = Transform3D.Identity
			.Translated(new Vector3(
				(float) cell.Location.X*tileScale,
				0.0f,
				(float) cell.Location.Y*tileScale
			)) * new Vector3(0,0,0);
			if (cell.Data) {
				return new MazeTile(true,false,position);
			} else {
				bool hasTree = ((rng.Randi()%10) == 0);
				return new MazeTile(false,hasTree,position);
			}
		}) as Grid2D<MazeTile>;
		
	}
	
	public override void _Process(double delta) {
		foreach (var cell in grid) {
			Vector3 position = Transform * cell.Data.Position;
			Vector3 offset = position - Player.ActivePlayer.GlobalPosition;
			if (offset.Length() > 100.0f) {
				cell.Data.Clear();
			} else {
				cell.Data.Reify(this);
			}
			
		}
	}
	
	
}
