using Godot;
using System;

public partial class knightController : meleeEnemy
{
	private MeshInstance3D sword;
	private MeshInstance3D hilt;
	private MeshInstance3D helmet;
	private MeshInstance3D chest;
	private MeshInstance3D legL;
	private MeshInstance3D legR;
	private bool isArmorDestroyed = false;

	[Export] public PackedScene fragmentParticle;

	[Export(PropertyHint.Range, "0, 2,")]
	public int armorPlane = 0;

	[ExportGroup("Materials")]
	[Export]
	public StandardMaterial3D blueArmorMaterial;
	[Export]
	public StandardMaterial3D yellowArmorMaterial;
	[Export]
	public StandardMaterial3D redArmorMaterial;
	[Export]
	public StandardMaterial3D blueWeaponMaterial;
	[Export]
	public StandardMaterial3D yellowWeaponMaterial;
	[Export]
	public StandardMaterial3D redWeaponMaterial;
	protected ShapeCast3D floorDetection;
	protected GpuParticles3D deathParticle;
	protected GpuParticles3D blueParticle;
	protected GpuParticles3D yellowParticle;
	protected GpuParticles3D redParticle;
	protected StandardMaterial3D deadMaterial;
	protected StandardMaterial3D deadWeaponMaterial;
	public AudioStreamPlayer3D hitSound;
	public AudioStreamPlayer3D deathSound;
	public AudioStreamPlayer3D attackSound;
	public AudioStreamPlayer3D armorHitSound;
	protected override void OnSpawn()
	{
		mesh = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Body");
		sword = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Sword/Sword");
		hilt = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Hilt/Hilt");
		helmet = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Helmet");
		chest = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/Chest");
		legL = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/LegLeft");
		legR = GetNode<MeshInstance3D>("Armature_001/Skeleton3D/LegRight");
		floorDetection = GetNode<ShapeCast3D>("FloorDetection");
		deathParticle = GetNode<GpuParticles3D>("Armature_001/DeathEffect");
		blueParticle = GetNode<GpuParticles3D>("Armature_001/BlueArmorHit");
		yellowParticle = GetNode<GpuParticles3D>("Armature_001/YellowArmorHit");
		redParticle = GetNode<GpuParticles3D>("Armature_001/RedArmorHit");
		SwapType(gameMaster.currentDimension);
		hitSound = GetNode<AudioStreamPlayer3D>("Hit");
		deathSound = GetNode<AudioStreamPlayer3D>("Death");
		attackSound = GetNode<AudioStreamPlayer3D>("Attack");
		armorHitSound = GetNode<AudioStreamPlayer3D>("ArmorHit");
	}

	public override void _Process(double delta)
	{
		if(hitPlayed)
		{
			ForceState("Walk");
			hitPlayed = false;
		}
		switch(stateMachine.GetCurrentNode())
		{
			case "Idle":
				animationTree.Set("parameters/conditions/Walk", true);
				break;
			case "Walk":
				//Falls down if nothing under it
				if (!floorDetection.IsColliding())
				{
					stateMachine.Travel("Fall");
					break;
				}
				//Movement towards player
				Vector3 desiredDirection = Vector3.Zero;
				nav.TargetPosition = player.GlobalPosition;
				desiredDirection = (nav.GetNextPathPosition() - GlobalPosition).Normalized();
				Velocity = Velocity.Lerp(desiredDirection * moveSpeed, .4f);
				moveVal = moveVal.Lerp(new Godot.Vector3(1, 0, 0), .4f);
				LookAt(GlobalPosition + (Velocity * -1), Vector3.Up);
				animationTree.Set("parameters/conditions/Attack", InMeleeRange());
				MoveAndSlide();
				break;
			case "Attack":
				animationTree.Set("parameters/conditions/Walk", !InMeleeRange());
				break;
			case "Hit":
				break;
			case "Death":
				//particles and fade
				deathParticle.Emitting = true;
				Color c = deadMaterial.AlbedoColor;
				c.A = Mathf.Lerp(c.A, 0f, 0.025f);
				deadMaterial.AlbedoColor = c;
				deadWeaponMaterial.AlbedoColor = c;
				
				if (dead)
				{
					// Spawn fragment particle
					var fragment = fragmentParticle.Instantiate<FragementParticlePickup>();
					var fragment2 = fragmentParticle.Instantiate<FragementParticlePickup>();
					GetTree().CurrentScene.AddChild(fragment);
					GetTree().CurrentScene.AddChild(fragment2);
					fragment.InitializeSpawnPosition(GlobalPosition + new Vector3(-0.1f, 1, 0));
					fragment2.InitializeSpawnPosition(GlobalPosition + new Vector3(0.1f, 1, 0));
					QueueFree();
				}
				break;
			case "Fall":
				//Disable nav movement
				nav.SetVelocity(Vector3.Zero);
				nav.AvoidanceEnabled = false;

				//Let physics take over
				Velocity = GetGravity();
				MoveAndSlide();

				//When floor detected again, resume walk
				if (floorDetection.IsColliding())
				{
					nav.AvoidanceEnabled = true;
					stateMachine.Travel("Walk");
				}
				break;
			default:
				break;
		}
	}

	private void ArmorSwitch(bool thisisArmorDestroyed)
	{
		helmet.Visible = thisisArmorDestroyed;
		chest.Visible = thisisArmorDestroyed;
		legL.Visible = thisisArmorDestroyed;
		legR.Visible = thisisArmorDestroyed;
	}

	protected override void SwapType(int type)
	{
		//dies with previous color
		if(currentHealth <= 0)
			return;
		// Weapon color swaps with plane
		switch (type)
		{
			case 0:
				mesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
				sword.Mesh.SurfaceSetMaterial(0, blueWeaponMaterial);
				hilt.Mesh.SurfaceSetMaterial(0, blueWeaponMaterial);
				break;
			case 1:
				mesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
				sword.Mesh.SurfaceSetMaterial(0, yellowWeaponMaterial);
				hilt.Mesh.SurfaceSetMaterial(0, yellowWeaponMaterial);
				break;
			case 2:
				mesh.Mesh.SurfaceSetMaterial(0, redMaterial);
				sword.Mesh.SurfaceSetMaterial(0, redWeaponMaterial);
				hilt.Mesh.SurfaceSetMaterial(0, redWeaponMaterial);
				break;
			default:
				GD.Print("TypeSetError(Enemy)");
				break;
		}
		// Sets armor for native plane
		switch (armorPlane)
		{
			case 0:
				helmet.SetSurfaceOverrideMaterial(0, blueArmorMaterial);
				chest.SetSurfaceOverrideMaterial(0, blueArmorMaterial);
				legL.SetSurfaceOverrideMaterial(0, blueArmorMaterial);
				legR.SetSurfaceOverrideMaterial(0, blueArmorMaterial);
				break;
			case 1:
				helmet.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
				chest.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
				legL.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
				legR.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
				break;
			case 2:
				helmet.SetSurfaceOverrideMaterial(0, redArmorMaterial);
				chest.SetSurfaceOverrideMaterial(0, redArmorMaterial);
				legL.SetSurfaceOverrideMaterial(0, redArmorMaterial);
				legR.SetSurfaceOverrideMaterial(0, redArmorMaterial);
				break;
			default:
				GD.Print("TypeSetError(armorPlane)");
				break;
		}
		// Hides armor if on native plane
		if (type == armorPlane || isArmorDestroyed)
		{
			helmet.Visible = false;
			chest.Visible = false;
			legL.Visible = false;
			legR.Visible = false;
		}
		else
		{
			helmet.Visible = true;
			chest.Visible = true;
			legL.Visible = true;
			legR.Visible = true;
		}
	}

   
	//Enemy takes damage in their special ways and dies
	public override void Hit(int weaponPlane, float baseDamage)
	{
		//If no armor, then takes regular damage
		if(armor > 0 && gameMaster.currentDimension != armorPlane)
		{
			// Damages armor
			if(armorPlane == 0)
				blueParticle.Emitting = true;
			if(armorPlane == 1)
				yellowParticle.Emitting = true;
			if(armorPlane == 2)
				redParticle.Emitting = true;

			if(weaponPlane == armorPlane)
				armor -= baseDamage * 2f;
			else
				armor -= baseDamage;
		}
		else
		{
			deathParticle.Emitting = true;
			if(weaponPlane == gameMaster.currentDimension)
				currentHealth -= baseDamage * 2.0f;
			else
				currentHealth -= baseDamage;
		}

		// Once the armor is <= 0 for the first time then call ArmorSwitch once
		if(armor <= 0 && !isArmorDestroyed)
		{
			isArmorDestroyed = true;
			ArmorSwitch(false);
		}
		
		if(currentHealth <= 0)
		{
			// duplicates materials so it can fade
			mesh.Mesh = (Mesh)mesh.Mesh.Duplicate(true);
			sword.Mesh = (Mesh)sword.Mesh.Duplicate(true);
			deadMaterial =  (StandardMaterial3D)mesh.Mesh.SurfaceGetMaterial(0).Duplicate(true);
			deadWeaponMaterial = (StandardMaterial3D)sword.Mesh.SurfaceGetMaterial(0).Duplicate(true);
			mesh.Mesh.SurfaceSetMaterial(0, deadMaterial);
			sword.Mesh.SurfaceSetMaterial(0, deadWeaponMaterial);
			hilt.Mesh.SurfaceSetMaterial(0, deadWeaponMaterial);
			deadMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
			deadWeaponMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
			ForceState("Death");
			animationTree.Set("parameters/conditions/Death", true);
		} else {
			stateMachine.Start("Hit");
			animationTree.Set("parameters/conditions/Hit", true);
		}
	}
	protected void ForceState(string state)
	{
		stateMachine.Travel(state);

		animationTree.Set("parameters/conditions/Walk", false);
		animationTree.Set("parameters/conditions/Attack", false);
		animationTree.Set("parameters/conditions/Hit", false);
		animationTree.Set("parameters/conditions/Fall", false);
	}
	public void PlaySound(string sound)
	{
		if(deathSound.Playing || attackSound.Playing)
			return;
		if(sound == "Hit")
			if(armor > 0)
				armorHitSound.Play();
			else
				hitSound.Play();
		if(sound == "Attack")
			attackSound.Play();
		if(sound == "Death")
			deathSound.Play();
	}
}
