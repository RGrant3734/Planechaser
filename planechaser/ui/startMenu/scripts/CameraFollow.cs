using Godot;
using System;

public partial class CameraFollow : Camera3D
{
    [Export]
    public Node3D target;
    public override void _Process(double delta)
    {
        this.Position = this.Position.Lerp(target.Position + target.Basis.Z *2 + Basis.Y, .8f);
        LookAt(this.target.Position);
    }
}
