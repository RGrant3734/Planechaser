using Godot;
using System;

public partial class lightEyeController : CharacterBody3D
{
    [Export(PropertyHint.Range, "0, 2, ")]
    public int startTypeNumber = 0;
    [Export(PropertyHint.Range, "0, 2, ")]
    public int nativePlane = 0;
    [Export]
    public int totalHealth = 100;
    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D blueMaterial;
    [Export]
    public StandardMaterial3D yellowMaterial;
    [Export]
    public StandardMaterial3D redMaterial;
    [Export]
    public MeshInstance3D socketMesh;
    [Export]
    public MeshInstance3D lowerLidMesh;
    [Export]
    public MeshInstance3D upperLidMesh;
    [Export]
    public StandardMaterial3D blueEyeMaterial;
    [Export]
    public StandardMaterial3D yellowEyeMaterial;
    [Export]
    public StandardMaterial3D redEyeMaterial;
    [Export]
    public MeshInstance3D eyeballMesh;
    protected GameMaster gameMaster;
    protected CharacterBody3D player;
    protected bool isDead = false;
    public override void _Ready()
    {
        // intializing important assets
        gameMaster = GetNode<GameMaster>("/root/GameMaster");
        gameMaster.Planeshift += Planeshift;

        player = GetTree().GetFirstNodeInGroup("Player") as CharacterBody3D;
        if (player == null)
        {
            throw new Exception("Expects a player in group Player");
        }
    }

    protected void Planeshift(int currDimension)
    {
        SwapType((currDimension + startTypeNumber) % 3);
    }
    public override void _Process(double delta)
    {
        eyeballMesh.LookAt(player.GlobalPosition, Vector3.Up);
        eyeballMesh.RotateY(Mathf.DegToRad(90));
    }

    protected void SwapType(int type)
    {
        switch (type)
        {
            case 0:
                socketMesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
                upperLidMesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
                lowerLidMesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
                break;
            case 1:
                socketMesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
                upperLidMesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
                lowerLidMesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
                break;
            case 2:
                socketMesh.Mesh.SurfaceSetMaterial(0, redMaterial);
                upperLidMesh.Mesh.SurfaceSetMaterial(0, redMaterial);
                lowerLidMesh.Mesh.SurfaceSetMaterial(0, redMaterial);
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
        switch (nativePlane)
        {
            case 0:
                eyeballMesh.SetSurfaceOverrideMaterial(0, blueEyeMaterial);
                break;
            case 1:
                eyeballMesh.SetSurfaceOverrideMaterial(0, yellowEyeMaterial);
                break;
            case 2:
                eyeballMesh.SetSurfaceOverrideMaterial(0, redEyeMaterial);
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
        if(!isDead)
        {
            switch (nativePlane)
            {
                case 0:
                    if (nativePlane == gameMaster.currentDimension)
                    {
                        upperLidMesh.Visible = false;
                        lowerLidMesh.Visible = false;
                    }
                    else
                    {
                        upperLidMesh.Visible = true;
                        lowerLidMesh.Visible = true;
                    }
                    break;
                case 1:
                    if (nativePlane == gameMaster.currentDimension)
                    {
                        upperLidMesh.Visible = false;
                        lowerLidMesh.Visible = false;
                    }
                    else
                    {
                        upperLidMesh.Visible = true;
                        lowerLidMesh.Visible = true;
                    }
                    break;
                case 2:
                    if (nativePlane == gameMaster.currentDimension)
                    {
                        upperLidMesh.Visible = false;
                        lowerLidMesh.Visible = false;
                    }
                    else
                    {
                        upperLidMesh.Visible = true;
                        lowerLidMesh.Visible = true;
                    }
                    break;
                default:
                    GD.Print("TypeSetError(Enemy)");
                    break;
            }
        }
    }
}
