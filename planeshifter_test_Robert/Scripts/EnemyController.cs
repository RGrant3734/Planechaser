using Godot;
using System;

public partial class EnemyController : CharacterBody3D
{
    [Export(PropertyHint.Range, "0, 2,")]
    public int StartTypeNumber = 0;
    private GameMaster _GameMaster;

    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D type1Material;
    [Export]
    public StandardMaterial3D type2Material;
    [Export]
    public StandardMaterial3D type3Material;

    private CollisionShape3D type1Collsion;
    private CollisionShape3D type2Collsion;
    private CollisionShape3D type3Collsion;
    private MeshInstance3D type1Mesh;
    private MeshInstance3D type2Mesh;
    private MeshInstance3D type3Mesh;

    public override void _Ready()
    {
        base._Ready();
        _GameMaster = GetNode<GameMaster>("/root/GameMaster");
        _GameMaster.Planeshift += Planeshift;

        type1Collsion = GetNode<CollisionShape3D>("Type1Collision");
        type2Collsion = GetNode<CollisionShape3D>("Type2Collision");
        type3Collsion = GetNode<CollisionShape3D>("Type3Collision");

        type1Mesh = GetNode<MeshInstance3D>("Type1Mesh");
        type2Mesh = GetNode<MeshInstance3D>("Type2Mesh");
        type3Mesh = GetNode<MeshInstance3D>("Type3Mesh");

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
                type1Collsion.Disabled = false;
                type2Collsion.Disabled = true;
                type3Collsion.Disabled = true;
                type1Collsion.Visible = true;
                type2Collsion.Visible = false;
                type3Collsion.Visible = false;
                break;
            case 1:
                type1Collsion.Disabled = true;
                type2Collsion.Disabled = false;
                type3Collsion.Disabled = true;
                type1Collsion.Visible = false;
                type2Collsion.Visible = true;
                type3Collsion.Visible = false;
                break;
            case 2:
                type1Collsion.Disabled = true;
                type2Collsion.Disabled = true;
                type3Collsion.Disabled = false;
                type1Collsion.Visible = false;
                type2Collsion.Visible = false;
                type3Collsion.Visible = true;
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
    }
}
