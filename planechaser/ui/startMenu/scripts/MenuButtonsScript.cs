using Godot;
using System;

public partial class MenuButtonsScript : Button
{
    public override void _Ready()
    {
        base._Ready();
        Visible = false;
    }

    private async void OnStartPressed()
    {
        if(Visible)
        {
            Visible = !Visible;
            return;
        }
        await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
        Visible = !Visible;
    }
}
