using Godot;
using System;

public partial class enemyProjectile : RigidBody3D
{
    [Export]
    public float speed = 1f;
    public Vector3 direction;
    public async override void _Ready()
    {
        // For some reason, spawns at the player, so disabled collison and visibility until it reached its true spawn point
        GetNode<CollisionShape3D>("Area3D/CollisionShape3D").Disabled = true;
        Visible = false;
        await ToSignal(GetTree().CreateTimer(0.04), "timeout");
        Visible = true;
        GetNode<CollisionShape3D>("Area3D/CollisionShape3D").Disabled = false;
    }
    public override void _PhysicsProcess(double delta)
    {
        //Fires forward
        LinearVelocity = Transform.Basis.Z * speed * -1;
    }
    public void AreaContact(Node3D body)
    {
        // Mask 2, if player then handle damage, else it is a non enemy obstacle
        if (body.IsInGroup("Player"))
        {
            //Player takes damage
        }
        QueueFree();
    }
}
