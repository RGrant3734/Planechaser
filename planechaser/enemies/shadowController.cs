using Godot;
using System;

public partial class shadowController : rangedEnemy
{
    private MeshInstance3D staff;
    private MeshInstance3D robe;

    [ExportGroup("Materials")]
    [Export]
    public StandardMaterial3D blueWeaponMaterial;
    [Export]
    public StandardMaterial3D yellowWeaponMaterial;
    [Export]
    public StandardMaterial3D redWeaponMaterial;
    [Export]
    public StandardMaterial3D blueRobeMaterial;
    [Export]
    public StandardMaterial3D yellowRobeMaterial;
    [Export]
    public StandardMaterial3D redRobeMaterial;
    protected ShapeCast3D floorDetection;
    protected int rangedCount = 0;
    [Signal]
    public delegate void LightningCallEventHandler();
    protected GpuParticles3D deathParticle;
	protected StandardMaterial3D deadMaterial;
	protected StandardMaterial3D deadWeaponMaterial;
    protected StandardMaterial3D deadRobeMaterial;
    protected StandardMaterial3D deadRobeAccentMaterial;
    public AudioStreamPlayer3D hitSound;
	public AudioStreamPlayer3D deathSound;
	public AudioStreamPlayer3D attackSound;
    public AudioStreamPlayer3D castSound;
	public AudioStreamPlayer3D lightningSound;
    protected override void OnSpawn()
    {
        staff = GetNode<MeshInstance3D>("Armature/Skeleton3D/Staff/Staff");
        robe = GetNode<MeshInstance3D>("Armature/Skeleton3D/Robe");
        floorDetection = GetNode<ShapeCast3D>("FloorDetection");
        deathParticle = GetNode<GpuParticles3D>("Armature/DeathEffect");
        hitSound = GetNode<AudioStreamPlayer3D>("Hit");
		deathSound = GetNode<AudioStreamPlayer3D>("Death");
		attackSound = GetNode<AudioStreamPlayer3D>("Attack");
		castSound = GetNode<AudioStreamPlayer3D>("Cast");
        lightningSound = GetNode<AudioStreamPlayer3D>("Lightning");
        SwapType(gameMaster.currentDimension);
    }

    public override void _Process(double delta)
    {
        switch (stateMachine.GetCurrentNode())
        {
            case "Idle":
                if(!gameMaster.spawning)
                    break;

                animationTree.Set("parameters/conditions/Attack", InMeleeRange());
                animationTree.Set("parameters/conditions/Lightning", rangedCount >= 3);
                animationTree.Set("parameters/conditions/Ranged", InSight());
                LookAt(new Vector3(player.GlobalPosition.X * -1, player.GlobalPosition.Y, player.GlobalPosition.Z * -1), Vector3.Up);
                sight.TargetPosition = sight.ToLocal(player.GlobalPosition);
                break;
            case "Attack":
                animationTree.Set("parameters/conditions/Idle", !InMeleeRange());
                break;
            case "Lightning":
                rangedCount = 0;
                break;
            case "Death":
                //particles and fade
				deathParticle.Emitting = true;
				Color c = deadMaterial.AlbedoColor;
				c.A = Mathf.Lerp(c.A, 0f, 0.05f);
				deadMaterial.AlbedoColor = c;
				deadWeaponMaterial.AlbedoColor = c;
                deadRobeMaterial.AlbedoColor = c;
                deadRobeAccentMaterial.AlbedoColor = c;
                if(dead)
                {
                    gameMaster.levelCompleted = true;
                    gameMaster.spawning = false;
                    SetCollisionLayerValue(3, false);
                    SetCollisionLayerValue(4, false);
                }
                break;
            case "Ranged":
                rangedCount++;
                animationTree.Set("parameters/conditions/Idle", true);
                break;
            default:
                break;
        }
    }
    protected override void SwapType(int type)
    {
        //dies with previous color
		if(currentHealth <= 0)
			return;
        switch (type)
        {
            case 0:
                mesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
                staff.Mesh.SurfaceSetMaterial(0, blueWeaponMaterial);
                robe.Mesh.SurfaceSetMaterial(0, blueRobeMaterial);
                robe.Mesh.SurfaceSetMaterial(1, yellowRobeMaterial);
                break;
            case 1:
                mesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
                staff.Mesh.SurfaceSetMaterial(0, yellowWeaponMaterial);
                robe.Mesh.SurfaceSetMaterial(0, yellowRobeMaterial);
                robe.Mesh.SurfaceSetMaterial(1, redRobeMaterial);
                break;
            case 2:
                mesh.Mesh.SurfaceSetMaterial(0, redMaterial);
                staff.Mesh.SurfaceSetMaterial(0, redWeaponMaterial);
                robe.Mesh.SurfaceSetMaterial(0, redRobeMaterial);
                robe.Mesh.SurfaceSetMaterial(1, blueRobeMaterial);
                break;
            default:
                GD.Print("TypeSetError(Enemy)");
                break;
        }
    }
    //Enemy takes damage in their special ways and dies
    public override void Hit(int weaponPlane, float baseDamage)
    {   
        if(!gameMaster.spawning)
            return;
        // If the current weapon's native plane matches the current level plane then do bonus damage
		if(weaponPlane == gameMaster.currentDimension)
			currentHealth -= baseDamage * 1.5f;
		else
			currentHealth -= baseDamage;
		
		if(currentHealth <= 0)
		{
			//duplicates materials so it can fade
			mesh.Mesh = (Mesh)mesh.Mesh.Duplicate(true);
			staff.Mesh = (Mesh)staff.Mesh.Duplicate(true);
            robe.Mesh = (Mesh)robe.Mesh.Duplicate(true);
			deadMaterial =  (StandardMaterial3D)mesh.Mesh.SurfaceGetMaterial(0).Duplicate(true);
			deadWeaponMaterial = (StandardMaterial3D)staff.Mesh.SurfaceGetMaterial(0).Duplicate(true);
            deadRobeMaterial = (StandardMaterial3D)robe.Mesh.SurfaceGetMaterial(0).Duplicate(true);
            deadRobeAccentMaterial = (StandardMaterial3D)robe.Mesh.SurfaceGetMaterial(1).Duplicate(true);
			mesh.Mesh.SurfaceSetMaterial(0, deadMaterial);
			staff.Mesh.SurfaceSetMaterial(0, deadWeaponMaterial);
            robe.Mesh.SurfaceSetMaterial(0, deadRobeMaterial);
            robe.Mesh.SurfaceSetMaterial(1, deadRobeAccentMaterial);
			deadMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
			deadWeaponMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
            deadRobeMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
			deadRobeAccentMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
			ForceState("Death");
			animationTree.Set("parameters/conditions/Death", true);
		} else {
			//stateMachine.Start("Hit");
			//animationTree.Set("parameters/conditions/Hit", true);
		}
    }
    public void LightningAttack()
    {
        EmitSignal(SignalName.LightningCall);
    }
    protected void ForceState(string state)
	{
		stateMachine.Travel(state);

		animationTree.Set("parameters/conditions/Idle", false);
		animationTree.Set("parameters/conditions/Ranged", false);
		animationTree.Set("parameters/conditions/Attack", false);
		animationTree.Set("parameters/conditions/Hit", false);
		animationTree.Set("parameters/conditions/Jump", false);
		animationTree.Set("parameters/conditions/Lightning", false);
	}
    protected override void AnimFinished(StringName anim)
	{
		if (anim == "Global/Death")
		{
			dead = true;
		}
	}
    public void PlaySound(string sound)
    {
		if(deathSound.Playing || attackSound.Playing || castSound.Playing || lightningSound.Playing)
			return;
        if(sound == "Hit")
			hitSound.Play();
		if(sound == "Attack")
			attackSound.Play();
		if(sound == "Death")
			deathSound.Play();
		if(sound == "Cast")
			castSound.Play();
        if(sound == "lightning")
            lightningSound.Play();
    }
}
