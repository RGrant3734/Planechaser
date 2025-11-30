using Godot;
using System;

public partial class gruntController : meleeEnemy
{
    protected ShapeCast3D floorDetection;

    [Export]
    public PackedScene fragmentParticle;

    protected override void OnSpawn()
    {
        SwapType(gameMaster.currentDimension);
        floorDetection = GetNode<ShapeCast3D>("FloorDetection");
    }

    public override void _Process(double delta)
    {
        switch (stateMachine.GetCurrentNode())
        {
            case "Idle":
                animationTree.Set("parameters/conditions/Walk", true);
                break;
            case "Walk":
                //Falls down if nothing under it
                if (!floorDetection.IsColliding())
                {
                    stateMachine.Travel("Fall");
                    break;
                }
                //Movement towards player
                Vector3 desiredDirection = Vector3.Zero;
                nav.TargetPosition = player.GlobalPosition;
                desiredDirection = (nav.GetNextPathPosition() - GlobalPosition).Normalized();
                Velocity = Velocity.Lerp(desiredDirection * moveSpeed, .4f);
                moveVal = moveVal.Lerp(new Godot.Vector3(1, 0, 0), .4f);
                LookAt(GlobalPosition + (Velocity * -1), Vector3.Up);
                animationTree.Set("parameters/conditions/Attack", InMeleeRange());
                MoveAndSlide();
                break;
            case "Attack":
                animationTree.Set("parameters/conditions/Walk", !InMeleeRange());
                break;
            case "Hit":
                animationTree.Set("parameters/conditions/Hit", false);
                break;
            case "Death":
                if (dead)
                {
                    // Spawn fragment particle
                    var fragment = fragmentParticle.Instantiate<FragementParticlePickup>();
                    GetTree().CurrentScene.AddChild(fragment);
                    fragment.InitializeSpawnPosition(GlobalPosition + new Vector3(0, 1, 0));
                    QueueFree();
                }
                break;
            case "Fall":
                //Disable nav movement
                nav.SetVelocity(Vector3.Zero);
                nav.AvoidanceEnabled = false;

                //Let physics take over
                Velocity = GetGravity();
                MoveAndSlide();

                //When floor detected again, resume walk
                if (floorDetection.IsColliding())
                {
                    nav.AvoidanceEnabled = true;
                    stateMachine.Travel("Walk");
                }
                break;
            default:
                break;
        }
    }
    //Enemy takes damage in their special ways and dies
    public override void Hit(int weaponPlane, float baseDamage)
    {
        // If the current weapon's native plane matches the current level plane then do bonus damage
        if(weaponPlane == gameMaster.currentDimension)
            currentHealth -= baseDamage * 1.5f;
        else
            currentHealth -= baseDamage;
        
        if(currentHealth <= 0)
        {
            animationTree.Set("parameters/conditions/Death", true);
        } else {
            animationTree.Set("parameters/conditions/Hit", true);
        }
    }
}
