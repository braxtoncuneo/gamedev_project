using Godot;
using System;

using System.Collections.Generic;

namespace ProcGen {
	
public struct ZoneCoord {
	Vector3I position;
	int      scale;
	
	public Vector3I Position { get { return position; } }
	public int      Scale    { get { return scale;    } }
	
	public ZoneCoord(Vector3I position, int scale) {
		this.position = position;
		this.scale    = scale;
	}
	
	public ZoneCoord Scaled(int scale) {
		ZoneCoord result = this;
		int delta = scale - this.scale;
		if (delta != 0) {
			result.position.X = position.X >> delta;
			result.position.Y = position.Y >> delta;
			result.position.Z = position.Z >> delta;
		}
		return result;
	}
};


public abstract class Zone
{
	
	public const int MaxScale = 10;
	public const int MinScale = 10;
	
	private static Dictionary<ZoneCoord, Zone> zoneRegistry = new Dictionary<ZoneCoord,Zone>();
	
	private ZoneCoord coord;
	public  ZoneCoord Coord { get { return coord; } }
	
	public Zone(ZoneCoord coord) {
		this.coord = coord;
	}
	
	public abstract bool Refine();
	
	public static Zone Lookup(ZoneCoord coord) {
		Zone result = null;
		zoneRegistry.TryGetValue(coord, out result);
		return result;
	}
	
	public static Zone LookupSmallest(ZoneCoord coord) {
		Zone result = null;
		int min = MinScale;
		int max = MaxScale;
		int last = max + 1;
		while (max > min) {
			int mid = (min+max) / 2;
			ZoneCoord searchcoord = coord.Scaled(mid);
			result = Lookup(coord.Scaled(mid));
			if (result is not null) {
				max = mid;
			} else {
				min = mid + 1;
			}
		}
		return Lookup(coord.Scaled(min));
	}
	
	public static Zone Generate(ZoneCoord coord) {
		Zone result = Lookup(coord);
		if (result is not null) {
			return result;
		}
		result = LookupSmallest(coord);
		int initialScale = result.Coord.Scale;
		int scaleLimit = coord.Scale;
		for (int i=initialScale-1; i<scaleLimit; i++) {
			if (!result.Refine()) {
				return result;
			}
			Zone smallerResult = Lookup(coord.Scaled(i));
			if (smallerResul is null) {
				return result;
			} else {
				result = smallerResult;
			}
		}
		return smallerResult;
	}
	
}

}
