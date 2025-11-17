using Godot;
using System;

public abstract partial class rangedEnemy : meleeEnemy
{
    [ExportGroup("Ranged")]
    [Export]
    public float rangedDamage = 10;
    [Export]
    public float rangedSpeed = 1;
    [Export]
    public float rangedDelay = 1;
    [Export]
    public RayCast3D sight;
    protected override void OnSpawn()
    {

    }
    
    protected void RangedAttack()
    {
        
    }
}
