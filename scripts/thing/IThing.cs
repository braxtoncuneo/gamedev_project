using Godot;
using System;

public interface IThing
{
	public ThingSubject Subject { get; set; }
	public ThingContent Content { get; set; }
}
