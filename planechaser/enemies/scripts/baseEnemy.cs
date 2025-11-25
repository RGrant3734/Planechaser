using Godot;
using System;

public abstract partial class baseEnemy : CharacterBody3D
{
    // Property that allows offset of plane
    [Export(PropertyHint.Range, "0, 2, ")]
    public int startTypeNumber = 0;
    [Export]
    public AnimationTree animationTree;

    // Base Stats
    [ExportGroup("Stats")]
    public float totalHealth = 100;
    [Export]
    public float armor = 0.0f;

    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D blueMaterial;
    [Export]
    public StandardMaterial3D yellowMaterial;
    [Export]
    public StandardMaterial3D redMaterial;
    [Export]
    public MeshInstance3D mesh;

    protected float currentHealth;
    protected GameMaster gameMaster;
    protected FPSController player;
    protected NavigationAgent3D nav;
    protected AnimationNodeStateMachinePlayback stateMachine;

    public override void _Ready()
    {
        // intializing important assets
        gameMaster = GetNode<GameMaster>("/root/GameMaster");
        gameMaster.Planeshift += Planeshift;

        player = GetTree().GetFirstNodeInGroup("Player") as FPSController;
        if (player == null)
        {
            throw new Exception("Expects a player in group Player");
        }

        nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
        if (nav == null)
        {
            throw new Exception("Expects a navigation agent 3D as a child with default name");
        }

        currentHealth = totalHealth;
        nav.TargetPosition = player.GlobalPosition;
        stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
        OnSpawn();
    }

    protected virtual void Planeshift(int currDimension)
    {
        SwapType((currDimension + startTypeNumber) % 3);
    }
    // All enemies swap at least their base mesh materials
    protected virtual void SwapType(int type)
    {
        switch (type)
        {
            case 0:
                mesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
                break;
            case 1:
                mesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
                break;
            case 2:
                mesh.Mesh.SurfaceSetMaterial(0, redMaterial);
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
    }
    protected abstract void OnSpawn();
}
