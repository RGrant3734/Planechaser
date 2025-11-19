using Godot;
using System;

public partial class Bullet : RigidBody3D
{
	// Speed of the bullet
	[Export] public float speed = 0.75f;
	// Sound effect for when the rocket is traveling
	[Export] private AudioStreamPlayer3D swoosh;
	// Explosion scene reference
	[Export] private PackedScene explosion;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Dont want the bullet to be a child of the gun when it spawns
		// so that it doesnt inherit its transforms
		TopLevel = true;
		swoosh.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		// Applying a driving force from the back side of the bullet towards the -Z direction
        ApplyImpulse(-Transform.Basis.Z * speed, Transform.Basis.Z);
    }

	public void OnBodyEntered(Node3D body)
    {
		// Ideally this is where the enemy takes damage and the explosion is spawned in
		
		// Instantiate explosion scene as soon as the bullet makes contact with a body
		// After that the explosion takes care of the rest
		Explosion E = explosion.Instantiate<Explosion>();
		E.Position = Position;
		GetTree().Root.AddChild(E);

		// Delete the bullet
        QueueFree();
    }
}
