using Godot;
using System;

using System.Collections.Generic;
using System.Net.Mime;

namespace ProcGen {

public interface ISpace <Coord,DataType>
{
	public ICell<Coord,DataType> At(Coord coord);
}

public interface ISnappableSpace <Coord,DataType,LooseCoord> : ISpace <Coord,DataType>
{
	public Coord Snap(LooseCoord looseCoord);

	public ICell<Coord,DataType> By(LooseCoord looseCoord);

}

public interface IMappableSpace <Coord,DataType> : ISpace <Coord, DataType>
{
	public IMappableSpace<Coord,OtherDataType> Map<OtherDataType>(Func<ICell<Coord,DataType>,OtherDataType> function);
}


}
