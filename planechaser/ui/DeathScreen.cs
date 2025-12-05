using Godot;
using System;

public partial class DeathScreen : Control
{
    private AnimationPlayer animPlayer;
    private Node player;
    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Visible = false;
        player = GetTree().Root.GetNode<Node>("GameMaster/Player");
        if (player != null)
        {
            // Connect to the custom signal
            player.Connect("MyCustomSignal", new Callable(this, nameof(OnPlayerDead)));
        }

    }

    private void OnPlayerDead(string message)
    {
        if (message == "PlayerDied")
        {
            Visible = true;
            animPlayer.Play("glitchIn");
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    private void OnTryAgainPressed()
    {
        Globals.PlayerReset();
        GetTree().ReloadCurrentScene();
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
}
