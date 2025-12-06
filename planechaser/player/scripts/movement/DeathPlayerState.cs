using Godot;
using System;

public partial class DeathPlayerState : PlayerMovementState
{
	// needs to be await
	public override void Enter(State prevState)
    {
		WEAPON.PlayerDeath();
        ANIMATION.Play("Death", -1.0f, 1.0f);
        PLAYER.deathSound.Play();
		// Once animation is done playing then send a signal to game state to notify that the player is dead
    }

	public override void Update(double delta)
    {
        base.Update(delta);
        PLAYER.UpdateGravity(delta);
        // Notice how we dont run UpdateInput. This is because we want to lock out the player when dead
	}
}
