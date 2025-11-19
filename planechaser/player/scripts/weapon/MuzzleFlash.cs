using Godot;
using System;

public partial class MuzzleFlash : Node3D
{
	[Signal] public delegate void MuzzleFlashSignalEventHandler();
	[Export] private float flashTime = 0.05f;
	
	private OmniLight3D light;
	private GpuParticles3D emitter;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        light = GetNode<OmniLight3D>("OmniLight3D");
		emitter = GetNode<GpuParticles3D>("GPUParticles3D");
		MuzzleFlashSignal += AddMuzzleFlash;
    }

	public async void AddMuzzleFlash()
    {
		// Make sure light is off in the editor
		light.Visible = true;
		emitter.Emitting = true;
		
		// Flash is really fast!
		await ToSignal(GetTree().CreateTimer(flashTime), "timeout");
		light.Visible = false;

        
    }
}
