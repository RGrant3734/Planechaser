using Godot;
using System;

public partial class Explosion : Area3D
{
	// Particles reference
	[Export] public GpuParticles3D particles;
	// Explosion audio reference
	[Export] public AudioStreamPlayer3D audio;
	[Export] public CollisionShape3D collision;
	[Export] public float explosionDamage = 25.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		// Off rip we want the particles to spawn along with the audio
		particles.Emitting = true;
		audio.Play();
    }

	private void OnTimerTimeout()
    {
		// If timer runs out then destroy the object
        QueueFree();
    }

	private void OnCollisionTimerTimeout()
    {
		collision.QueueFree();
    }

	// We would need an OnBodyEntered function from Area3D
	private void OnBodyEntered(Node3D body)
    {
		// Explosion can register both enemies and eye boss
		if(body.IsInGroup("Enemy") || body.IsInGroup("Eye"))
			body.Call("Hit", 2, explosionDamage);
        
    }

}
