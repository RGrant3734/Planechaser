using Godot;
using System;

public partial class gruntController : meleeEnemy
{
	protected ShapeCast3D floorDetection;

	[Export]
	public PackedScene fragmentParticle;
	protected GpuParticles3D deathParticle;
	protected StandardMaterial3D deadMaterial;
	public AudioStreamPlayer3D hitSound;
	public AudioStreamPlayer3D deathSound;
	public AudioStreamPlayer3D attackSound;
	protected override void OnSpawn()
	{
		SwapType(gameMaster.currentDimension);
		floorDetection = GetNode<ShapeCast3D>("FloorDetection");
		deathParticle = GetNode<GpuParticles3D>("Armature_002/DeathEffect");
		hitSound = GetNode<AudioStreamPlayer3D>("Hit");
		deathSound = GetNode<AudioStreamPlayer3D>("Death");
		attackSound = GetNode<AudioStreamPlayer3D>("Attack");
		gameMaster.gruntNum++;
	}

	public override void _Process(double delta)
	{
		if(hitPlayed)
		{
			ForceState("Walk");
			hitPlayed = false;
		}
		switch (stateMachine.GetCurrentNode())
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
				GetChild<CollisionShape3D>(0).Disabled = true;
				deathParticle.Emitting = true;
				Color c = deadMaterial.AlbedoColor;
				c.A = Mathf.Lerp(c.A, 0f, 0.025f);
				deadMaterial.AlbedoColor = c;
				if (dead)
				{
					// Spawn fragment particle
					var fragment = fragmentParticle.Instantiate<FragementParticlePickup>();
					GetTree().CurrentScene.AddChild(fragment);
					fragment.InitializeSpawnPosition(GlobalPosition + new Vector3(0, 1, 0));
					gameMaster.gruntNum--;
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
	//Enemy takes damage in their special ways and dies
	public override void Hit(int weaponPlane, float baseDamage)
	{
		// If the current weapon's native plane matches the current level plane then do bonus damage
		if(weaponPlane == gameMaster.currentDimension)
			currentHealth -= baseDamage * 2.0f;
		else
			currentHealth -= baseDamage;
		
		if(currentHealth <= 0)
		{
			//duplicates materials so it can fade
			mesh.Mesh = (Mesh)mesh.Mesh.Duplicate(true);
			deadMaterial = (StandardMaterial3D)mesh.Mesh.SurfaceGetMaterial(0).Duplicate(true);
			mesh.Mesh.SurfaceSetMaterial(0, deadMaterial);
			deadMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
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
			hitSound.Play();
		if(sound == "Attack")
			attackSound.Play();
		if(sound == "Death")
			deathSound.Play();
	}
}
