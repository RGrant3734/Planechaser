using Godot;
using System;

public partial class gruntController : meleeEnemy
{
    protected ShapeCast3D floorDetection;
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
                //Take damage here
                if(currentHealth <= 0)
                {
                    animationTree.Set("parameters/conditions/Death", true);
                }
                animationTree.Set("parameters/conditions/Hit", false);
                break;
            case "Death":
                if (dead)
                {
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
    public override void Hit(string weapon, float damage)
    {
        float damageFinal = 0;
        switch (weapon)
        {
            case "AK":
                break;
            case "Rifle":
                break;
            case "Rocket":
                break;
            default:
                break;
        }
        currentHealth -= damageFinal;
        if(currentHealth <= 0)
        {
            animationTree.Set("parameters/conditions/Death", true);
        } else {
            animationTree.Set("parameters/conditions/Hit", true);
        }
    }
}
