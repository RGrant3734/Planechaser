using Godot;
using System;

public partial class ShiftingDevice : MeshInstance3D
{
	[Export] public float frequency = 1.0f;
	[Export] public float amplitude = 0.02f;
	[Export] public float angle = 3.0f;
	private Vector3 position;
	private float origin = 0.0f;
	private float time = 0.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        position = Position;
		origin = Position.Y;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		// time aids in acquiring a new value to feed into sin. We use frequency and amplitude values
		// to create a custom bobbing animation for the resource pickup
		// Only need to wrap time if very long play sessions (days) are to be expected
		time += (float)delta;
		position.Y = origin + Mathf.Sin(time * frequency) * amplitude;
		// Update the position and rotate it to the according angle
		Position = position;
		//Rotate(Transform.Basis.Y.Normalized(), Mathf.DegToRad(angle));
        
    }
}
