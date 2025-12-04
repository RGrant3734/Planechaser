using Godot;
using System;

public partial class enemySoulManager : Node3D
{
    public MeshInstance3D shadowFragment;
    public MeshInstance3D lightFragment;
    public MeshInstance3D bloodFragment;
	[Export] public float angle = 3.0f;
    protected GameMaster gameMaster;
    public override void _Ready()
    {
        gameMaster = GetNode<GameMaster>("/root/GameMaster");
        shadowFragment = GetNode<MeshInstance3D>("ShadowFragment");
        lightFragment = GetNode<MeshInstance3D>("LightFragment");
        bloodFragment = GetNode<MeshInstance3D>("BloodFragment");
        if(gameMaster.currentDimension != 0)
            shadowFragment.Visible = false;
        if(gameMaster.currentDimension != 1)
            lightFragment.Visible = false;
        if(gameMaster.currentDimension != 2)
            bloodFragment.Visible = false;
    }

    public override void _Process(double delta)
    {
		Rotate(Transform.Basis.Y.Normalized(), Mathf.DegToRad(angle));
    }
}
