using Godot;
using System;

public partial class gruntController : baseEnemy
{
    private MeshInstance3D mesh;

    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D blueMaterial;
    [Export]
    public StandardMaterial3D yellowMaterial;
    [Export]
    public StandardMaterial3D redMaterial;

    protected override void OnSpawn()
    {
        mesh = GetNode<MeshInstance3D>("Armature_002/Skeleton3D/Cube_002");
    }

    protected override void SwapType(int type)
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
}
