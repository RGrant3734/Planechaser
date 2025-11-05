using Godot;
using System;

public partial class MenuControl : Control
{
    // options ui
    [Export]
    private Control optionsUI;

    public override void _Ready()
    {
        base._Ready();
        optionsUI.Visible = false;
    }

    public void OnStartPressed()
    {
        
    }
    public void OptionsPressed()
    {
        optionsUI.Visible = true;
    }
    public void TutorialPressed()
    {
        
    }
    public void QuitPressed()
    {
        GetTree().Quit();
    }
}
