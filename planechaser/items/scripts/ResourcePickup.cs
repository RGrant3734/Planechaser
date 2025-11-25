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
	private Vector3 position;
	private float origin = 0.0f;
	private float time = 0.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        position = resource.Position;
		origin = resource.Position.Y;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		time += (float)delta;
		position.Y = origin + Mathf.Sin(time * frequency) * amplitude;
		resource.Position = position;
		Rotate(resource.Transform.Basis.Y.Normalized(), Mathf.DegToRad(angle));
        
    }

	public void OnBodyEntered(Node3D body)
    {
		// Send player signal here
		if(body.IsInGroup("Player"))
			body.Call("ResourcePickup", resourceType, plane);
        QueueFree();
    }


}
