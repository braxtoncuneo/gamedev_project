using Godot;
using System;

using System.Collections.Generic;
using System.Linq;

namespace ProcGen {


public class Grid2D <DataType>
	: ISpace<Vector2I,DataType>
	, ISnappableSpace<Vector2I,DataType,Vector2>
	, IMappableSpace<Vector2I,DataType>
	, IEnumerable<ICell<Vector2I,DataType>>
{


	int width;
	int height;
	bool wrap;

	Dictionary<Vector2I,DataType> cells;

	public Grid2D (int width, int height, bool wrap)
	{
		this.width = width;
		this.height = height;
		this.wrap = wrap;
		cells = new Dictionary<Vector2I,DataType>();
	}
	
	private bool BoundCoord(ref Vector2I coord) {
		int x = coord.X;
		int y = coord.Y;
		if (wrap) {
			x -= x/width;
			y -= y/height;
		}
		if ( (x<0) || (x>=width) || (y<0) || (y>=height) ){
			return false;
		}
		coord.X = x;
		coord.Y = y;
		return true;
	}

	public ICell<Vector2I,DataType> At(Vector2I coord) {
		if (BoundCoord(ref coord)){
			return new Cell(this,coord);
		} else {
			return null;
		}
	}

	public Vector2I Snap(Vector2 looseCoord) {
		int x = (int) Math.Floor(looseCoord.X);
		int y = (int) Math.Floor(looseCoord.Y);
		if (wrap) {
			x -= x/width;
			y -= y/height;
		}
		return new Vector2I(x,y);
	}

	public ICell<Vector2I,DataType> By(Vector2 looseCoord) {
		return At(Snap(looseCoord));
	}

	public IEnumerator<ICell<Vector2I,DataType>> GetEnumerator() {
		foreach (var pair in cells) {
			yield return new Cell(this,pair.Key) as ICell<Vector2I,DataType>;
		}
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}


	public IMappableSpace<Vector2I,OtherDataType> Map<OtherDataType>(Func<ICell<Vector2I,DataType>,OtherDataType> function) {
		Grid2D<OtherDataType> result = new Grid2D<OtherDataType>(width,height,wrap);

		foreach (var cell in this) {
			result.At(cell.Location).Data = function(cell);
		}

		return result as IMappableSpace<Vector2I,OtherDataType>;
	}


	public class Cell
		: ICell<Vector2I,DataType>
		, IAdjCell<Vector2I,DataType>
	{
		Grid2D<DataType> backingGrid;

		Vector2I location;

		public Vector2I Location {
			get { return location; }
		}

		public DataType Data {
			get {
				DataType result;
				backingGrid.cells.TryGetValue(Location, out result);
				return result;
			}
			set {
				backingGrid.cells[location] = value;
			}
		}

		public IEnumerable<IAdjCell<Vector2I,DataType>> Adj() {
			for(int y=-1; y<=1; y++) {
				for(int x=-1; x<=1; x++) {
					if ( (x==0) && (y==0) ) {
						continue;
					}
					Vector2I adjLocation = new Vector2I(location.X+x,location.Y+y);
					if (backingGrid.BoundCoord(ref adjLocation)) {
						var result = (new Cell(backingGrid,adjLocation)) as IAdjCell<Vector2I,DataType>;
						yield return result;
					}
				}
			}
		}

		public Cell (Grid2D<DataType> backingGrid, Vector2I location) {
			this.backingGrid = backingGrid;
			this.location = location;
		}

	}

}


}
