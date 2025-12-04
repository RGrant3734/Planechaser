using Godot;
using System;

public partial class GlassWall : StaticBody3D
{
	public float currentHealth = 1000;
	public GameMaster gameMaster;
	public MeshInstance3D mesh;
	public GpuParticles3D breakParticle;
	public StandardMaterial3D deadMaterial;
	public override void _Ready()
	{
		gameMaster = GetNode<GameMaster>("/root/GameMaster");
		breakParticle = GetNode<GpuParticles3D>("BreakEffect");
		mesh = GetNode<MeshInstance3D>("Wall");
	}
	public void Hit(int weaponPlane, float baseDamage)
	{
		// If the current weapon's native plane matches the current level plane then do bonus damage
		if(weaponPlane == gameMaster.currentDimension)
			currentHealth -= baseDamage * 1.5f;
		else
			currentHealth -= baseDamage;
			
		if(currentHealth <= 0)
		{
			QueueFree();
		} else {
			breakParticle.Emitting = true;
		}
	}
}
