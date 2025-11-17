using Godot;
using System;

public partial class gruntController : meleeEnemy
{
    protected override void OnSpawn()
    {
        SwapType(gameMaster.currentDimension);
    }

    public override void _Process(double delta)
    {
        switch (stateMachine.GetCurrentNode())
        {
            case "Idle":
                animationTree.Set("parameters/conditions/Walk", true);
                break;
            case "Walk":
                //Movement towards player
                Vector3 desiredDirection = Vector3.Zero;
                if (!nav.IsNavigationFinished() && nav.IsTargetReachable())
                {
                    nav.TargetPosition = player.GlobalPosition;
                    desiredDirection = (nav.GetNextPathPosition() - GlobalPosition).Normalized();
                    Velocity = Velocity.Lerp(desiredDirection * moveSpeed, .4f);
                    moveVal = moveVal.Lerp(new Godot.Vector3(1, 0, 0), .4f);
                    LookAt(GlobalPosition + (Velocity * -1), Vector3.Up);
                }
                animationTree.Set("parameters/conditions/Attack", InMeleeRange());
                MoveAndSlide();
                break;
            case "Attack":
                animationTree.Set("parameters/conditions/Walk", !InMeleeRange());
                break;
            case "Hit":
                LookAt(GlobalPosition + (Velocity * -1), Vector3.Up);
                animationTree.Set("parameters/conditions/Hit", false);
                break;
            case "Death":
                if (dead)
                {
                    QueueFree();
                }
                break;
            default:
                break;
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
