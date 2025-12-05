using Godot;
using System;

public partial class lightEyeController : CharacterBody3D
{
	[Export(PropertyHint.Range, "0, 2, ")]
	public int startTypeNumber = 0;
	[Export(PropertyHint.Range, "0, 2, ")]
	public int nativePlane = 0;
	[Export]
	public float totalHealth = 100;
	[ExportGroup("Materials")]
	[Export]
	public StandardMaterial3D blueMaterial;
	[Export]
	public StandardMaterial3D yellowMaterial;
	[Export]
	public StandardMaterial3D redMaterial;
	[Export]
	public MeshInstance3D socketMesh;
	[Export]
	public MeshInstance3D lowerLidMesh;
	[Export]
	public MeshInstance3D upperLidMesh;
	[Export]
	public StandardMaterial3D blueEyeMaterial;
	[Export]
	public StandardMaterial3D yellowEyeMaterial;
	[Export]
	public StandardMaterial3D redEyeMaterial;
	[Export]
	public MeshInstance3D eyeballMesh;
	[Export]
	public StandardMaterial3D blueDeadMaterial;
	[Export]
	public StandardMaterial3D yellowDeadMaterial;
	[Export]
	public StandardMaterial3D redDeadMaterial;
	protected GameMaster gameMaster;
	protected CharacterBody3D player;
	public bool isDead = false;
	protected float currentHealth;
	[Signal]
	public delegate void EyeDeathEventHandler();
	public AudioStreamPlayer3D hitSound;
	public AudioStreamPlayer3D deathSound;
	public override void _Ready()
	{
		// intializing important assets
		gameMaster = GetNode<GameMaster>("/root/GameMaster");
		gameMaster.Planeshift += Planeshift;
		gameMaster.Spawn += ChangeEyes;
		player = GetTree().GetFirstNodeInGroup("Player") as CharacterBody3D;
		if (player == null)
		{
			throw new Exception("Expects a player in group Player");
		}
		hitSound = GetNode<AudioStreamPlayer3D>("Hit");
		deathSound = GetNode<AudioStreamPlayer3D>("Death");
		currentHealth = totalHealth;
	}

	protected void Planeshift(int currDimension)
	{
		SwapType((currDimension + startTypeNumber) % 3);
	}
	// Always looks at the player while alive
	public override void _Process(double delta)
	{
		if(!isDead){
			eyeballMesh.LookAt(player.GlobalPosition, Vector3.Up);
			eyeballMesh.RotateY(Mathf.DegToRad(90));
			eyeballMesh.RotateX(Mathf.DegToRad(15));
		}
	}

	protected void SwapType(int type)
	{
		switch (type)
		{
			case 0:
				socketMesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
				upperLidMesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
				lowerLidMesh.Mesh.SurfaceSetMaterial(0, blueMaterial);
				break;
			case 1:
				socketMesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
				upperLidMesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
				lowerLidMesh.Mesh.SurfaceSetMaterial(0, yellowMaterial);
				break;
			case 2:
				socketMesh.Mesh.SurfaceSetMaterial(0, redMaterial);
				upperLidMesh.Mesh.SurfaceSetMaterial(0, redMaterial);
				lowerLidMesh.Mesh.SurfaceSetMaterial(0, redMaterial);
				break;
			default:
				GD.Print("TypeSetError(Enemy)");
				break;
		}
		switch (nativePlane)
		{
			case 0:
				eyeballMesh.SetSurfaceOverrideMaterial(0, blueEyeMaterial);
				break;
			case 1:
				eyeballMesh.SetSurfaceOverrideMaterial(0, yellowEyeMaterial);
				break;
			case 2:
				eyeballMesh.SetSurfaceOverrideMaterial(0, redEyeMaterial);
				break;
			default:
				GD.Print("TypeSetError(Enemy)");
				break;
		}
		ChangeEyes();
	}
	// Open and closing eyes based on plane, starts closed until spawning happens
	public void ChangeEyes()
	{
		if(gameMaster.spawning && !isDead)
		{
			switch (nativePlane)
			{
				case 0:
					if (nativePlane == gameMaster.currentDimension)
					{
						upperLidMesh.Visible = false;
						lowerLidMesh.Visible = false;
					}
					else
					{
						upperLidMesh.Visible = true;
						lowerLidMesh.Visible = true;
					}
					break;
				case 1:
					if (nativePlane == gameMaster.currentDimension)
					{
						upperLidMesh.Visible = false;
						lowerLidMesh.Visible = false;
					}
					else
					{
						upperLidMesh.Visible = true;
						lowerLidMesh.Visible = true;
					}
					break;
				case 2:
					if (nativePlane == gameMaster.currentDimension)
					{
						upperLidMesh.Visible = false;
						lowerLidMesh.Visible = false;
					}
					else
					{
						upperLidMesh.Visible = true;
						lowerLidMesh.Visible = true;
					}
					break;
				default:
					GD.Print("TypeSetError(Enemy)");
					break;
			}
		}
	}
	// Eye can die which leads to a hand deactivation
	public void Hit(int weaponPlane, float baseDamage)
	{
		// If the current weapon's native plane matches the current level plane then do bonus damage
		if(!gameMaster.spawning || isDead || nativePlane != gameMaster.currentDimension)
			return;
		
		if(currentHealth <= 0)
		{
			if(!isDead)
				PlaySound("Death");
			isDead = true;
			upperLidMesh.Visible = true;
			lowerLidMesh.Visible = true;
			// Visual difference on socket when dead
			blueMaterial = blueDeadMaterial;
			yellowMaterial = yellowDeadMaterial;
			redMaterial = redDeadMaterial;
			EmitSignal(SignalName.EyeDeath);
		}
		else if(weaponPlane == gameMaster.currentDimension)
        {
            PlaySound("Hit");
			currentHealth -= baseDamage * 1.5f;
        }
		else
        {
			PlaySound("Hit");
            currentHealth -= baseDamage;
        }
	}
	public void PlaySound(string sound)
	{
		if(deathSound.Playing)
			return;
		if(sound == "Hit")
			hitSound.Play();
		if(sound == "Death")
			deathSound.Play();
	}
}
