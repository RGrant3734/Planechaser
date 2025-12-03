using Godot;
using System;

public partial class mageController : rangedEnemy
{
	private MeshInstance3D staff;

	[Export] public PackedScene fragmentParticle;

	[ExportGroup("Materials")]
	[Export]
	public StandardMaterial3D blueWeaponMaterial;
	[Export]
	public StandardMaterial3D yellowWeaponMaterial;
	[Export]
	public StandardMaterial3D redWeaponMaterial;
	protected ShapeCast3D floorDetection;
	//protected GPUParticles3D deathParticle;
	
	protected override void OnSpawn()
	{
		staff = GetNode<MeshInstance3D>("Armature/Skeleton3D/Staff/Staff");
		floorDetection = GetNode<ShapeCast3D>("FloorDetection");
		//GD.Print(GetNode<GPUParticles3D>("DeathEffect"));
		//deathParticle = GetNode<GPUParticles3D>("DeathEffect");
		SwapType(gameMaster.currentDimension);
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
				animationTree.Set("parameters/conditions/Attack", InMeleeRange());
				animationTree.Set("parameters/conditions/Ranged", InSight());
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
				//LookAt(player.GlobalPosition, Vector3.Up);
				//RotateY(Mathf.Pi);
				LookAt(GlobalPosition + (Velocity * -1), Vector3.Up);
				sight.TargetPosition = sight.ToLocal(player.GlobalPosition);
				MoveAndSlide();
				break;
			case "Attack":
				animationTree.Set("parameters/conditions/Walk", !InMeleeRange());
				break;
			case "Hit":
				break;
			case "Death":
				if (dead)
				{
					// Spawn fragment particle
					var fragment = fragmentParticle.Instantiate<FragementParticlePickup>();
					var fragment2 = fragmentParticle.Instantiate<FragementParticlePickup>();
					var fragment3 = fragmentParticle.Instantiate<FragementParticlePickup>();
					GetTree().CurrentScene.AddChild(fragment);
					GetTree().CurrentScene.AddChild(fragment2);
					GetTree().CurrentScene.AddChild(fragment3);
					fragment.InitializeSpawnPosition(GlobalPosition + new Vector3(-0.2f, 1, 0));
					fragment2.InitializeSpawnPosition(GlobalPosition + new Vector3(0, 1, 0));
					fragment3.InitializeSpawnPosition(GlobalPosition + new Vector3(0.2f, 1, 0));
					QueueFree();
				}
				break;
			case "Ranged":
				animationTree.Set("parameters/conditions/Walk", !InSight());
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
	protected override void SwapType(int type)
	{
		switch (type)
		{
			case 0:
				mesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
				staff.Mesh.SurfaceSetMaterial(0, blueWeaponMaterial);
				break;
			case 1:
				mesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
				staff.Mesh.SurfaceSetMaterial(0, yellowWeaponMaterial);
				break;
			case 2:
				mesh.Mesh.SurfaceSetMaterial(0, redMaterial);
				staff.Mesh.SurfaceSetMaterial(0, redWeaponMaterial);
				break;
			default:
				GD.Print("TypeSetError(Enemy)");
				break;
		}
	}
	//Enemy takes damage in their special ways and dies
	public override void Hit(int weaponPlane, float baseDamage)
	{   
		// If the current weapon's native plane matches the current level plane then do bonus damage
		if(weaponPlane == gameMaster.currentDimension)
			currentHealth -= baseDamage * 1.5f;
		else
			currentHealth -= baseDamage;

		if(currentHealth <= 0)
		{
			deathParticle.emitting = true;
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
		animationTree.Set("parameters/conditions/Ranged", false);
	}
}
