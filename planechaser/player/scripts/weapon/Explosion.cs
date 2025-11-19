using Godot;
using System;

public partial class Explosion : Area3D
{
	// Particles reference
	[Export] public GpuParticles3D particles;
	// Explosion audio reference
	[Export] public AudioStreamPlayer3D audio;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		// Off rip we want the particles to spawn along with the audio
		particles.Emitting = true;
		audio.Play();
    }

	private void OnTimerTimeOut()
    {
		// If timer runs out then destroy the object
        QueueFree();
    }

	// We would need an OnBodyEntered function from Area3D

}
