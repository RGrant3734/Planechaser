using Godot;
using System;

public partial class OptionsScript : Control
{
    private Label volumeLabel;

    public override void _Ready()
    {
        base._Ready();
        volumeLabel = GetNode<Label>("PanelContainer/ColorRect/VolumeLabel");
        ProcessMode = ProcessModeEnum.Always;
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

}
