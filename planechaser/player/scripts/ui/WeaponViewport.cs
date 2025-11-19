using Godot;
using System;

public partial class WeaponViewport : SubViewport
{
	private Vector2I ScreenSize;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Basic script that automatically sets the subwiewport window 
		// to match the window size of the game without needing to adjust it manually
		ScreenSize = GetWindow().Size;
		Size = ScreenSize;
    }
}
