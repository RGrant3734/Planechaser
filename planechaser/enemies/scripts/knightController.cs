using Godot;
using System;

public partial class knightController : meleeEnemy
{
    private MeshInstance3D sword;
    private MeshInstance3D hilt;
    private MeshInstance3D helmet;
    private MeshInstance3D chest;
    private MeshInstance3D legL;
    private MeshInstance3D legR;

    [Export(PropertyHint.Range, "0, 2,")]
    public int armorPlane = 0;

    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D blueArmorMaterial;
    [Export]
    public StandardMaterial3D yellowArmorMaterial;
    [Export]
    public StandardMaterial3D redArmorMaterial;
    [Export]
    public StandardMaterial3D blueWeaponMaterial;
    [Export]
    public StandardMaterial3D yellowWeaponMaterial;
    [Export]
    public StandardMaterial3D redWeaponMaterial;
    protected ShapeCast3D floorDetection;
    protected override void OnSpawn()
    {
        mesh = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Body");
        sword = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Sword/Sword");
        hilt = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Hilt/Hilt");
        helmet = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Helmet");
        chest = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Chest");
        legL = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/LegLeft");
        legR = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/LegRight");
        floorDetection = GetNode<ShapeCast3D>("FloorDetection");
        SwapType(gameMaster.currentDimension);
    }

    public override void _Process(double delta)
    {
        switch(stateMachine.GetCurrentNode())
        {
            case "Idle":
                animationTree.Set("parameters/conditions/Walk", true);
                break;
            case "Walk":
                //Falls down if nothing under it
                if (!floorDetection.IsColliding())
                {
                    stateMachine.Travel("Fall");
                    break;
                }
                //Movement towards player
                Vector3 desiredDirection = Vector3.Zero;
                nav.TargetPosition = player.GlobalPosition;
                desiredDirection = (nav.GetNextPathPosition() - GlobalPosition).Normalized();
                Velocity = Velocity.Lerp(desiredDirection * moveSpeed, .4f);
                moveVal = moveVal.Lerp(new Godot.Vector3(1, 0, 0), .4f);
                LookAt(GlobalPosition + (Velocity * -1), Vector3.Up);
                animationTree.Set("parameters/conditions/Attack", InMeleeRange());
                MoveAndSlide();
                break;
            case "Attack":
                animationTree.Set("parameters/conditions/Walk", !InMeleeRange());
                break;
            case "Hit":
                LookAt(GlobalPosition + (Velocity * -1), Vector3.Up);
                animationTree.Set("parameters/conditions/Hit", false);
                break;
            case "Death":
                if (dead)
                {
                    QueueFree();
                }
                break;
            case "Fall":
                //Disable nav movement
                nav.SetVelocity(Vector3.Zero);
                nav.AvoidanceEnabled = false;

                //Let physics take over
                Velocity = GetGravity();
                MoveAndSlide();

                //When floor detected again, resume walk
                if (floorDetection.IsColliding())
                {
                    nav.AvoidanceEnabled = true;
                    stateMachine.Travel("Walk");
                }
                break;
            default:
                break;
        }
    }

    protected override void SwapType(int type)
    {
        // Weapon color swaps with plane
        switch (type)
        {
            case 0:
                mesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
                sword.Mesh.SurfaceSetMaterial(0, blueWeaponMaterial);
                hilt.Mesh.SurfaceSetMaterial(0, blueWeaponMaterial);
                break;
            case 1:
                mesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
                sword.Mesh.SurfaceSetMaterial(0, yellowWeaponMaterial);
                hilt.Mesh.SurfaceSetMaterial(0, yellowWeaponMaterial);
                break;
            case 2:
                mesh.Mesh.SurfaceSetMaterial(0, redMaterial);
                sword.Mesh.SurfaceSetMaterial(0, redWeaponMaterial);
                hilt.Mesh.SurfaceSetMaterial(0, redWeaponMaterial);
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
        // Sets armor for native plane
        switch (armorPlane)
        {
            case 0:
                helmet.SetSurfaceOverrideMaterial(0, blueArmorMaterial);
                chest.SetSurfaceOverrideMaterial(0, blueArmorMaterial);
                legL.SetSurfaceOverrideMaterial(0, blueArmorMaterial);
                legR.SetSurfaceOverrideMaterial(0, blueArmorMaterial);
                break;
            case 1:
                helmet.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
                chest.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
                legL.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
                legR.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
                break;
            case 2:
                helmet.SetSurfaceOverrideMaterial(0, redArmorMaterial);
                chest.SetSurfaceOverrideMaterial(0, redArmorMaterial);
                legL.SetSurfaceOverrideMaterial(0, redArmorMaterial);
                legR.SetSurfaceOverrideMaterial(0, redArmorMaterial);
                break;
            default:
                GD.Print("TypeSetError(armorPlane)");
                break;
        }
        // Hides armor if on native plane
        if (type == armorPlane)
        {
            helmet.Visible = false;
            chest.Visible = false;
            legL.Visible = false;
            legR.Visible = false;
        }
        else
        {
            helmet.Visible = true;
            chest.Visible = true;
            legL.Visible = true;
            legR.Visible = true;
        }
    }
}
