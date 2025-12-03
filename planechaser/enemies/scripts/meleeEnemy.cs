using Godot;
using System;
using System.Text.RegularExpressions;

public abstract partial class meleeEnemy : baseEnemy
{
	[ExportGroup("Melee")]
	[Export]
	public float moveSpeed = 1.0f;
	[Export]
	public float meleeDamage = 10.0f;
	[Export]
	public float meleeDelay = 1.0f;
	[Export]
	public float meleeRange = 2.0f;

	//Animation Values
	protected Vector3 moveVal;
	protected bool hitPlayed = false;
	
	//Checks if the player is within the enemies melee range
	protected bool InMeleeRange()
	{
		return GlobalPosition.DistanceTo(player.GlobalPosition) < meleeRange;
	}
	// Called by one of the animations
	protected void HitPlayer()
	{
		if(InMeleeRange())
		{
			//Player takes damage
			player.TakeDamage(meleeDamage);
		}
	}
	// When enemy is hit triggers state
	public abstract void Hit(int weapon, float damage);

	// Want to make sure that animations play all the way through before deleted
	protected void AnimFinished(StringName anim)
	{
		if (anim == "Death")
		{
			dead = true;
		}
		if (anim == "Hit")
		{
			hitPlayed = true;
		}
	}
}
