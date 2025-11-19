using Godot;
using System;

public partial class Globals : Node
{
	// Reference to player conponent. This will act as our singleton
    // Its static here since we dont want to create a Globals object
    static public FPSController player { get; set; }
}
