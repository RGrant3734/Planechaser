using Godot;
using System;

public partial class PauseMenu : Control
{
    [Export]
    private Control optionsUI;
    [Export]
    private Control shopUI;

    public override void _Ready()
    {
        base._Ready();
        optionsUI.Visible = false;
        shopUI.Visible = false;
        Visible = false;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause") && !shopUI.Visible && !optionsUI.Visible)
        {
            Visible = !Visible;
            optionsUI.Visible = false;
        }
    }

    public void OnContinuePressed()
    {
        Visible = false;
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
