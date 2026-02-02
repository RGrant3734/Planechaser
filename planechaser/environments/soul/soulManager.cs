using Godot;
using System;

public partial class soulManager : Node3D
{
	[Export]
	public bool shadow = false;
	[Export]
	public bool light = false;
	[Export]
	public bool blood = false;
	public MeshInstance3D shadowFragment;
	public MeshInstance3D lightFragment;
	public MeshInstance3D bloodFragment;
	[Export]
	public SpotLight3D soulLight;
	private float time = 0.0f;
	private Vector3 position;
	private float origin = 0.0f;
	[Export] public float frequency = 3.0f;
	[Export] public float amplitude = 0.3f;
	[Export] public float angle = 3.0f;
	public float percentSpeed = .2f;
	protected GameMaster gameMaster;
	public PathFollow3D path;
	public override void _Ready()
	{
		gameMaster = GetNode<GameMaster>("/root/GameMaster");
		path = (PathFollow3D)GetParent();
		position = Position;
		origin = Position.Y;
		shadowFragment = GetNode<MeshInstance3D>("ShadowFragment");
		lightFragment = GetNode<MeshInstance3D>("LightFragment");
		bloodFragment = GetNode<MeshInstance3D>("BloodFragment");
		shadowFragment.Visible = shadow;
		lightFragment.Visible = light;
		bloodFragment.Visible = blood;

	}

	public override void _Process(double delta)
	{
		if(gameMaster.levelCompleted && path.ProgressRatio <= .95)
		{
			soulLight.Visible = true;
			path.ProgressRatio += percentSpeed * (float)delta;
		}
		time += (float)delta;
		position.Y = origin + Mathf.Sin(time * frequency) * amplitude;
		// Update the position and rotate it to the according angle
		Position = position;
		Rotate(Transform.Basis.Y.Normalized(), Mathf.DegToRad(angle));
	}

	public void BodyEntered(Node3D body)
	{
		if (body.IsInGroup("Player"))
		{
			if(shadow)
				GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_from_Shadow.tscn");
			if(light)
				GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_from_Light.tscn");
			if(blood)
				GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_from_Blood.tscn");
		}
	}
}
