using Godot;
using System;

public partial class FloorChange : MeshInstance3D
{
    private StandardMaterial3D material;

    private Timer timer;

    public override void _Ready()
    {
        base._Ready();
        material = (StandardMaterial3D)Mesh.SurfaceGetMaterial(0);
        if (material == null)
        {
            GD.PrintErr("No StandardMaterial3D found on surface 0 of the mesh.");
            return;
        }
        timer = GetTree().Root.GetNode<Timer>("Menu/Background/Timer");
        timer.Timeout += ChangeColor;
    }

    private async void ChangeColor()
    {
        await ToSignal(GetTree().CreateTimer(0.9), "timeout");
        if(material.AlbedoColor == Colors.DarkBlue)
        {
            material.AlbedoColor = Colors.Gold;
        }
        else if(material.AlbedoColor == Colors.Red)
        {
            material.AlbedoColor = Colors.DarkBlue;
        }
        else
        {
            material.AlbedoColor = Colors.Red;
        }
    }
}
