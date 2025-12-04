using Godot;
using System;

public partial class GameMaster : Node3D
{
	[Signal]
	public delegate void PlaneshiftEventHandler(int nextdimension);
	[Signal]
	public delegate void SpawnEventHandler();
	public bool spawning = false;
	public int currentDimension = 0;
	public int numDimension = 3;
	public Node blue;
	public Node yellow;
	public Node red;
	public NavigationRegion3D region;
	public bool offCooldown = true;
	public float planeshiftCooldown = 5;

	// Animation for fading
	private AnimationPlayer animationPlayer;

	// Starts with shift to make sure all is on the same plane
	public override void _Ready()
	{
		EmitSignal(SignalName.Planeshift, currentDimension);
		region = GetNode<NavigationRegion3D>("NavigationRegion3D");
		blue = GetNode<Node>("NavigationRegion3D/Blue");
		yellow = GetNode<Node>("NavigationRegion3D/Yellow");
		red = GetNode<Node>("NavigationRegion3D/Red");
    
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		// fade in the game
		animationPlayer.Play("fadeIn");
	}
	public override void _Input(InputEvent @event)
	{
		if (offCooldown && @event.IsActionPressed("planeshift"))
		{
			int nextdimension = (currentDimension + 1) % 3;
			Shift(nextdimension);
			PlaneshiftCooldown();
		}
	}
	// Shifts to the next dimension
	public void Shift(int newDimension)
	{
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
}
