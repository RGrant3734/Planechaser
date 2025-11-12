using Godot;
using System;

public abstract partial class baseEnemy : Node
{
    // Property that allows offset of plane
    [Export(PropertyHint.Range, "0, 2, ")]
    public int startTypeNumber = 0;

    // Base Stats
    [ExportGroup("Stats")]
    public int totalHealth = 100;
    [Export]
    public int armor = 0;

    protected int currentHealth;
    protected GameMaster gameMaster;
    protected CharacterBody3D player;
    protected NavigationAgent3D nav;

    public override void _Ready()
    {
        gameMaster = GetNode<GameMaster>("/root/GameMaster");
        gameMaster.Planeshift += Planeshift;

        player = GetTree().GetFirstNodeInGroup("Player") as CharacterBody3D;
        if (player == null)
        {
            throw new Exception("Expects a player in group Player");
        }

        nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
        if (nav == null)
        {
            throw new Exception("Expects a navigation agent 3D as a child with default name");
        }
    }

    protected virtual void Planeshift(int currDimension)
    {
        SwapType((currDimension + startTypeNumber) % 3);
    }

    protected abstract void SwapType(int type);
    protected abstract void OnSpawn();
}
