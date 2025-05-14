using Godot;
using System;

using System.Collections.Generic;
using System.Linq;

namespace ProcGen {


public class Tree <KeyType,DataType>
	: ISpace<List<KeyType>,DataType>
	, IMappableSpace<List<KeyType>,DataType>
	, IEnumerable<ICell<List<KeyType>,DataType>>
{
	
	DataType data;
	Tree<KeyType,DataType> parent;
	Dictionary<KeyType,Tree<KeyType,DataType>> children;
	
	public Tree ()
	{
		parent   = null;
		children = new Dictionary<KeyType,Tree<KeyType,DataType>>();
	}
	
	private Tree(Tree<KeyType,DataType> parent) : this() {
		this.parent = parent;
	}

	public ICell<List<KeyType>,DataType> At(List<KeyType> coord) {
		Tree<KeyType,DataType> iter = this;
		int limit = coord.Count();
		for (int i=0; i<limit; i++) {
			KeyType key = coord[limit];
			if (! iter.children.TryGetValue(key, out iter)) {
				var newTree = new Tree<KeyType,DataType>(this);
				iter.children[key] = newTree;
				iter = newTree;
			}
		}
		return new Cell(iter,new List<KeyType>(coord));
	}

	private IEnumerator<ICell<List<KeyType>,DataType>> RecurseEnumeration(List<KeyType> coord) {
		yield return new Cell(this,coord) as ICell<List<KeyType>,DataType>;
		foreach (var pair in children) {
			var subcoord = new List<KeyType>(coord);
			subcoord.Add(pair.Key);
			yield return new Cell(this,subcoord) as ICell<List<KeyType>,DataType>;
		}
	}

	public IEnumerator<ICell<List<KeyType>,DataType>> GetEnumerator() {
		return this.RecurseEnumeration(new List<KeyType>());
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	private Tree<KeyType,OtherDataType> RecurseMap<OtherDataType>(
		Func<ICell<List<KeyType>,DataType>,OtherDataType> function,
		List<KeyType> coord
	) {
		Tree<KeyType,OtherDataType> result = new Tree<KeyType,OtherDataType>();
		result.data = function(new Cell(this,coord));
		foreach (var child in children) {
			var subcoord = new List<KeyType>(coord);;
			subcoord.Add(child.Key);
			result.children[child.Key] = child.Value.RecurseMap(function,subcoord);
		}
		return result;
	}

	public IMappableSpace<List<KeyType>,OtherDataType> Map<OtherDataType>(Func<ICell<List<KeyType>,DataType>,OtherDataType> function) {
		return RecurseMap(function,new List<KeyType>()) as IMappableSpace<List<KeyType>,OtherDataType>;
	}


	public class Cell
		: ICell<List<KeyType>,DataType>
		, IAdjCell<List<KeyType>,DataType>
	{
		Tree<KeyType,DataType> node;
		List<KeyType> location;

		public List<KeyType> Location {
			get { return location; }
		}

		public DataType Data {
			get {
				return node.data;
			}
			set {
				node.data = value;
			}
		}

		public IEnumerable<IAdjCell<List<KeyType>,DataType>> Adj() {
			if (node.parent is not null){
				var supercoord = new List<KeyType>(location);;
				supercoord.RemoveAt(location.Count()-1);
				yield return (new Cell(node.parent,supercoord)) as IAdjCell<List<KeyType>,DataType>;
			}
			foreach (var child in node.children) {
				var subcoord = new List<KeyType>(location);;
				subcoord.Add(child.Key);
				yield return (new Cell(child.Value,subcoord)) as IAdjCell<List<KeyType>,DataType>;
			}
		}

		public Cell (Tree<KeyType,DataType> node, List<KeyType> location) {
			this.node     = node;
			this.location = location;
		}

	}

}


}
