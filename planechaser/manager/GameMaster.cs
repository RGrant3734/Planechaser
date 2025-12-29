using Godot;
using System;

public partial class GameMaster : Node3D
{
	[Export]
	public int gruntMax = 5;
	[Export]
	public int knightMax = 5;
	[Export]
	public int mageMax = 5;
	[Signal]
	public delegate void PlaneshiftEventHandler(int nextdimension);
	[Signal]
	public delegate void SpawnEventHandler();
	public bool spawning = false;
	public int currentDimension = 0;
	public int numDimension = 3;
	public int gruntNum = 0;
	public int knightNum = 0;
	public int mageNum = 0;
	public Node blue;
	public Node yellow;
	public Node red;
	public NavigationRegion3D region;
	public bool offCooldown = true;
	public float planeshiftCooldown = 5;
	public AudioStreamPlayer shiftSound;
	// Animation for fading
	private AnimationPlayer animationPlayer;

	public bool levelCompleted = false;
	public int bloodDeathCount = 0;
	
	// Random Number Generator
	public RandomNumberGenerator randomFloat = new RandomNumberGenerator();
	public override void _EnterTree()
	{
		// Ensure it starts opaque before scene renders
		var fade = GetNode<ColorRect>("FadeScreen");
		fade.Visible = true;
		fade.Modulate = new Color(0, 0, 0, 1);
	}
	public override void _Ready()
	{
		EmitSignal(SignalName.Planeshift, currentDimension);
		region = GetNode<NavigationRegion3D>("NavigationRegion3D");
		blue = GetNode<Node>("NavigationRegion3D/Blue");
		yellow = GetNode<Node>("NavigationRegion3D/Yellow");
		red = GetNode<Node>("NavigationRegion3D/Red");
		shiftSound = GetNode<AudioStreamPlayer>("Shift");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		// add animation finished signal handle for flash in
		animationPlayer.AnimationFinished += OnFlashInAnimationFinished;
		animationPlayer.Play("fadeIn");
	}
	public override void _Input(InputEvent @event)
	{
		if (offCooldown && @event.IsActionPressed("planeshift"))
		{
			animationPlayer.Play("flashIn");
		}
	}
	// Shifts to the next dimension
	public void Shift(int newDimension)
	{
		shiftSound.Play();
		currentDimension = newDimension;
		EmitSignal(SignalName.Planeshift, newDimension);
		// Nav region rebaking for objects that swap
		//objects that swap need to be children of the Node nodes (Blue, Yellow, Red)
		switch (currentDimension)
		{
			case 0:
				blue.Reparent(this);
				red.Reparent(region);
				break;
			case 1:
				yellow.Reparent(this);
				blue.Reparent(region);
				break;
			case 2:
				red.Reparent(this);
				yellow.Reparent(region);
				break;
		}
		region.BakeNavigationMesh();
	}
	public void StartSpawn()
	{
		spawning = true;
		EmitSignal(SignalName.Spawn);
	}
	public async void PlaneshiftCooldown()
	{
		offCooldown = false;
		await ToSignal(GetTree().CreateTimer(planeshiftCooldown), "timeout");
		offCooldown = true;
	}
  
	public void OnFlashInAnimationFinished(StringName animName)
	{
		if (animName != "flashIn") return;
		int nextdimension = (currentDimension + 1) % 3;
		Shift(nextdimension);
		PlaneshiftCooldown();
		animationPlayer.Play("flashOut");
	}
}
