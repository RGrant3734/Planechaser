using Godot;
using System;

public partial class EnvironmentSwapper : StaticBody3D
{
    [Export(PropertyHint.Range, "0, 2,")]
    public int StartTypeNumber = 0;

    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D Plane1Material;
    [Export]
    public StandardMaterial3D Plane2Material;
    [Export]
    public StandardMaterial3D Plane3Material;

    private GameMaster _GameMaster;
    private MeshInstance3D meshInstance;

    public override void _Ready()
    {
        base._Ready();
        _GameMaster = GetNode<GameMaster>("/root/GameMaster");
        _GameMaster.Planeshift += Planeshift;
        meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");

        SwapType(StartTypeNumber);
    }

    public void Planeshift(int currDimension)
    {
        SwapType((currDimension + StartTypeNumber) % 3);
    }
    public void SwapType(int type)
    {
        switch (type)
        {
            case 0:
                meshInstance.SetSurfaceOverrideMaterial(0, Plane1Material);
                break;
            case 1:
                meshInstance.SetSurfaceOverrideMaterial(0, Plane2Material);
                break;
            case 2:
                meshInstance.SetSurfaceOverrideMaterial(0, Plane3Material);
                break;
            default:
                GD.Print("TypeSetError(Environment)");
                break;
        }
    }
}
