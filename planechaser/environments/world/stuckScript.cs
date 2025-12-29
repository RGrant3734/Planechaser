using Godot;
using System;

public partial class stuckScript : Area3D
{
    [Export]
    public StaticBody3D mainObject;
    private GameMaster _GameMaster;

    public override void _Ready()
    {
        _GameMaster = GetNode<GameMaster>("/root/GameMaster");
    }

    public override void _Process(double delta)
    {
        if(GetParent<EnvironmentSwapper>().HiddenPlane != _GameMaster.currentDimension)
        {
            foreach (var body in GetOverlappingBodies())
            {
                if(body.IsInGroup("Player") || body.IsInGroup("Enemy"))
                {
                    body.GlobalPosition = new Vector3(body.GlobalPosition.X, body.GlobalPosition.Y + Scale.Y, body.GlobalPosition.Z);
                }
            }
        }
    }
}
