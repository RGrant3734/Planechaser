	using Godot;
	using Godot.Collections;
	using System;
	using System.Runtime.CompilerServices;

	public partial class bloodController : rangedEnemy
	{
		private Vector3 startingPosition;
		private Vector3 jumpPosition;
		private bool attacking = false;
		private MeshInstance3D sword;
		private MeshInstance3D hilt;
		private MeshInstance3D helmet;
		private MeshInstance3D chest;
		private MeshInstance3D legL;
		private MeshInstance3D legR;
		private bool isArmorDestroyed = false;
		[Export]
		public NavigationLink3D navLink;
		[Export(PropertyHint.Range, "0, 2,")]
		public int armorPlane = 0;
		[Export]
		public float armorRegenAmount = 5;
		[Export]
		public float armorRegenSpeed = 1;
		private float armorMax;

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
		[Signal]
		public delegate void LightningCallEventHandler();
		public bool lightningReady = true;
		public int lightningCooldown = 5;
		private bool atHome = true;
		private Vector3 lastRotation;
		private int stuckCount = 0;
		protected GpuParticles3D deathParticle;
		protected GpuParticles3D blueParticle;
		protected GpuParticles3D yellowParticle;
		protected GpuParticles3D redParticle;
		protected StandardMaterial3D deadMaterial;
		protected StandardMaterial3D deadWeaponMaterial;
		private bool counted = false;
		private Timer timer;
		public AudioStreamPlayer3D hitSound;
		public AudioStreamPlayer3D deathSound;
		public AudioStreamPlayer3D attackSound;
		public AudioStreamPlayer3D armorHitSound;
		public AudioStreamPlayer3D castSound;
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
			hitSound = GetNode<AudioStreamPlayer3D>("Hit");
			deathSound = GetNode<AudioStreamPlayer3D>("Death");
			attackSound = GetNode<AudioStreamPlayer3D>("Attack");
			armorHitSound = GetNode<AudioStreamPlayer3D>("ArmorHit");
			castSound = GetNode<AudioStreamPlayer3D>("Cast");
			timer = GetNode<Timer>("Timer");
			timer.Timeout += RegenArmor;
			timer.WaitTime = armorRegenSpeed;
			armorMax = armor;
			startingPosition = GlobalPosition;
			jumpPosition = navLink.GetGlobalEndPosition();
			SwapType(gameMaster.currentDimension);
		}

		public override void _Process(double delta)
		{
			if(hitPlayed)
			{
				ForceState("Idle");
				hitPlayed = false;
			}
			if(atHome && armor < armorMax)
			{
				if(armor < 0)
					armor = 0;
				if(timer.TimeLeft == 0)
					timer.Start();
			}
			// Resets them if fall from platform
			if(RotationDegrees == new Vector3(90, 90, 0))
			{
				if(lastRotation == RotationDegrees)
					stuckCount++;
				
				if(stuckCount >= 20)
				{
					GlobalPosition = jumpPosition;
					atHome = false;
					stuckCount = 0;
				}
			}
			lastRotation = RotationDegrees;
			// only attacks melee if they have armor
			if(armorPlane == gameMaster.currentDimension || isArmorDestroyed)
				attacking = false;
			else
				attacking = true;

			switch(stateMachine.GetCurrentNode())
			{
				case "Idle":
					sight.TargetPosition = sight.ToLocal(player.GlobalPosition);
					if(!gameMaster.spawning)
						break;
					animationTree.Set("parameters/conditions/Idle", false);
					animationTree.Set("parameters/conditions/Walk", attacking);
					animationTree.Set("parameters/conditions/Ranged",!attacking && lightningReady);
					break;
				case "Walk":
					// leaves home
					if (attacking && atHome)
					{
						GlobalPosition = jumpPosition;
						Velocity = Vector3.Zero;
						moveVal = Vector3.Zero;
						atHome = false;
						break;
					} 
					// attacks player
					if (attacking && !atHome)
					{
						nav.TargetPosition = player.GlobalPosition;
						Vector3 target = nav.GetNextPathPosition();
						Vector3 direction = (target - GlobalPosition).Normalized();
						Velocity = Velocity.Lerp(direction * moveSpeed, 0.3f);
						if (Velocity.Length() > 0.1f)
							LookAt(GlobalPosition + Velocity.Normalized() *-1, Vector3.Up);
						moveVal = moveVal.Lerp(new Vector3(1, 0, 0), 0.3f);
						animationTree.Set("parameters/conditions/Attack", InMeleeRange());
						MoveAndSlide();
						break;
					}
					// goes home
					if(!attacking && !atHome)
					{
						if (GlobalPosition.DistanceTo(navLink.GetGlobalEndPosition()) < 2.0f)
						{
							jumpPosition = GlobalPosition;
							GlobalPosition = navLink.GetGlobalStartPosition();
							RotateY(180);
							Velocity = Vector3.Zero;
							moveVal = Vector3.Zero;
							atHome = true;
							animationTree.Set("parameters/conditions/Walk", false);
							animationTree.Set("parameters/conditions/Idle", true);
							break;
						}
						nav.TargetPosition = navLink.GetGlobalEndPosition();
						Vector3 homeTarget = nav.GetNextPathPosition();
						Vector3 homeDirection = (homeTarget - GlobalPosition).Normalized();
						Velocity = Velocity.Lerp(homeDirection * moveSpeed * 2, 0.3f);
						if (Velocity.Length() > 0.1f)
							LookAt(GlobalPosition + Velocity.Normalized() *-1, Vector3.Up);
						moveVal = moveVal.Lerp(new Vector3(1, 0, 0), 0.3f);
						animationTree.Set("parameters/conditions/Attack", InMeleeRange());
						MoveAndSlide();
						break;
					}
					// should be at idle
					if(!attacking && atHome)
					{
						animationTree.Set("parameters/conditions/Walk", false);
						animationTree.Set("parameters/conditions/Idle", true);
						break;
					}
					break;
				case "Attack":
					animationTree.Set("parameters/conditions/Walk", !InMeleeRange());
					break;
				case "Shout":
					break;
				case "Ranged":
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
						if(!counted)
						{
							counted = true;
							gameMaster.bloodDeathCount++;
							if(gameMaster.bloodDeathCount == 3)
							{
								gameMaster.levelCompleted = true;
								gameMaster.spawning = false;
							}
							SetCollisionLayerValue(4, false);	
						}
					}
					break;
				case "Land":
					break;
				case "Jump":
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
			if(currentHealth <= 0)
				return;
			// Weapon color swaps with plane
			switch (type)
			{
				case 0:
					mesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
					break;
				case 1:
					mesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
					break;
				case 2:
					mesh.Mesh.SurfaceSetMaterial(0, redMaterial);
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
					sword.SetSurfaceOverrideMaterial(0, blueWeaponMaterial);
					hilt.SetSurfaceOverrideMaterial(0, blueWeaponMaterial);
					break;
				case 1:
					helmet.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
					chest.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
					legL.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
					legR.SetSurfaceOverrideMaterial(0, yellowArmorMaterial);
					sword.SetSurfaceOverrideMaterial(0, yellowWeaponMaterial);
					hilt.SetSurfaceOverrideMaterial(0, yellowWeaponMaterial);
					break;
				case 2:
					helmet.SetSurfaceOverrideMaterial(0, redArmorMaterial);
					chest.SetSurfaceOverrideMaterial(0, redArmorMaterial);
					legL.SetSurfaceOverrideMaterial(0, redArmorMaterial);
					legR.SetSurfaceOverrideMaterial(0, redArmorMaterial);
					sword.SetSurfaceOverrideMaterial(0, redWeaponMaterial);
					hilt.SetSurfaceOverrideMaterial(0, redWeaponMaterial);
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
				helmet.Visible = false;
				chest.Visible = true;
				legL.Visible = true;
				legR.Visible = true;
			}
		}

	
		//Enemy takes damage in their special ways and dies
		public override void Hit(int weaponPlane, float baseDamage)
		{
			if(!gameMaster.spawning)
				return;
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
					currentHealth -= baseDamage * 1.5f;
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
				deadWeaponMaterial = (StandardMaterial3D)sword.GetSurfaceOverrideMaterial(0).Duplicate(true);
				mesh.Mesh.SurfaceSetMaterial(0, deadMaterial);
				sword.SetSurfaceOverrideMaterial(0, deadWeaponMaterial);
				hilt.SetSurfaceOverrideMaterial(0, deadWeaponMaterial);
				deadMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
				deadWeaponMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
				ForceState("Death");
				animationTree.Set("parameters/conditions/Death", true);
			} else {
				//stateMachine.Start("Hit");
				//animationTree.Set("parameters/conditions/Hit", true);
			}
		}

		protected async override void RangedAttack()
		{
			// Spawns a lightning attack on the player for the planes they are not native to
			lightningReady = false;
			if(armorPlane != 0)
			{
				var blueprojectileFired = projectile.Instantiate<bloodLightning>();
				blueprojectileFired.strikePlane = 0;
				GetTree().CurrentScene.AddChild(blueprojectileFired);
				blueprojectileFired.GlobalPosition = player.GlobalPosition;
				blueprojectileFired.SwapType();
				blueprojectileFired.Attack();
				await ToSignal(GetTree().CreateTimer(1), "timeout");
			}
			if (armorPlane != 1)
			{
				var yellowprojectileFired = projectile.Instantiate<bloodLightning>();
				yellowprojectileFired.strikePlane = 1;
				GetTree().CurrentScene.AddChild(yellowprojectileFired);
				yellowprojectileFired.GlobalPosition = player.GlobalPosition;
				yellowprojectileFired.SwapType();
				yellowprojectileFired.Attack();
				await ToSignal(GetTree().CreateTimer(1), "timeout");
			}
			if (armorPlane != 2){
				var redprojectileFired = projectile.Instantiate<bloodLightning>();
				redprojectileFired.strikePlane = 2;
				GetTree().CurrentScene.AddChild(redprojectileFired);
				redprojectileFired.GlobalPosition = player.GlobalPosition;
				redprojectileFired.SwapType();
				redprojectileFired.Attack();
				await ToSignal(GetTree().CreateTimer(1), "timeout");
			}
			LightningCooldown();
		}
		public void LightningAttack()
		{
			EmitSignal(SignalName.LightningCall);
		}
		public async void LightningCooldown()
		{
			await ToSignal(GetTree().CreateTimer(lightningCooldown), "timeout");
			lightningReady = true;
		}
		protected void ForceState(string state)
		{
			stateMachine.Travel(state);

			animationTree.Set("parameters/conditions/Idle", false);
			animationTree.Set("parameters/conditions/Ranged", false);
			animationTree.Set("parameters/conditions/Walk", false);
			animationTree.Set("parameters/conditions/Attack", false);
			animationTree.Set("parameters/conditions/Hit", false);
			animationTree.Set("parameters/conditions/Jump", false);
			animationTree.Set("parameters/conditions/Land", false);
		}
		protected override void AnimFinished(StringName anim)
		{
			if (anim == "Blood/Death")
			{
				dead = true;
			}
		}
		public void RegenArmor()
		{
			armor += armorRegenAmount;
			if(armor > armorMax)
			{
				armor = armorMax;
			}
			if(armor > 100)
			{
				isArmorDestroyed = false;
				SwapType(gameMaster.currentDimension);
			}
		}
		public void PlaySound(string sound)
		{
			if(deathSound.Playing || attackSound.Playing || castSound.Playing)
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
			if(sound == "Cast")
				castSound.Play();
		}
	}
