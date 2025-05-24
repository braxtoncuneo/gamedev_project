using Godot;
using System;

using System.Collections.Generic;
using System.Net.Mime;

namespace ProcGen {

public interface ISpace <CellKind,Coord,DataType>
	where CellKind : ICell<Coord,DataType>
{
	public CellKind At(Coord coord);
}

public interface ISnappableSpace <CellKind,Coord,DataType,LooseCoord> : ISpace <CellKind,Coord,DataType>
{
	public Coord Snap(LooseCoord looseCoord);
	public CellKind By(LooseCoord looseCoord);
}

public interface IMappableSpace <CellKind,Coord,DataType> : ISpace <CellKind,Coord, DataType>
{
	public OtherSpaceKind Map<OtherDataType>(Func<CellKind,OtherDataType> function);
}


}
