using Godot;
using System;

public partial class WeirdTree : StaticBody3D
{
    private MeshInstance3D mesh1;
    private MeshInstance3D mesh2;

    private StandardMaterial3D material1;
    private StandardMaterial3D material2;
    private Timer timer;

    public override void _Ready()
    {
        base._Ready();
        mesh1 = (MeshInstance3D)GetNode("Cylinder");
        material1 = (StandardMaterial3D)mesh1.Mesh.SurfaceGetMaterial(0);
        mesh2 = (MeshInstance3D)GetNode("Cylinder/Cone");
        material2 = (StandardMaterial3D)mesh2.Mesh.SurfaceGetMaterial(0);

        if (material1 == null || material2 == null)
        {
            GD.PrintErr("Mesh3D not found.");
            return;
        }
        timer = GetTree().Root.GetNode<Timer>("Menu/Background/Timer");
        timer.Timeout += ChangeColor;
    }

    private void ChangeColor()
    {
        if(material1.AlbedoColor == Colors.Cyan)
        {
            material1.AlbedoColor = Colors.Yellow;
            material2.AlbedoColor = Colors.Yellow;
        }
        else if(material1.AlbedoColor == Colors.Red)
        {
            material1.AlbedoColor = Colors.Cyan;
            material2.AlbedoColor = Colors.Cyan;
        }
        else
        {
            material1.AlbedoColor = Colors.Red;
            material2.AlbedoColor = Colors.Red;
        }
    }
}
