using Godot;
using System;

public partial class lightHandController : rangedEnemy
{
    private MeshInstance3D staff;

    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D blueWeaponMaterial;
    [Export]
    public StandardMaterial3D yellowWeaponMaterial;
    [Export]
    public StandardMaterial3D redWeaponMaterial;
    protected override void OnSpawn()
    {
        staff = GetNode<MeshInstance3D>("Armature/Skeleton3D/Staff/Staff");
        SwapType(gameMaster.currentDimension);
    }

    public override void _PhysicsProcess(double delta)
    {
    }

    public override void _Process(double delta)
    {
        switch (stateMachine.GetCurrentNode())
        {
            case "Idle":
                sight.TargetPosition = sight.ToLocal(player.GlobalPosition);
                animationTree.Set("parameters/conditions/Attack", InSight());
                break;
            case "Attack":
                animationTree.Set("parameters/conditions/Idle", !InSight());
                break;
            default:
                break;
        }
    }
    protected override void SwapType(int type)
    {
        switch (type)
        {
            case 0:
                mesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
                staff.Mesh.SurfaceSetMaterial(0, blueWeaponMaterial);
                break;
            case 1:
                mesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
                staff.Mesh.SurfaceSetMaterial(0, yellowWeaponMaterial);
                break;
            case 2:
                mesh.Mesh.SurfaceSetMaterial(0, redMaterial);
                staff.Mesh.SurfaceSetMaterial(0, redWeaponMaterial);
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
    }
    //Enemy takes damage in their special ways and dies
    public override void Hit(string weapon, float damage)
    {
        //nothing should happen here
    }
}
