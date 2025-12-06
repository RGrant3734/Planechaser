using Godot;
using System;
using System.ComponentModel;

public partial class FragementParticlePickup : CharacterBody3D
{
    [Export] private AudioStreamPlayer3D audio;
    [Export] private Node3D soul;

    private Area3D pickupArea;
    private Area3D followPlayerArea;

    private Vector3 initialPosition;
    [Export] private float followSpeed = 2.0f;
    private Vector3 targetPosition;
    private MeshInstance3D meshInstance;
    private StandardMaterial3D material;
    private Color[] colors = { new Color(1f, 0f, 0f), new Color(1f, 1f, 0f), new Color(0f, 0f, 1f) };
    private float colorTimer = 0.0f;
    [Export] private float colorSegmentDuration = 1.25f; // seconds per segment between two colors

    public override void _Ready()
    {
        base._Ready();
        pickupArea = GetNode<Area3D>("PickupArea");
        followPlayerArea = GetNode<Area3D>("FollowPlayerArea");

        initialPosition = GlobalPosition;
        targetPosition = initialPosition;

        pickupArea.BodyEntered += OnPickupAreaBodyEntered;
        followPlayerArea.BodyEntered += OnFollowPlayerAreaBodyEntered;

        // setup material instance for this fragment's mesh so we can modify it without changing the scene resource globally
        meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
        var activeMat = meshInstance.GetActiveMaterial(0);
        if (activeMat is StandardMaterial3D stdMat)
        {
            material = stdMat.Duplicate() as StandardMaterial3D;
            meshInstance.SetSurfaceOverrideMaterial(0, material);
        }
    }

    // Initialize the fragment's starting world position and ensure internal
    // position/target tracking variables match it. Triggers after spawned from enemy.
    public void InitializeSpawnPosition(Vector3 worldPosition)
    {
        GlobalPosition = worldPosition;
        initialPosition = GlobalPosition;
        targetPosition = initialPosition;
        // Reset any physics motion so the fragment doesn't translate unexpectedly on spawn
        Velocity = Vector3.Zero;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        // update velocity to follow target position
        Vector3 direction = (targetPosition - GlobalPosition).Normalized();
        Velocity = direction * followSpeed + GetGravity()/2;
        MoveAndSlide();
        // update material color if we have an instance
        if (material != null)
        {
            colorTimer += (float)delta;
            float totalDuration = colors.Length * colorSegmentDuration;
            if (colorTimer >= totalDuration)
            {
                colorTimer -= totalDuration;
            }
            float step = colorTimer / colorSegmentDuration; // e.g. 0..colors.Length
            int index = (int)MathF.Floor(step) % colors.Length;
            float frac = step - MathF.Floor(step);
            // get from and to colors
            Color from = colors[index];
            Color to = colors[(index + 1) % colors.Length];
            Color c = new Color(
                from.R + (to.R - from.R) * frac,
                from.G + (to.G - from.G) * frac,
                from.B + (to.B - from.B) * frac,
                from.A + (to.A - from.A) * frac
            );
            material.AlbedoColor = c;
            // optionally set emission to the same color so it glows nicely
            material.Emission = c;
        }
    }


    private async void OnPickupAreaBodyEntered(Node3D body)
    {
        if (body is FPSController player)
        {
            // Notify the player of the fragment particle pickup
            player.FragmentParticlePickup();
            audio.Play();
            pickupArea.SetDeferred("monitoring", false);
		    pickupArea.SetDeferred("monitorable", false);
            soul.Visible = false;
            meshInstance.Visible = false;
            await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
            
            QueueFree();
        }
    }
    private void OnFollowPlayerAreaBodyEntered(Node3D body)
    {
        if (body is FPSController player)
        {
            // set target position to player's position
            targetPosition = player.GlobalPosition;
        }
    }
}
