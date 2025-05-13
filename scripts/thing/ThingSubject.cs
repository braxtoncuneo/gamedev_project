using Godot;
using System;

[GlobalClass]
public partial class ThingSubject : Resource
{
	public const int INSPECTION_LAYER = 2;
		
	[Export]
	public String Name { get; set; }
}
