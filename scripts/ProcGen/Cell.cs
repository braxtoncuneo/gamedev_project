using Godot;
using System;

using System.Collections.Generic;

namespace ProcGen {

public interface ICell<Coord,DataType>
{
	public Coord   Location {get;}
	public DataType Data {get;set;}
	public IAdjCell<Coord,DataType> AsAdj() {
		return this as IAdjCell<Coord,DataType>;
	}
	public IDirAdjCell<Coord,DataType> AsDirAdj() {
		return this as IDirAdjCell<Coord,DataType>;
	}
	public ITreeCell<Coord,DataType> AsTree() {
		return this as ITreeCell<Coord,DataType>;
	}
}

public interface IAdjCell<Coord,DataType> : ICell<Coord,DataType>
{
	public IEnumerable<IAdjCell<Coord,DataType>> Adj();
}

public interface IDirAdjCell<Coord,DataType> : ICell<Coord,DataType>
{
	public IEnumerable<IDirAdjCell<Coord,DataType>> InAdj();
	public IEnumerable<IDirAdjCell<Coord,DataType>> OutAdj();
}

public interface ITreeCell<Coord,DataType> : ICell<Coord,DataType>
{
	public ITreeCell<Coord,DataType> Parent();
	public IEnumerable<ITreeCell<Coord,DataType>> Children();
	public ITreeCell<Coord,DataType> Root() {
		ITreeCell<Coord,DataType> iter = this;
		ITreeCell<Coord,DataType> parent = iter.Parent();
		while (parent is not null) {
			iter = parent;
			parent = iter.Parent();
		}
		return iter;
	}
	public IEnumerable<ITreeCell<Coord,DataType>> Descendants() {
		foreach (var child in Children()) {
			foreach (var desc in child.Descendants()) {
				yield return desc;
			}
		}
	}
	public IEnumerable<ITreeCell<Coord,DataType>> Leaves() {
		return Root().Descendants();
	}
}

public interface ICellDistance<Coord,DataType,Length> : ICell<Coord,DataType>
{
	public Length DistanceTo(ICellDistance<Coord,DataType,Length> otherCell);
}

}
