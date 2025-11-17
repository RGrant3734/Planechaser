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
    public float meleeSpeed = 1;
    [Export]
    public float meleeDelay = 1;
    [Export]
    public float meleeRange = 2;

    //Animation Values
    protected Vector3 moveVal;
    protected double deathVal;
    protected bool dead = false;


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

    protected void Hit(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            animationTree.Set("parameters/conditions/Death", true);
        }
        else
        {
            animationTree.Set("parameters/conditions/Hit", true);
        }
    }
        protected void AnimFinished(StringName anim)
    {
        if (anim == "Death")
        {
            dead = true;
        }
    }
}
