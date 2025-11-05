using Godot;
using System;

public partial class GruntController : CharacterBody3D
{
    // Set starting plane number (can be used for offsets like blue in red if feature ever needed)
    [Export(PropertyHint.Range, "0, 2,")]
    public int StartTypeNumber = 0;
    private GameMaster _GameMaster;
    private CharacterBody3D player;
    public NavigationAgent3D nav;
    public MeshInstance3D mesh;
    // References to the changing parts for easier access
    [ExportGroup("Materials")]
    // Colors
    [Export]
    public StandardMaterial3D type1Material;
    [Export]
    public StandardMaterial3D type2Material;
    [Export]
    public StandardMaterial3D type3Material;

    // Base stats that can be altered for some stronger version
    [ExportGroup("Character")]
    [Export]
    public int totalHealth = 100;
    [Export]
    public float damage = 10;
    [Export]
    public float attackSpeed = 1;
    [Export]
    public float speed = 5;

    // Hidden stats
    private int currentHealth;

    public override void _Ready()
    {
        base._Ready();
        _GameMaster = GetNode<GameMaster>("/root/GameMaster");
        _GameMaster.Planeshift += Planeshift;
        mesh = GetNode<MeshInstance3D>("Armature/Skeleton3D/MeshInstance3D");
        player = GetTree().GetFirstNodeInGroup("Player") as CharacterBody3D;

        nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
        if (nav == null)
        {
            throw new Exception("Expects a navigation agent 3D as a child with default name");
        }

        currentHealth = totalHealth;
        SwapType(StartTypeNumber);
        GD.Print("Test1: ", nav.TargetPosition);
        //nav.TargetPosition = player.GlobalPosition;
        GD.Print("Test2: ", nav.TargetPosition);
    }

    public override void _Process(double delta)
    {
        if (player == null)
        {
            return;
        }
        nav.TargetPosition = player.GlobalPosition;
        Vector3 nextPosition = nav.GetNextPathPosition();
        Vector3 desiredVelocity = (nextPosition - GlobalPosition).Normalized() * speed;
        Velocity = Velocity.Lerp(desiredVelocity, 0.4f);
        MoveAndSlide();
    }

    // Function that triggers on planeshift (features can be added here)
    public void Planeshift(int currDimension)
    {
        SwapType((currDimension + StartTypeNumber) % 3);
    }

    // Function to enable/disable properties for each type
    public void SwapType(int type)
    {
        switch (type)
        {
            case 0:
                mesh.Mesh.SurfaceSetMaterial(0, type1Material);
                break;
            case 1:
                mesh.Mesh.SurfaceSetMaterial(0, type2Material);
                break;
            case 2:
                mesh.Mesh.SurfaceSetMaterial(0, type3Material);
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
    }

    // Player tracking
    private void _on_vision_body_entered(Node body)
    {
        if (body.IsInGroup("player"))
        {
            GD.Print("Player Seen");
        }
    }

    // Hitbox for projectiles
    private void _on_hit_box_area_entered(Area3D area)
    {
        // multi for color matching, or it will just do base damage
        int multi = 1;
        if (_GameMaster.currentDimension == 0 && area.IsInGroup("blue"))
        {
            GD.Print("Blue Match");
            multi += 1;
        }
        else if (_GameMaster.currentDimension == 1 && area.IsInGroup("yellow"))
        {
            GD.Print("Yellow Match");
            multi += 1;
        }
        else if (_GameMaster.currentDimension == 2 && area.IsInGroup("red"))
        {
            GD.Print("Red Match");
            multi += 1;
        }
        // 4 should be swapped to area.bulletDamage or similar
        currentHealth -= 4 * multi;
    }
}
