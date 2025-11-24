using Godot;
using System;

public partial class WeaponController : Node3D
{
	// Our weapon resource.
	[Export] private WeaponResource currentWeapon;
	private AnimationPlayer weaponAnimPlayer;
	private AudioStreamPlayer3D gunSound;

	// Public property to allow CurrentWeapon to call LoadWeapon in the editor
	// This way we can see the changes of swaping Weapons resources in the editor

	// Noise Texture is what give the idle sway the randomness
	[Export] public NoiseTexture2D SwayNoise;
	// How fast should the random sway be
	[Export] public float SwaySpeed = 1.2f;
	// Weapon MeshInstance
	private Node3D WeaponMesh;
	// Current weapon count in the case we press the same button twice
	private int currWeaponIndex = 0;
	// Need to capture the mouse movement for our weapon sway
	private Vector2 MouseMovement;
	private float RandomSwayX;
	private float RandomSwayY;
	private float RandomSwayAmount;
	// Time helps in generating random sway for sin and cos
	private float time = 0.0f;
	// Factor multiplied into noise calculation (doesnt do much)
	private float IdleSwayAdjustment;
	// How strong the Idle Sway Rotation should be
	private float IdleSwayRotationStength;
	// Vector for holding the bob values
	private Vector2 bobAmount = Vector2.Zero;

	/* Shooting variables */
	//-------------------------
	// Need reference to the world camera so that we can give it a recoil effect
	[Export] public CameraRecoil cameraRecoilRef;
	[Export] public AmmoCounter ammoCounterRef;
	// Node reference to the RecoilPosition Node within this scene
	public float fireRate;
	public int ammoCount;
	private WeaponRecoil weaponRecoilRef;
	private MuzzleFlash muzzleFlashRef;
	private PackedScene impactEffect;
	private PackedScene weaponDecal;
	private int nativePlane;
	private float baseDamage;

	private WeaponResource[] arsenal =
    {
        GD.Load<WeaponResource>("res://player/assets/weapons/rifle/RifleResource.tres"),
		GD.Load<WeaponResource>("res://player/assets/weapons/launcher/LauncherResource.tres"),
		GD.Load<WeaponResource>("res://player/assets/weapons/sniper/SniperResource.tres")
    };
	public bool isLocked = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		// Grab references to Weapon mesh
		//WeaponMesh = GetNode<MeshInstance3D>("RecoilPosition/WeaponMesh");
		// Grab Recoil Position node which dictates how far the gun visually kicks back
		weaponRecoilRef = GetNode<WeaponRecoil>("WeaponRecoil");
		// Load the weapon at run time
		LoadWeapon(false);
	}

	private async void SwapWeapon()
    {
		// Lock out weapon system from shooting
		isLocked = true;
        // Play weapon holster animation
		weaponAnimPlayer.Play("Holster");
		await ToSignal(weaponAnimPlayer, AnimationPlayer.SignalName.AnimationFinished);
		// Delete current weapon
		WeaponMesh.QueueFree();

		// Load new weapon
		//currentWeapon = GD.Load<Weapons>(arsenal[currWeaponIndex]);
		// Save the current weapons ammo count before loading new weapon
		currentWeapon.AmmoCount = ammoCount;
		currentWeapon = arsenal[currWeaponIndex];
		WeaponMesh.Visible = false;
		LoadWeapon(true);
		weaponAnimPlayer.PlayBackwards("Holster");
		WeaponMesh.Visible = true;
		await ToSignal(weaponAnimPlayer, AnimationPlayer.SignalName.AnimationFinished);
		// Unlock weapon shooting
		isLocked = false;
		
    }

	public override void _Input(InputEvent @event)
	{
		// Switching weapons
		base._Input(@event);
		if (@event.IsActionPressed("weapon_1") && currWeaponIndex != 0)
		{
			// Load the resource, load the weapon by passing its attributes over to the meshes
			// and then setting the current help weapon to the appropriate value
			currWeaponIndex = 0;
			SwapWeapon();
		}
		if (@event.IsActionPressed("weapon_2") && currWeaponIndex != 1)
		{
			currWeaponIndex = 1;
			SwapWeapon();
		}
		if(@event.IsActionPressed("weapon_3") && currWeaponIndex != 2)
        {
			currWeaponIndex = 2;
			SwapWeapon();
        }


		if (@event is InputEventMouseMotion)
		{
			// Need to cast event over to InputEventMouseMotion, copy that into a local variable and
			// pass the Relative (mouse deltas between frames) over to MouseMovement 
			InputEventMouseMotion MouseEvent = (InputEventMouseMotion)@event;
			MouseMovement = MouseEvent.Relative;
		}
	}

	private void LoadWeapon(bool fromSwap)
	{
		// Make sure everything is valid before touching it
		if (currentWeapon == null)
			return;
		
		
		WeaponMesh = (Node3D)currentWeapon.Mesh.Instantiate();
		
		if(fromSwap)
        {
            var anim = weaponAnimPlayer.GetAnimation("Holster");
			// Position track last key
			WeaponMesh.Position = (Vector3)anim.TrackGetKeyValue(0,1);
		
			//Position = currentWeapon.Position;
			//RotationDegrees = currentWeapon.Rotation;
			// Rotation track last key
			WeaponMesh.RotationDegrees = (Vector3)anim.TrackGetKeyValue(1,1);
			
        }

		Position = currentWeapon.Position;
		RotationDegrees = currentWeapon.Rotation;
        // Scale never changes
		Scale = currentWeapon.Scale;
		
		// Set Weapon Visuals
		weaponRecoilRef.AddChild(WeaponMesh);

		// Grab special nodes from the weapon scene
		weaponAnimPlayer = WeaponMesh.GetNode<AnimationPlayer>("AnimationPlayer");
		muzzleFlashRef = WeaponMesh.GetNode<MuzzleFlash>("./WeaponMeshes/MuzzleFlash");
		gunSound = WeaponMesh.GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");


		// Set Random Idle Sway
		IdleSwayAdjustment = currentWeapon.IdleSwayAdjustment;
		IdleSwayRotationStength = currentWeapon.IdleSwayRotationStength;
		RandomSwayAmount = currentWeapon.RandomSwayAmmount;

		// Set fire rate
		fireRate = currentWeapon.FireRate;
		// Set weapons current/remaining ammo count 
		ammoCount = currentWeapon.AmmoCount;
		ammoCounterRef.EmitSignal("SwapWeaponAmmoCountSignal", ammoCount);

		// Set camera specific recoil
		cameraRecoilRef.recoilAmount = currentWeapon.CameraRecoilAmount;
		cameraRecoilRef.snapAmount = currentWeapon.CameraSnapAmount;
		cameraRecoilRef.speed = currentWeapon.CameraRecoverySpeed;

		// Set weapon specific recoil
		weaponRecoilRef.recoilAmount = currentWeapon.WeaponRecoilAmount;
		weaponRecoilRef.snapAmount = currentWeapon.WeaponSnapAmount;
		weaponRecoilRef.speed = currentWeapon.WeaponRecoverySpeed;

		// Weapon Impact effect
		impactEffect = currentWeapon.ImpactEffect;
		weaponDecal = currentWeapon.WeaponDecal;

		nativePlane = currentWeapon.NativePlane;
		baseDamage = currentWeapon.BaseDamage;
		
	}

	public void WeaponBob(double delta, float bobSpeed, float bobH, float bobV)
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
		// If MouseMovement is 0 then the only thing left would be the currentWeapon.Position.X/Y - RandomSwayX/Y
		position.X = (float)Mathf.Lerp(position.X, currentWeapon.Position.X - (MouseMovement.X *
			currentWeapon.SwayAmountPosition + RandomSwayX + (isMoving ? bobAmount.X : 0.0f)) * delta, currentWeapon.SwaySpeedPosition);
		position.Y = (float)Mathf.Lerp(position.Y, currentWeapon.Position.Y - (MouseMovement.Y *
			currentWeapon.SwayAmountPosition + RandomSwayY + (isMoving ? bobAmount.Y : 0.0f)) * delta, currentWeapon.SwaySpeedPosition);
		// Lerp weapon rotation based on mouse movement
		// Similar concept to position. If MouseMovement.X/Y is 0 then the only thing left would be the
		// currentWeapon.Rotation.Y/X +/- RandomSwayY/X * IdleSwayRotationStrength. This is what causes the idle sway
		rotationDeg.Y = (float)Mathf.Lerp(rotationDeg.Y, currentWeapon.Rotation.Y - (MouseMovement.X *
			currentWeapon.SwayAmountRotation + (RandomSwayY * IdleSwayRotationStength)) * delta, currentWeapon.SwaySpeedRotation);
		rotationDeg.X = (float)Mathf.Lerp(rotationDeg.X, currentWeapon.Rotation.X - (MouseMovement.Y *
			currentWeapon.SwayAmountRotation + (RandomSwayX * IdleSwayRotationStength)) * delta, currentWeapon.SwaySpeedRotation);
        
    }

	public void SwayWeapon(double delta, bool isIdle)
	{
		// Return to base position if the mouse is not moving
		MouseMovement = MouseMovement.Lerp(Vector2.Zero, (float)(delta * 6.0));

		// Make sure to clamp the sway ammounts 
		MouseMovement = MouseMovement.Clamp(currentWeapon.SwayMin, currentWeapon.SwayMax);
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

	private void UpdateAmmo()
    {
        ammoCount--;
		ammoCounterRef.EmitSignal("UpdateAmmoCountSignal", ammoCount);
    }

	private void FireWeapon()
    {
        // Add/Simulate weapon recoil by altering the camera rotation
		cameraRecoilRef.EmitSignal("AddCameraRecoilSignal");
		// Simulate weapon recoil kickback
		weaponRecoilRef.EmitSignal("WeaponFiredSignal");
		// Simulate muzzle flash
		muzzleFlashRef.EmitSignal("MuzzleFlashSignal");
		// Update Ammo count
		UpdateAmmo();
		// Play gun sound effect
		gunSound.Play();
    }

	private Node FindEnemyRoot(Node start)
	{
		// Traverse upward in the local tree to find if the root node
		// is part of the Enemy group
	    Node current = start;

	    while (current != null)
	    {
	        if (current.IsInGroup("Enemy"))
	            return current;

	        current = current.GetParent();
	    }

		// If null if returned then we know that its a solid object
	    return null;
	}

	private async void HitScan(Vector3 position, Vector3 normal, Node collider)
	{
		FireWeapon();
		// Spawn mesh at the given position. Position will be from ray cast result
		Decal instance = weaponDecal.Instantiate<Decal>();
		ImpactExplosion laserE = impactEffect.Instantiate<ImpactExplosion>();
		// Root will be at the FPS_Controller scene. Make sure to add the decal to the tree first before giving it a position and rotation
		GetTree().Root.AddChild(instance);
		GetTree().Root.AddChild(laserE);
		//instance.AlignParticle(instance.GlobalTransform.Origin + normal);
		instance.Position = position;
		laserE.GlobalPosition = position;
		
		var mat = laserE.ProcessMaterial as ParticleProcessMaterial;
		mat.Direction = normal;

		// Check if the collider is from an enemy type
		// If so then call the Hit that corresponds to the correct enemy type
		Node enemyRoot = FindEnemyRoot(collider);
		if(enemyRoot != null)
			//enemyRoot.Call("Hit", nativePlane, baseDamage);
			enemyRoot.Call("Hit", nativePlane, baseDamage);

		// First argument creates a point on the normal of the surface which defines
		// where the object should look. The second argument ensures the decal doesnt roll the wrong way
		instance.LookAt(instance.GlobalTransform.Origin + normal, Vector3.Up);
		if (normal != Vector3.Up || normal != Vector3.Down)
			instance.RotateObjectLocal(new Vector3(1.0f, 0.0f, 0.0f), Mathf.DegToRad(90));
		// Despawn timer
		await ToSignal(GetTree().CreateTimer(3.0f), "timeout");
		// Here we want to create a fade effect. So we create a tween and change linearly interpolate the decals alpha
		var fade = GetTree().CreateTween();
		fade.TweenProperty(instance, "modulate:a", 0, 1.5);
		// Need to provide the tween some time for it to interpolate the alpha to 0.0f
		await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
		// Once the timer goes out then we can delete the decal. In this case the decal will
		// only be deleted when its alpha is 0.0f which is basically invisible
		instance.QueueFree();
	}

	private void Projectile(Vector3 position)
    {
		FireWeapon();
        // Instantiate new bullet and pass over the target position
		// Bullet will take care of the rest
		Bullet b = currentWeapon.Bullet.Instantiate<Bullet>();
		//muzzleFlashRef.AddChild(b);
		b.GlobalTransform= muzzleFlashRef.GlobalTransform;
		GetTree().Root.AddChild(b);
		b.LookAt(position, Vector3.Up);
    }


	public void Shoot()
	{
		// Shoot a ray cast from the center of the screen
		// straight outwards until it either collides with a body or reaches limit

		// Grab a reference to the players world camera. (Camera Controller is the world camera)
		Camera3D camera = Globals.player.WORLDCAMERA;
		// Grab the worlds 3D physics state/sandbox. This state is where all of the physics occurs and its handled by the physics server
		var spaceState = camera.GetWorld3D().DirectSpaceState;
		// Need to find the center of the screen to create origin point. GetViewport here is the weapon camera viewport but since its always
		// following the player then we can assume that its the same as getting the world camera viewport
		Vector2 screenCenter = (Vector2)GetViewport().Get("size") / 2;
		// Start point of the ray in this case in the center of the screen. We are picking a point on the screen. 
		// Its important that we project the ray from the world camera
		Vector3 origin = camera.ProjectRayOrigin(screenCenter);
		// The end of ray is 1000m out from the cameras normal
		Vector3 end = origin + camera.ProjectRayNormal(screenCenter) * 1000;
		// Create the ray which will return back a dictionary with metadata on any
		// physics collisions. Make sure to enable collision with bodies or areas
		var query = PhysicsRayQueryParameters3D.Create(origin, end);
		query.CollideWithBodies = true;
		query.CollideWithAreas = true;
		// Detect layers 1, 2, and 3
		query.CollisionMask = (1 << 0) | (1 << 1) | (1 << 2);
		// Find out if the ray intersected with a body. It will return nothing if not
		// We are essentially creating a dictionary holding a number of keys that pertain to the collision information
		var result = spaceState.IntersectRay(query);
		// If the ray collided with something then we are safe to "fire" the weapon 
		// We send the position of contact and the normal vector of the surface
		GD.Print(result);
		if (result.Count != 0 && !currentWeapon.ProjectileBased)
			HitScan((Vector3)result["position"], (Vector3)result["normal"], (Node)result["collider"]);
		else if(result.Count != 0 && currentWeapon.ProjectileBased)
			Projectile((Vector3)result["position"]);
	}
	
	// Since we dont have a physicsProcess here then the script cant show a preview of the weapon sway

}
