using Godot;
using System;

public partial class OptionsScript : Control
{
	private Label volumeLabel;
	private Slider volumeSlider;

	private OptionButton displayModeOption;

	public override void _Ready()
	{
		base._Ready();
		volumeLabel = GetNode<Label>("PanelContainer/ColorRect/VolumeLabel");
		ProcessMode = ProcessModeEnum.Always;

		volumeSlider = GetNode<Slider>("PanelContainer/ColorRect/VolumeSlider");
		displayModeOption = GetNode<OptionButton>("PanelContainer/ColorRect/DisplayOptionBar");

		GrabCurrentSettings();
	}

	private void OnBackPressed()
	{
		Visible = false;
	}

	private void OnVolumeSliderChanged(float value)
	{
		volumeLabel.Text = ((int)value).ToString() + "%";
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.LinearToDb(value / 100f));
	}

	private void OnDisplayItemSelected(int index)
	{
		if(index == 0)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
			
		else if(index == 1)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
	}

	private void GrabCurrentSettings()
	{
		// Volume
		float dbVolume = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master"));
		float linearVolume = Mathf.DbToLinear(dbVolume) * 100f;
		volumeSlider.Value = linearVolume;
		volumeLabel.Text = ((int)linearVolume).ToString() + "%";

		// Display Mode
		var mode = DisplayServer.WindowGetMode();
		if(mode == DisplayServer.WindowMode.Windowed)
		{
			displayModeOption.Selected = 0;
		}
		else if(mode == DisplayServer.WindowMode.Fullscreen)
		{
			displayModeOption.Selected = 1;
		}
	}
	
}
