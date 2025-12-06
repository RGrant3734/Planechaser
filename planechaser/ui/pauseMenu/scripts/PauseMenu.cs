using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export]
	private Control optionsUI;
	[Export]
	private Control shopUI;

	private AudioStreamPlayer audio;

	public override void _Ready()
	{
		base._Ready();
		optionsUI.Visible = false;
		shopUI.Visible = false;
		Visible = false;
		ProcessMode = ProcessModeEnum.Always;
		// music
		audio = GetNode<AudioStreamPlayer>("GameDevShopTheme");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("exit") && !shopUI.Visible && !optionsUI.Visible)
		{
			Visible = !Visible;
			optionsUI.Visible = false;
		
			GetTree().Paused = !GetTree().Paused;

			// change mouse mode
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}

			if(audio.Playing)
			{
				audio.Stop();
			}
			else
			{
				audio.Play();
			}
		}
	}

	public void OnContinuePressed()
	{
		Visible = false;
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		audio.Stop();
	}

	public void OnShopPressed()
	{
		shopUI.Visible = true;
		Visible = false;
	}

	public void OnOptionsPressed()
	{
		optionsUI.Visible = true;
		Visible = false;
	}

	public void OnQuitPressed()
	{
		GetTree().Quit();
	}

	public void OnBackPressed()
	{
		Visible = true;
	}
}
