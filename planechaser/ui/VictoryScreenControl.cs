using Godot;
using System;

public partial class VictoryScreenControl : Control
{
    private AnimationPlayer animationPlayer;

    private Button returnButton;

    private bool isPressing = false;

    // font color
    [Export] public Color origFontColor;

    private Tween currentTween;

    private float transitionLength = 0.75f;

    private bool isTweenRunning = false;

    private bool isGray = false;

    private AudioStreamPlayer victorySound;

    public override void _EnterTree()
    {
        // Ensure it starts opaque before scene renders
        var fade = GetNode<Control>("Control");
        fade.Modulate = new Color(1, 1, 1, 0);
    }

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;

        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("fadeIn");

        returnButton = GetNode<Button>("Control/ReturnButton");

        victorySound = GetNode<AudioStreamPlayer>("AhhhEpiphany");
        victorySound.Play();
    }

    // Use _Input so we receive mouse events even when controls are set to ignore
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                isPressing = true;
            }

            // Left mouse button released (up)
            if (!mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (isPressing)
                {
                    isPressing = false;

                    // Trigger the button's pressed action
                    returnButton.EmitSignal("pressed");
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(isTweenRunning)
        {
            return;
        }
        if(currentTween != null && currentTween.IsRunning())
        {
            currentTween.Kill();
        }
        currentTween = CreateTween();
        if(!isGray)
        {
            currentTween.TweenProperty(returnButton, "theme_override_colors/font_color", Colors.DimGray, transitionLength);
            isTweenRunning = true;
            isGray = true;
            ChangeTweenRunning();
        }
        else
        {
            currentTween.TweenProperty(returnButton, "theme_override_colors/font_color", origFontColor, transitionLength);
            isTweenRunning = true;
            isGray = false;
            ChangeTweenRunning();
        }
    }

    private async void ChangeTweenRunning()
    {
        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
        isTweenRunning = !isTweenRunning;
    }

    public void OnReturnPressed()
    {
        animationPlayer.Play("fadeOut");
        // on animation finished, change scene to main menu
        animationPlayer.AnimationFinished += OnFadeOutFinished;
    }

    private void OnFadeOutFinished(StringName animName)
    {
        if (animName == "fadeOut")
        {
            GetTree().ChangeSceneToFile("res://ui/startMenu/scenes/menu.tscn");
        }
    }
}
