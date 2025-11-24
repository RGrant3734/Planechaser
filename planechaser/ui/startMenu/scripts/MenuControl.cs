using Godot;
using System;

public partial class MenuControl : Control
{
    // options ui
    [Export]
    private Control optionsUI;

    [Export]
    private Control tutorialUI;

    private bool isVisible = false;

    private Button startButton;

    private bool isPressing = false;

    public override void _Ready()
    {
        base._Ready();
        optionsUI.Visible = false;
        tutorialUI.Visible = false;
        // constrain mouse
        Input.MouseMode = Input.MouseModeEnum.Captured;

        startButton = GetNode<Button>("MarginContainer/StartButton");
        startButton.ButtonPressed = false;
    }

    public override void _GuiInput(InputEvent @event)
    {
        // Check if the event is a left mouse button press
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                isPressing = true;
                // Visually set the button to the 'pressed' state
                startButton.ButtonPressed = true; 
            }
            
            // Left mouse button released (up)
            if (!mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (isPressing)
                {
                    isPressing = false;
                    // Visually set the button back to the 'normal' state
                    startButton.ButtonPressed = false;
                    
                    // Emit the signal to trigger the button's action logic
                    startButton.EmitSignal(BaseButton.SignalName.Pressed);
                }
            }
        }
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
        tutorialUI.Visible = true;
    }
    public void QuitPressed()
    {
        GetTree().Quit();
    }
}
