using Godot;
using System;

public partial class CreditsTab : ColorRect
{
    public override void _Ready()
    {
        base._Ready();
        Visible = false;
    }

    public void OnCreditsPressed()
    {
        Visible = true;
    }

    public void OnBackPressed()
    {
        Visible = false;
    }
}
