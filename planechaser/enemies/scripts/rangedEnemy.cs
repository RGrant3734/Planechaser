using Godot;
using System;

public abstract partial class rangedEnemy : meleeEnemy
{
    [ExportGroup("Ranged")]
    [Export]
    public float rangedDamage = 10;
    [Export]
    public float rangedDelay = 1;
    [Export]
    public RayCast3D sight;
    [Export]
    public PackedScene projectile;

    protected bool InSight()
    {
        if (!InMeleeRange())
        {
            // If sight is blocked by non enemy obstacle then false, else it sees the player
            var collider = sight.GetCollider() as CharacterBody3D;
            if (collider == null)
                return false;

            if (collider.IsInGroup("Player"))
            {
                return true;
            }
        }
        return false;
    }
    
    protected void RangedAttack()
    {
        // Creates the projectile, adds to scene, then moves it to enemy that fires it and faces it to the player.
        var projectileFired = projectile.Instantiate<enemyProjectile>();
        GetTree().CurrentScene.AddChild(projectileFired);
        projectileFired.GlobalPosition = sight.GlobalPosition;
        projectileFired.LookAt(player.GlobalPosition, Vector3.Up);
    }
}
