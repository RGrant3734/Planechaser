using Godot;
using System;

public partial class FragementParticlePickup : RigidBody3D
{
    private Area3D pickupArea;
    private Area3D followPlayerArea;

    public override void _Ready()
    {
        base._Ready();
        pickupArea = GetNode<Area3D>("PickupArea");
        followPlayerArea = GetNode<Area3D>("FollowPlayerArea");

        pickupArea.BodyEntered += OnPickupAreaBodyEntered;
        followPlayerArea.BodyEntered += OnFollowPlayerAreaBodyEntered;
    }

    private void OnPickupAreaBodyEntered(Node3D body)
    {
        if (body is FPSController player)
        {
            // Notify the player of the fragment particle pickup
            player.FragmentParticlePickup();
            QueueFree();
        }
    }
    private void OnFollowPlayerAreaBodyEntered(Node3D body)
    {
        if (body is FPSController player)
        {
            // Start following the player
            var followTween = CreateTween();
            followTween.SetTrans(Tween.TransitionType.Sine);
            followTween.SetEase(Tween.EaseType.InOut);
            followTween.TweenProperty(this, "global_transform", new Transform3D(GlobalTransform.Basis, player.GlobalTransform.Origin), 0.5f);
        }
    }
}
