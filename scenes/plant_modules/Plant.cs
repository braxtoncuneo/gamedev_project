using Godot;
using System;
using System.Collections.Generic;





public partial class Plant : Node3D
{
	
	public class PlantSeed {
		public RandomNumberGenerator rng;
		public PlantSeed(uint seed) {
			rng.SetSeed(seed);
		}
	}
	
	public interface ISpecies
	{
		public bool   GrowCell (ProcGen.ITreeCell<List<int>,ProcGen.Reify> cell);
		public Node3D ReifyCell(ProcGen.ITreeCell<List<int>,ProcGen.Reify> cell);
	}
	
	ISpecies species;
	ProcGen.Tree<int,ProcGen.Reify> body;
	
	public Plant(ISpecies species) {
		this.species = species;
		body = new ProcGen.Tree<int,ProcGen.Reify>();
	}
	
	public void Grow() {
		species.GrowCell(body.At().AsTree());
	}
	
	public void Reify() {
		Node3D rootNode = species.ReifyCell(body.At().AsTree());
		AddChild(rootNode);
		rootNode.SetOwner(this);
	}
	
	public override void _Ready() {
		
		body.At().Data = new PlantSeed((uint)GD.Hash(GlobalPosition));
		
		while(species.Grow(body.At().AsTree())){}
		
	}
}
