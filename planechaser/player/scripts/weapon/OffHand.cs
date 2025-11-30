using Godot;
using System;

public partial class OffHand : Node3D
{
	// Need to keep track of the original position and rotation of the offhand
	private Vector3 positionOrigin;
	private Vector3 rotationOrigin;
	// Noise Texture is what give the idle sway the randomness
	[Export] public NoiseTexture2D SwayNoise;

	[ExportGroup("Idle Parameters")]
	[Export] public float IdleSwayAdjustment = 10.0f;
	[Export] public float IdleSwayRotationStength = 300.0f;
	[Export] public float RandomSwayAdjustment = 5.0f;
	
	[ExportGroup("Swaying Parameters")]
	[Export] public Vector2 SwayMin = new Vector2(-16.0f, -16.0f);
	[Export] public Vector2 SwayMax = new Vector2(16.0f, 16.0f);
	[Export(PropertyHint.Range, "0, 0.25, 0.01")] public float SwayAmountPosition = 0.1f;
	[Export(PropertyHint.Range, "0, 0.50, 0.1")] public float SwayAmountRotation = 0.5f;
	[Export(PropertyHint.Range, "0, 0.2, 0.01")] public float SwaySpeedPosition = 0.07f;
	[Export(PropertyHint.Range, "0, 0.2, 0.01")] public float SwaySpeedRotation = 0.1f;
	[Export] public float SwaySpeed = 1.2f;
	// Its important that we dont divide by 0.0f
	[Export] public float RandomSwayAmount = 0.5f;

	// Time helps in generating random sway for sin and cos
	private float time = 0.0f;
	private Vector2 MouseMovement = Vector2.Zero;
	private float RandomSwayX;
	private float RandomSwayY;
	
	// Vector for holding the bob values
	private Vector2 bobAmount = Vector2.Zero;


	
    public override void _Ready()
    {
        base._Ready();
		positionOrigin = Position;
		rotationOrigin = Rotation;
    }
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
		if (@event is InputEventMouseMotion)
		{
			// Need to cast event over to InputEventMouseMotion, copy that into a local variable and
			// pass the Relative (mouse deltas between frames) over to MouseMovement 
			InputEventMouseMotion MouseEvent = (InputEventMouseMotion)@event;
			MouseMovement = MouseEvent.Relative;
		}
    }

	public void OffHandBob(double delta, float bobSpeed, float bobH, float bobV)
	{
		// Time gives us a new value always
		time += (float)delta;

		// Sin(X/Y * frequency) * amplitude
		bobAmount.X = Mathf.Sin(time * bobSpeed) * bobH;
		bobAmount.Y = Mathf.Abs(Mathf.Cos(time * bobSpeed) * bobV);
	}

	private  void SwayHelper(ref Vector3 position, ref Vector3 rotationDeg, double delta, bool isMoving, float RandomSwayX = 0.0f, float RandomSwayY = 0.0f, float IdleSwayRotationStength = 0.0f)
	{

		// Lerp weapon position based on mouse movement. Alot of mistypes here (float -> double)
		// If MouseMovement is 0 then the only thing left would be the Position.X/Y - RandomSwayX/Y
		position.X = (float)Mathf.Lerp(position.X, positionOrigin.X - (MouseMovement.X *
			SwayAmountPosition + RandomSwayX + (isMoving ? bobAmount.X : 0.0f)) * delta, SwaySpeedPosition);
		position.Y = (float)Mathf.Lerp(position.Y, positionOrigin.Y - (MouseMovement.Y *
			SwayAmountPosition + RandomSwayY + (isMoving ? bobAmount.Y : 0.0f)) * delta, SwaySpeedPosition);
		// Lerp weapon rotation based on mouse movement
		// Similar concept to position. If MouseMovement.X/Y is 0 then the only thing left would be the
		// currentWeapon.Rotation.Y/X +/- RandomSwayY/X * IdleSwayRotationStrength. This is what causes the idle sway
		rotationDeg.Y = (float)Mathf.Lerp(rotationDeg.Y, rotationOrigin.Y - (MouseMovement.X *
			SwayAmountRotation + (RandomSwayY * IdleSwayRotationStength)) * delta, SwaySpeedRotation);
		rotationDeg.X = (float)Mathf.Lerp(rotationDeg.X, rotationOrigin.X - (MouseMovement.Y *
			SwayAmountRotation + (RandomSwayX * IdleSwayRotationStength)) * delta, SwaySpeedRotation);
        
    }

	private float GetSwayNoise()
	{
		// Default fallback if noise isn’t assigned
		if (SwayNoise == null || SwayNoise.Noise == null)
			return 0.0f;

		Vector3 playerPosition = Vector3.Zero;

		// Only access Globals when in play mode. Grab the current players position
		// Only want to do this while in play mode
		if (!Engine.IsEditorHint() && Globals.player != null)
			playerPosition = Globals.player.GlobalPosition;

		// Pseudo random value
		return SwayNoise.Noise.GetNoise2D(playerPosition.X, playerPosition.Y);
	}

	public void SwayWeapon(double delta, bool isIdle)
	{
		// Return to base position if the mouse is not moving
		MouseMovement = MouseMovement.Lerp(Vector2.Zero, (float)(delta * 5.0));

		// Make sure to clamp the sway ammounts 
		MouseMovement = MouseMovement.Clamp(SwayMin, SwayMax);
		Vector3 position = Position;
		Vector3 rotationDeg = RotationDegrees;

		// Only play random sway when in idle not when moving
		if (isIdle)
		{
			// Noise gives us random values based on position
			float SwayRandom = GetSwayNoise();
			// Will always return a random value based on player position and we tone it down with IdleSwayAdjustment
			float SwayRandomAdjusted = SwayRandom * IdleSwayAdjustment; // Adjust sway strength (factor)

			// create time with delta and set two sine values for x and y
			time += (float)delta * (SwaySpeed + SwayRandom); // Notice how we add Randomization
															 // Create a bit of random sin wave with SwayRandomAdjusted
			// The + and - provide a wave shift for more added randomness
			// The stronger the RandomSwayAmount the less suttle the total sway
			RandomSwayX = (float)Mathf.Sin(time * 1.5 + SwayRandomAdjusted) / RandomSwayAmount;
			RandomSwayY = (float)Mathf.Sin(time - SwayRandomAdjusted) / RandomSwayAmount;

			// ref key word allows to pass arguments by reference
			SwayHelper(ref position, ref rotationDeg, delta, false, RandomSwayX, RandomSwayY, IdleSwayRotationStength);
		}
		else
		{
			SwayHelper(ref position, ref rotationDeg, delta, true);
		}

		// Set the Weapon position and rotation in degrees
		Position = position;
		RotationDegrees = rotationDeg;

	}
}
