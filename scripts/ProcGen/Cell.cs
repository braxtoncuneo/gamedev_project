using Godot;
using System;

using System.Collections.Generic;

namespace ProcGen {

public interface ICell <Coord,DataType>
{
	public Coord   Location {get;}
	public DataType Data {get;set;}
}

public interface IAdjCell <Coord,DataType> : ICell<Coord,DataType>
{
	public IEnumerable<IAdjCell<Coord,DataType>> Adj();
}

}
