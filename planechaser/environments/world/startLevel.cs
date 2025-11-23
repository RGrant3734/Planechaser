using Godot;
using System;

public partial class startLevel : Area3D
{
    protected GameMaster gameMaster;
    public override void _Ready()
    {
        gameMaster = GetNode<GameMaster>("/root/GameMaster");
    }

    public void PlayerExited(Node3D body)
    {
        if(body.IsInGroup("Player"))
        {
            gameMaster.StartSpawn();
            QueueFree();
        }
    }
}
