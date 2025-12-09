using Godot;
using System;
using System.Data;


public partial class FPSController : CharacterBody3D
{
	/* Player stats */
	//-------------------------------------------
	[ExportGroup("Player Stats")]
	public float health = 0.0f;
	[Export] public float healthCapacity = 100.0f;
	public float armor = 0.0f;
	[Export] public float armorCapacity = 25.0f;
	[Export]public float speed = 6.0f;
  
  	/*Player Fragment Particle Count*/
	[Export] public int fragmentParticleCount = 0;
	
	/* Mouse parameters */
	//-------------------------------------------
	[ExportGroup("Mouse Parameters")]
	[Export] public float MouseSensitivity = 0.1f;
	// How far down we can look
	[Export] public float TiltLowerLimit { get; set; } = Mathf.DegToRad(-90.0f);
	// How far up we can look
	[Export] public float TiltUpperLimit { get; set; } = Mathf.DegToRad(90.0f);
	/* Camera settings */
	//-------------------------------------------
	[ExportGroup("Camera Settings")]
	// Camera controller that we will manipulate in script
	[Export] public Camera3D WORLDCAMERA { get; set; }
	[Export] public float DefaultFov = 120.0f;
	// Help detect if mouse is moving
	private bool mouseInput = false;
	// unsanitized mouse rotaition
	private Vector3 mouseRotation;
	// Total Y rotation since last frame
	private float rotationInput;
	// Total X rotation since last frame
	private float tiltInput;
	// Horizontal player rotation
	private Vector3 playerRotation;
	// vertical camera rotation
	private Vector3 cameraRotation;
	// Used by sliding state
	public float _currentRotation;
	private bool isArmorDestroyed = false;

	private ProgressBar healthBar;
	private ProgressBar armorBar;
	private Label healthLabel;
	private Label armorLabel;
  	private Label fragmentParticleLabel;
	private Node shop;

	[ExportGroup("Audio")]
	[Export] private AudioStreamPlayer3D armorHit;
	[Export] private AudioStreamPlayer3D healthHit;
	[Export] public AudioStreamPlayer3D deathSound;


	[Signal]
	public delegate void MyCustomSignalEventHandler(string message);
	[Signal]
	public delegate void FragmentParticleSpendResultEventHandler(bool success, int newCount);


	/* PLAYER API */
	//------------------------------------------
	[ExportGroup("Player API")]
	// Animation player node
	[Export] public AnimationPlayer ANIMATION;
	// Sphere shapecast above the player
	[Export] public  ShapeCast3D crouchShapeCast;
	// Reference so that the movement states are able to access the WeaponController
	// Player acts as the middle man between the movement and weapon states
	[Export] public WeaponController WEAPON;
	[Export] public OffHand OFFHAND;

	public override void _Ready()
	{
		base._Ready();

		// Make this script globally accessible
		Globals.player = this;
	
		// Set the mouse to capture mode off rip. This will capture the mouse
		// to be at the center of the screen. We then want the player and camera to rotate with it
		Input.MouseMode = Input.MouseModeEnum.Captured;
		// Set the camera fov accordingly
		WORLDCAMERA.Fov = DefaultFov;
		// Added shapecast exception. We want the shapecast to ignore ourselfs. Couls have done this with layers
		crouchShapeCast.AddException(this);

		// Grab stat related references
		healthBar = GetNode<ProgressBar>("UserInterface/HealthBar");
		healthBar.MaxValue = healthCapacity;
		armorBar = GetNode<ProgressBar>("UserInterface/ArmorBar");
		armorBar.MaxValue = armorCapacity;
		healthLabel = GetNode<Label>("UserInterface/HealthBar/HealthCount");
		armorLabel = GetNode<Label>("UserInterface/ArmorBar/ArmorCount");
		fragmentParticleLabel = GetNode<Label>("UserInterface/FragmentParticleNum");
	  
		shop = GetTree().Root.GetNode<Node>("GameMaster/ShopMenu");
		if (shop != null)
		{
			// Shop will emit a spend request like this: EmitSignal("FragmentParticleSpendRequested", cost);
			// We connect to that signal here so the Player can handle spending logic
			shop.Connect("FragmentParticleSpendRequested", new Callable(this, nameof(OnShopSpendRequested)));
		}

		// Set the players base health and armor accordingly
		health = healthCapacity;
		armor = armorCapacity;

		// Sync the player values specified in the editor to the global player values.
		// from now on we use these values to keep values persistent
		Globals.SyncFromPlayer();
		UpdateStatLabels();
	}

	// _Input > UI > _UnhandledInput. We use _UnhandledInput here since we dont want any mouse movement
	// when we focus on a UI, menu, or button.
	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		
		// Determine if the mouse is captured and is moving
		mouseInput = (@event is InputEventMouseMotion) && (Input.MouseMode == Input.MouseModeEnum.Captured);

		// Screen space to world space
		if (mouseInput)
		{
			// Grab the MouseMotionEvent 
			InputEventMouseMotion motionEvent = (InputEventMouseMotion)@event;
			// Converting mouse movement into radians that we will pass over to the player and camera
			// In this case we are grabbing the total ammount the mouse moved since the last frame
			// This is cornverting to radians per pixel (MouseSensitivity). From here we decide to use radians or degrees

			// How much has the mouse moved in the last frame. Convert that into rad * pixel

			// Its important that we negate these values because turning right in screen space is + but in world space will
			// be negative. Thats why we take the screen space rotation and negate it over to world space rotation 
			rotationInput = -motionEvent.Relative.X * MouseSensitivity;
			tiltInput = -motionEvent.Relative.Y * MouseSensitivity;

			//GD.Print($"({rotationInput}, {tiltInput})");
		}
	}

	private void UpdateCamera(double delta)
	{
		// Grab the current side to side rotation. This will be used by the sliding state
		// to determine the tilt ammount
		_currentRotation = rotationInput;

		// Technically its the x axis thats going up and down. mouseRotation is a vector3 and we are essentially
		// just storing the total rotation that will be applied to the player and camera
		// grab increment to current tilt rotation and clamp it 
		mouseRotation.X += tiltInput * (float)delta;
		// Make sure to clamp vertical rotation
		// Do DegToRad twice in the case that the default values are changed
		mouseRotation.X = Mathf.Clamp(mouseRotation.X, Mathf.DegToRad(TiltLowerLimit), Mathf.DegToRad(TiltUpperLimit));
		// Horizontal rotation
		mouseRotation.Y += rotationInput * (float)delta;

		// Form vectors to be applied to the player and camera rotations respectively
		// If we look horizontally we want the player to rotate which will rotate the camera with it since its a child
		playerRotation = new Vector3(0.0f, mouseRotation.Y, 0.0f);
		// We only want the camera to pitch up and down
		cameraRotation = new Vector3(mouseRotation.X, 0.0f, 0.0f);

		// Camera rotation, want up and down rotation
		WORLDCAMERA.Rotation = cameraRotation;
		// Player rotation, want horizontal rotation
		Basis = Basis.FromEuler(playerRotation);

		// Dont want previous frame rotation inputs to affect the current frame accidentally so we set them to 0.0f
		rotationInput = 0.0f;
		tiltInput = 0.0f;

	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		// Helper input for exiting the game more easily
		//if (@event.IsActionPressed("hardExit"))
			//GetTree().Quit();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		// Its essential to place camera movement in process that way we get snappy and precise input as compared to _PhysicsProcess
		UpdateCamera(delta);
		// Smoothly update health bar. No need for delta here just a float
		if(healthBar.Value > Globals.PlayerHealth || healthBar.Value < Globals.PlayerHealth)
		{	
			healthBar.Value = Mathf.Lerp(healthBar.Value, Globals.PlayerHealth, 0.75f);
		}
		if(armorBar.Value > Globals.PlayerArmor || armorBar.Value < Globals.PlayerArmor)
		{
			armorBar.Value = Mathf.Lerp(armorBar.Value, Globals.PlayerArmor, 0.75f);
		}
	}

	// Callable by our state scripts. The idea is that they can call these functions and customize speed properties
	public void UpdateGravity(double delta)
	{
		// Only add gravity when in the air
		if (!IsOnFloor())
		{	
			// Its essential to only update the players Velocity and keep vel local 
			Vector3 velocity = Velocity;
			velocity += GetGravity() * (float)delta;
			Velocity = velocity;
		}
	}

	public void UpdateInput(float speed, float acceleration, float deceleration)
	{
		//Globals.debug.AddDebugProperty("Movement", speed, 0, new Color(1, 0, 0));

		Vector3 velocity = Velocity;
		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		// want the direction to always be in terms of the local characters coordinate/basis. If we rotate
		// and move forward the input will rotate the same amount and move forward
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			// You will reach the maximum speed over time (linearly) and not instantly
			// Its important for the first argument to be changing or else it will
			// never move to its intended target speed. The current velocity will always be changing
			velocity.X = Mathf.Lerp(velocity.X, direction.X * speed, acceleration);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * speed, deceleration);
		}
		else
		{
			// MoveToward is like Lerp in that it provides movement smoothing. In this case
			// when the player stops moving it will smoothing approach 0. We provide 
			// a decelaration weight so in the case we want the player to decelerate at a different
			// speed when compared to acceleration 
			velocity.X = Mathf.MoveToward(Velocity.X, 0, deceleration);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, deceleration);
		}

		Velocity = velocity;
	}
	
	public void UpdateVelocity()
	{
		//Globals.debug.AddDebugProperty("Velocity", Velocity, 0, new Color(0, 1, 0));
		// Before we updated velocity here by doing Velocity = velocity for the sake of keeping
		// a global private velocity. That then caused the jump velocity to be cancelled since this
		// velocity did not know of any jumps (Y = 0) and overwrote the jump velocity
		MoveAndSlide();
		
	}

	private void UpdateStatLabels()
	{
		// Simply alter the text of the health and armor counts
		healthLabel.Text = new string($"{Globals.PlayerHealth}/{healthCapacity}");
		armorLabel.Text = new string($"{Globals.PlayerArmor}/{armorCapacity}");
		fragmentParticleLabel.Text = new string($"MINOR FRAGMENTS: {Globals.PlayerFragmentParticleCount}");
	}

	public void FragmentParticlePickup()
	{
		// When the player picks up a fragment particle we want to increase count by 1
		Globals.PlayerFragmentParticleCount += 1;
		UpdateStatLabels();
		// signal shop that fragment particle was collected
		EmitSignal(SignalName.MyCustomSignal, "FragmentParticleCollected");
		// also emit a typed change signal in case other systems want it
		EmitSignal(SignalName.FragmentParticleSpendResult, true, Globals.PlayerFragmentParticleCount);
	}

	public void TakeDamage(float damage)
	{
		// Dont want to keep getting hit after player dies
		if (Globals.PlayerHealth <= 0)
			return;
		if(Globals.PlayerArmor <= 0)
		{
			// Make sure armor is reset back to 0 and not go negative
			Globals.PlayerHealth -= damage;
			healthHit.Play();
	  		// for showing death screen
			if (Globals.PlayerHealth <= 0)
			{
				Globals.PlayerHealth = 0;
				EmitSignal(SignalName.MyCustomSignal, "PlayerDied");
			}
		}
		else
		{
			// Prioritize armor damage if its not <= 0
			// Its necessary that we check its value after being hit so that it doenst go negative
			Globals.PlayerArmor -= damage;
			armorHit.Play();
			if(Globals.PlayerArmor <= 0)
				Globals.PlayerArmor = 0;
		}
		
		// Dont forget to update the stat labels accordingly
		UpdateStatLabels();
	}

	public bool IsHealthFull()
	{
		// Helper function used to check the health ammount
		return Globals.PlayerHealth == healthCapacity;
	}

	public bool IsArmorFull()
	{
		// Helper function used to check the armor ammount
		return Globals.PlayerArmor == armorCapacity;
	}

	public bool ResourcePickup(string resource, int plane = 0)
	{
		// Assume that player resources are full so that they dont get picked up by the player
		bool isFull = true;
		switch(resource)
		{
			// Go the the appropriate match and call their respective functions
			// setting isFull to false signifies that the player is able to collect the resource
			// which will be returned back to the caller: ResourcePickup.cs
			case "ammo":
				if(!WEAPON.IsAmmoFull(plane))
				{
					// Need to let the weapon controller know that the player picked up ammo
					// We send the matching plane so that the right gun is filled
					WEAPON.AddAmmo(plane);
					isFull = false;
				}
				break;
			case "health":
				if(!IsHealthFull())
				{
					// If added health will go over the capacity then limit the health to the capacity
					Globals.PlayerHealth = Globals.PlayerHealth + 50.0f > healthCapacity ? healthCapacity : Globals.PlayerHealth + 50.0f;
					UpdateStatLabels();
					isFull = false;
				}
				break;
			case "armor":
				if(!IsArmorFull())
				{
					// If added armor will go over the capacity then limit the armor to the capacity
					Globals.PlayerArmor = Globals.PlayerArmor + 25.0f > armorCapacity ? armorCapacity : Globals.PlayerArmor + 25.0f;
					UpdateStatLabels();
					isFull = false;
				}
				break;
			default:
				GD.PushError("Passed incorrect resource");
				break;
		}

		return isFull;
	}

	public void OnHealthPressed()
	{
		// Upgrade current health capacity
		healthCapacity += 25.0f;
		// Make sure the health bar knows about this change too
		healthBar.MaxValue = healthCapacity;
		Globals.PlayerHealth = healthCapacity;
		GD.Print(healthCapacity);
		UpdateStatLabels();
	}

	public void OnArmorPressed()
	{	// Upgrade current armor capacity
		armorCapacity += 25.0f;
		// Make sure the armor bar knows about this change too
		armorBar.MaxValue = armorCapacity;
		Globals.PlayerArmor = armorCapacity;
		GD.Print(armorCapacity);
		UpdateStatLabels();
	}

	public void OnAttackPressed()
	{
		// Upgrade base damage
		// Need to call the Weapon Controller to upgrade the base damage of all guns
		WEAPON.UpgradeDamage();
	}

	public void OnSpeedPressed()
	{
		// Upgrade current speed
		// Simply increment the speed by a constant
		Globals.PlayerSpeed += 5.0f;
	}

	public void OnShopSpendRequested(int cost)
	{
		bool success = false;
		if (Globals.PlayerFragmentParticleCount >= cost)
		{
			Globals.PlayerFragmentParticleCount -= cost;
			success = true;
		}
		UpdateStatLabels();
		EmitSignal(SignalName.FragmentParticleSpendResult, success, Globals.PlayerFragmentParticleCount);
	}
	
}
