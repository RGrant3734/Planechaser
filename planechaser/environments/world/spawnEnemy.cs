using Godot;
using System;
using System.Diagnostics;

public partial class spawnEnemy : StaticBody3D
{
    [Export(PropertyHint.Range, "0, 2,")]
    public int enemyType = 0; // 0: Grunt, 1: Knight, 2: Mage
    [Export]
    public int numInWave = 1;
    [Export]
    public float waveTimer = 20;
    [Export(PropertyHint.Range, "0, 2,")]
    public int armorPlane = 0;
    [Export]
    public PackedScene grunt;
    [Export]
    public PackedScene knight;
    [Export]
    public PackedScene mage;
    protected GameMaster gameMaster;
    protected Marker3D spawnPoint;

    public override void _Ready()
    {
        gameMaster = GetNode<GameMaster>("/root/GameMaster");
        gameMaster.Spawn += Spawn;
        spawnPoint = GetNode<Marker3D>("Marker3D");
    }

    // When spawning starts, the enemy spawns and after some time another wave spawns
    public async void Spawn()
    {
        while(gameMaster.spawning){
            for (int i = 0; i < numInWave; i++)
            {
                switch (enemyType)
                {
                    case 0:
                        var gruntSpawned = grunt.Instantiate<gruntController>();
                        GetTree().CurrentScene.AddChild(gruntSpawned);
                        gruntSpawned.GlobalPosition = spawnPoint.GlobalPosition;
                        break;
                    case 1:
                        var knightSpawned = knight.Instantiate<knightController>();
                        knightSpawned.armorPlane = armorPlane;
                        GetTree().CurrentScene.AddChild(knightSpawned);
                        knightSpawned.GlobalPosition = spawnPoint.GlobalPosition;
                        break;
                    case 2:
                        var mageSpawned = mage.Instantiate<mageController>();
                        GetTree().CurrentScene.AddChild(mageSpawned);
                        mageSpawned.GlobalPosition = spawnPoint.GlobalPosition;
                        break;
                    default:
                        break;
                }
                await ToSignal(GetTree().CreateTimer(5), "timeout");
            }
            await ToSignal(GetTree().CreateTimer(waveTimer), "timeout");
        }
    }
}
