using Godot;
using System;

public partial class GlassWall : StaticBody3D
{
	[Export]
	private float totalHealth = 1000;
	public float currentHealth = 1000;
	public GameMaster gameMaster;
	public MeshInstance3D mesh;
	public GpuParticles3D breakParticle1;
	public GpuParticles3D breakParticle2;
	public GpuParticles3D breakParticle3;
	[Export]
	public FastNoiseLite noise; 
	public override void _Ready()
	{
		gameMaster = GetNode<GameMaster>("/root/GameMaster");
		breakParticle1 = GetNode<GpuParticles3D>("BreakEffect");
		breakParticle2 = GetNode<GpuParticles3D>("BreakEffect2");
		breakParticle3 = GetNode<GpuParticles3D>("BreakEffect3");
		mesh = GetNode<MeshInstance3D>("Wall");
		currentHealth = totalHealth;
	}
	public void Hit(int weaponPlane, float baseDamage)
	{
		if(!gameMaster.spawning)
			return;
		// If the current weapon's native plane matches the current level plane then do bonus damage
		if(weaponPlane == gameMaster.currentDimension)
			currentHealth -= baseDamage * 1.5f;
		else
			currentHealth -= baseDamage;
			
		noise.Frequency = 1 - (currentHealth/totalHealth);
		if(currentHealth <= 0)
		{
			QueueFree();
		} else {
			breakParticle1.Emitting = true;
			breakParticle2.Emitting = true;
			breakParticle3.Emitting = true;
		}
	}
}
