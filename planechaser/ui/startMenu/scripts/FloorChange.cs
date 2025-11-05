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
        timer.Start();
        timer.Timeout += ChangeColor;
    }

    private void ChangeColor()
    {
        if(material.AlbedoColor == Colors.DarkBlue)
        {
            // change albedo color to red
            material.AlbedoColor = Colors.Red;
        }
        else if(material.AlbedoColor == Colors.Red)
        {
            material.AlbedoColor = Colors.Gold;
        }
        else
        {
            material.AlbedoColor = Colors.DarkBlue;
        }
    }
}
