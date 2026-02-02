using Godot;
using System;

public partial class LevelManager : Area3D
{
    [Export]
	public bool toShadow = false;
	[Export]
	public bool toLight = false;
	[Export]
	public bool toBlood = false;
	[Export]
	public bool toJail = false;
    [Export]
	public bool toCourtyard = false;
    [Export]
	public bool gameOver = false;
	protected GameMaster gameMaster;
	public override void _Ready()
	{
		gameMaster = GetNode<GameMaster>("/root/GameMaster");
	}

	public void BodyEntered(Node3D body)
	{
		if (body.IsInGroup("Player"))
		{
            if (toLight)
            {
                GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_to_Light.tscn");
                return;
            }

			if(toShadow)
            {
				GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_to_Shadow.tscn");
                return;
            }

			if(toBlood)
            {
				GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_to_Blood.tscn");
                return;
            }

            if(toJail)
            {
				GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_to_Jail.tscn");
                return;
            }

            if(toCourtyard)
            {
				GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_to_Courtyard.tscn");
                return;
            }

			if(gameOver)
            {
				GetTree().ChangeSceneToFile("res://ui/victory_screen.tscn");
                return;
            }
		}
	}
}
