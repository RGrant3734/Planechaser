using Godot;
using System;

public partial class ShopScript : Control
{
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public void OnBackPressed()
    {
        animationPlayer.Play("fade_out");
    }
    
    public void OnShopPressed()
    {
        animationPlayer.Play("fade_in");
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "fade_out")
            Visible = false;
    }
}
