using Godot;
using System;

public partial class shadowController : rangedEnemy
{
    private MeshInstance3D staff;
    private MeshInstance3D robe;

    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D blueWeaponMaterial;
    [Export]
    public StandardMaterial3D yellowWeaponMaterial;
    [Export]
    public StandardMaterial3D redWeaponMaterial;
    [Export]
    public StandardMaterial3D blueRobeMaterial;
    [Export]
    public StandardMaterial3D yellowRobeMaterial;
    [Export]
    public StandardMaterial3D redRobeMaterial;
    protected ShapeCast3D floorDetection;
    protected int rangedCount = 0;
    [Signal]
    public delegate void LightningCallEventHandler();
    protected override void OnSpawn()
    {
        staff = GetNode<MeshInstance3D>("Armature/Skeleton3D/Staff/Staff");
        robe = GetNode<MeshInstance3D>("Armature/Skeleton3D/Robe");
        floorDetection = GetNode<ShapeCast3D>("FloorDetection");
        SwapType(gameMaster.currentDimension);
    }

    public override void _Process(double delta)
    {
        switch (stateMachine.GetCurrentNode())
        {
            case "Idle":
                if(!gameMaster.spawning)
                    break;

                animationTree.Set("parameters/conditions/Attack", InMeleeRange());
                animationTree.Set("parameters/conditions/Lightning", rangedCount >= 3);
                animationTree.Set("parameters/conditions/Ranged", InSight());
                LookAt(new Vector3(player.GlobalPosition.X * -1, player.GlobalPosition.Y, player.GlobalPosition.Z * -1), Vector3.Up);
                sight.TargetPosition = sight.ToLocal(player.GlobalPosition);
                break;
            case "Attack":
                animationTree.Set("parameters/conditions/Idle", !InMeleeRange());
                break;
            case "Lightning":
                rangedCount = 0;
                break;
            case "Death":
                if (dead)
                {
                    QueueFree();
                }
                break;
            case "Ranged":
                rangedCount++;
                animationTree.Set("parameters/conditions/Idle", true);
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
                robe.Mesh.SurfaceSetMaterial(0, blueRobeMaterial);
                robe.Mesh.SurfaceSetMaterial(1, yellowRobeMaterial);
                break;
            case 1:
                mesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
                staff.Mesh.SurfaceSetMaterial(0, yellowWeaponMaterial);
                robe.Mesh.SurfaceSetMaterial(0, yellowRobeMaterial);
                robe.Mesh.SurfaceSetMaterial(1, redRobeMaterial);
                break;
            case 2:
                mesh.Mesh.SurfaceSetMaterial(0, redMaterial);
                staff.Mesh.SurfaceSetMaterial(0, redWeaponMaterial);
                robe.Mesh.SurfaceSetMaterial(0, redRobeMaterial);
                robe.Mesh.SurfaceSetMaterial(1, blueRobeMaterial);
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
    }
    //Enemy takes damage in their special ways and dies
    public override void Hit(int weaponPlane, float baseDamage)
    {   
        // If the current weapon's native plane matches the current level plane then do bonus damage
        if(weaponPlane == gameMaster.currentDimension)
            currentHealth -= baseDamage * 1.5f;
        else
            currentHealth -= baseDamage;

        if(currentHealth <= 0)
        {
            animationTree.Set("parameters/conditions/Death", true);
        } else {
            animationTree.Set("parameters/conditions/Hit", true);
        }

    }

    public void LightningAttack()
    {
        EmitSignal(SignalName.LightningCall);
    }
}
