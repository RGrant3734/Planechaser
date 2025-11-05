using Godot;
using System;

public partial class MenuControl : Control
{
    // options ui
    [Export]
    private Control optionsUI;

    private bool isVisible = false;

    public override void _Ready()
    {
        base._Ready();
        optionsUI.Visible = false;
        // constrain mouse
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public void OnStartPressed()
    {
        if(!isVisible)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
            isVisible = true;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
            isVisible = false;
        }
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
