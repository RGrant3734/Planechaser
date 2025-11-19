using Godot;
using System;

public partial class ImpactExplosion : GpuParticles3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Emitting = true;
	}

	private void OnTimerTimeout()
    {
        QueueFree();
    }
}
