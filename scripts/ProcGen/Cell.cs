using Godot;
using System;

using System.Collections.Generic;

namespace ProcGen {

public interface ICell <Coord,DataType>
{
	public Coord   Location {get;}
	public DataType Data {get;set;}
}

public interface IAdjCell <CellKind,Coord,DataType> : ICell<Coord,DataType>
	where CellKind : IAdjCell <CellKind,Coord,DataType>, ICell<Coord,DataType>
{
	public IEnumerable<CellKind> Adj();
}

public interface IDagCell <CellKind,Coord,DataType> : ICell<Coord,DataType>
	where CellKind : IDagCell <CellKind,Coord,DataType>, ICell<Coord,DataType>
{
	public IEnumerable<CellKind> InAdj();
	public IEnumerable<CellKind> OutAdj();
}

public interface ITreeCell <CellKind,Coord,DataType> : ICell<Coord,DataType>
	where CellKind : ITreeCell <CellKind,Coord,DataType>, ICell<Coord,DataType>
{
	public CellKind Parent();
	public IEnumerable<CellKind> Children();
}

public interface ICellDistance <CellKind,Coord,DataType,Length> : ICell<Coord,DataType>
	where CellKind : ICellDistance <CellKind,Coord,DataType,Length>, ICell<Coord,DataType>
{
	public Length DistanceTo(CellKind otherCell);
}

}
