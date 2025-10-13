using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Export]
    public float Speed = 5f;
    private GameMaster _GameMaster;
    public override void _Ready()
    {
        base._Ready();
        _GameMaster = GetNode<GameMaster>("/root/GameMaster");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("p_planeshift"))
        {
            int nextdimension = (_GameMaster.currentDimension + 1) % 3;
            _GameMaster.Shift(nextdimension);
        }
    }
    public override void _Process(double delta)
    {
        float hInput = Input.GetAxis("p_left", "p_right");
        float vInput = Input.GetAxis("p_up", "p_down");
        Vector3 desiredV = new Vector3(hInput, 0, vInput).Normalized() * Speed;

        if (!this.IsOnFloor())
        {
            Velocity = Velocity.Lerp(desiredV, .05f) + GetGravity();
        }
        else
        {
            Velocity = Velocity.Lerp(desiredV, 0.5f);
        }
        MoveAndSlide();
    }
}
