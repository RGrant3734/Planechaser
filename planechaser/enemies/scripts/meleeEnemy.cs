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
    protected bool dead = false;

    //Checks if the player is within the enemies melee range
    protected bool InMeleeRange()
    {
        return GlobalPosition.DistanceTo(player.GlobalPosition) < meleeRange;
    }
    protected void HitPlayer()
    {
        if(InMeleeRange())
        {
            //Player takes damage
        }
    }
    // When enemy is hit triggers state
    public abstract void Hit(string weapon, float damage);

    // Want to make sure that animations play all the way through before deleted
    protected void AnimFinished(StringName anim)
    {
        if (anim == "Death")
        {
            dead = true;
        }
    }
}
