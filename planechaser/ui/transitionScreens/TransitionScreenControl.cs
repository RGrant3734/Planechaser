using Godot;
using System;

public partial class TransitionScreenControl : Control
{
    private AnimationPlayer animationPlayer;

    private Button continueButton;

    private bool isPressing = false;

    // font color
    [Export] public Color origFontColor;

    private Tween currentTween;

    private float transitionLength = 0.75f;

    private bool isTweenRunning = false;

    private bool isGray = false;

    // level-determining label
    private Label levelLabel;

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

        continueButton = GetNode<Button>("Control/ContinueButton");

        levelLabel = GetNode<Label>("Control/LevelLabel");
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
                    continueButton.EmitSignal("pressed");
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
            currentTween.TweenProperty(continueButton, "theme_override_colors/font_color", Colors.DimGray, transitionLength);
            isTweenRunning = true;
            isGray = true;
            ChangeTweenRunning();
        }
        else
        {
            currentTween.TweenProperty(continueButton, "theme_override_colors/font_color", origFontColor, transitionLength);
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

    public void OnContinuePressed()
    {
        animationPlayer.Play("fadeOut");
        // on animation finished, change scene to main menu
        animationPlayer.AnimationFinished += OnFadeOutFinished;
    }

    private void OnFadeOutFinished(StringName animName)
    {
        if (animName == "fadeOut")
        {
            if(levelLabel.Text == "L I G H T")
                GetTree().ChangeSceneToFile("res://levels/Level_Light.tscn");
            else if (levelLabel.Text == "S H A D O W")
                GetTree().ChangeSceneToFile("res://levels/Level_Shadow.tscn");
            else if (levelLabel.Text == "B L O O D")
                GetTree().ChangeSceneToFile("res://levels/Level_Blood.tscn");
            else if (levelLabel.Text == "J A I L")
                GetTree().ChangeSceneToFile("res://levels/Level_Jail.tscn");
            else if (levelLabel.Text == "C O U R T Y A R D")
                GetTree().ChangeSceneToFile("res://levels/Level_Courtyard.tscn");
            else if((levelLabel.Text == "SHADOW DEFEATED") || (levelLabel.Text == "LIGHT DEFEATED") || (levelLabel.Text == "BLOOD DEFEATED"))
                GetTree().ChangeSceneToFile("res://levels/Level_Courtyard.tscn");
            else if (levelLabel.Text == "Y O U R   S O U L   I S   L O S T")
                GetTree().ChangeSceneToFile("res://ui/transitionScreens/transition_to_Jail.tscn");
            // otherwise error, return to main menu
            else
                GetTree().ChangeSceneToFile("res://ui/startMenu/scenes/menu.tscn");
        }
    }
}
