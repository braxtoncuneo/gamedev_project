using Godot;
using System;
using ProcGen;

public partial class ProcGenMaze : Node3D
{
	
	
	public class MazeTile {
		
		public bool   IsWall;
		public Node3D TileNode;
		
	}
	
	Grid2D<MazeTile> grid;
	
	PackedScene wall;
	PackedScene floor;
	
	
	public override void _Ready() {
		
		wall  = GD.Load<PackedScene>("res://scenes/room_modules/wall_module.tscn" );
		floor = GD.Load<PackedScene>("res://scenes/room_modules/floor_module.tscn");
		
		grid = new Grid2D<MazeTile>(100,100,true);
		for (int y=0; y<100; y++) {
			for (int  x=0; x<100; x++) {
				var tile = new MazeTile();
				tile.IsWall = false;
				grid.At(new Vector2I(x,y)).Data = tile;
			}
		}
		grid.At(new Vector2I(10,10)).Data.IsWall = true;
		

		for (int i=0; i<100; i++) {
			grid = grid.Map((cell) => {
				var result = new MazeTile();
				int count = 0;
				var adjCell = cell as IAdjCell<Vector2I,MazeTile>;
				foreach (var adj in adjCell.Adj()) {
					if (adj.Data.IsWall) {
						count++;
					}
				}
				if (!cell.Data.IsWall) {
					result.IsWall = (count==1);
				} else {
					result.IsWall = ((count>0)&&(count<5));
				}
				return result;
			}) as Grid2D<MazeTile>;
		}
		
		foreach (var cell in grid) {
			if (cell.Data.IsWall) {
				cell.Data.TileNode = wall.Instantiate() as Node3D;
			} else {
				cell.Data.TileNode = floor.Instantiate() as Node3D;
			}
			cell.Data.TileNode.Transform = Transform3D.Identity
				.Translated(new Vector3(
					(float) cell.Location.X*4.0f,
					0.0f,
					(float) cell.Location.Y*4.0f
				));
			AddChild(cell.Data.TileNode);
		}
	}
	
	
}
