using Godot;
using System;
using System.Text.RegularExpressions;

public abstract partial class meleeEnemy : baseEnemy
{
    [ExportGroup("Melee")]
    [Export]
    public float moveSpeed = 1;
    [Export]
    public float meleeDamage = 10;
    [Export]
    public float meleeDelay = 1;
    [Export]
    public float meleeRange = 2;

    //Animation Values
    protected Vector3 moveVal;

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
            player.TakeDamage();
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
    }
}
