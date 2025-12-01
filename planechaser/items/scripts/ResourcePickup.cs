using Godot;
using System;

public partial class ResourcePickup : Node3D
{
	[Export] public Node3D resource;
	[Export] public string resourceType;
	[Export(PropertyHint.Range, "0, 2, 1")] public int plane = 0;
	[Export] public float frequency = 3.0f;
	[Export] public float amplitude = 0.3f;
	[Export] public float angle = 3.0f;
	[Export] public Area3D collision;
	[Export] public AudioStreamPlayer3D audio;
	private Vector3 position;
	private float origin = 0.0f;
	private float time = 0.0f;
	private float pickupTimeout = 15.0f;
	private bool isFull;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        position = resource.Position;
		origin = resource.Position.Y;
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
		resource.Position = position;
		Rotate(resource.Transform.Basis.Y.Normalized(), Mathf.DegToRad(angle));
        
    }

	private void TooglePickup(bool condition)
    {
		resource.Visible = condition;
        collision.SetDeferred("monitoring", condition);
		collision.SetDeferred("monitorable", condition);
    }

	public async void OnBodyEntered(Node3D body)
    {
		// Only queue free the resource when the players connected resource is not full
		// Returning false signifies that the player is currently full on the passed resourceType
		// Plane
		// Yellow = 0, Blue = 1, Red = 2
		if (body is FPSController player && !player.ResourcePickup(resourceType, plane))
        {
			// Get rid of the collision/area so that the player doesnt acquire it twice by accident
			// and play the associated audio 
			TooglePickup(false);
			audio.Play();
			
			await ToSignal(GetTree().CreateTimer(pickupTimeout), "timeout");
			// Reenable the pickup so that the player can pick it up again
			// This will reengage the OnBodyEntered signal again
			TooglePickup(true);
        }
    }


}
