using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class lightningAttack : Area3D
{
	[Export(PropertyHint.Range, "0, 2, ")]
	public int strikePlane = 0;
	[ExportGroup("Materials")]
	[Export]
	public StandardMaterial3D blueMaterial;
	[Export]
	public StandardMaterial3D yellowMaterial;
	[Export]
	public StandardMaterial3D redMaterial;
	private MeshInstance3D mesh;
	private GameMaster gameMaster;
	private shadowController caller;
	private bool lightningStriking = false;
	
	public AudioStreamPlayer3D lightningSound;
	public AudioStreamPlayer3D lightningCrashSound;
	public override void _Ready()
	{
		mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		gameMaster = GetNode<GameMaster>("/root/GameMaster");
		gameMaster.Planeshift += ShowStrikes;
		caller = (shadowController)GetTree().GetFirstNodeInGroup("Shadow");
		caller.LightningCall += Attack;
		lightningSound = GetNode<AudioStreamPlayer3D>("Lightning");
		lightningCrashSound = GetNode<AudioStreamPlayer3D>("LightningCrash");
		mesh.Visible = false;
		switch (strikePlane)
		{
			case 0:
				mesh.SetSurfaceOverrideMaterial(0, blueMaterial);
				break;
			case 1:
				mesh.SetSurfaceOverrideMaterial(0, yellowMaterial);
				break;
			case 2:
				mesh.SetSurfaceOverrideMaterial(0, redMaterial);
				break;
			default:
				GD.Print("TypeSetError(Enemy)");
				break;
		}
	}

	public async void Attack()
	{
		lightningSound.Play();
		lightningStriking = true;
		ShowStrikes(gameMaster.currentDimension);
		await ToSignal(GetTree().CreateTimer(5), "timeout");
		lightningCrashSound.Play();
		foreach (Node3D body in GetOverlappingBodies())
		{
			if (body.IsInGroup("Player"))
			{
				if(strikePlane == gameMaster.currentDimension)
				{
					// Not hit
				} else
				{
					FPSController player = (FPSController)body;
					// Take shit load of damage 
					player.TakeDamage(100.0f);
				}
			}
		}
		lightningStriking = false;
		mesh.Visible = false;
	}

	public void ShowStrikes(int plane)
	{
		if(!lightningStriking)
			return;

		switch (plane)
		{
			case 0:
				if (strikePlane == 0)
				{
					mesh.Visible = false;
				}
				else
				{
					mesh.Visible = true;
				}
				break;
			case 1:
				if (strikePlane == 1)
				{
					mesh.Visible = false;
				}
				else
				{
					mesh.Visible = true;  
				}
				break;
			case 2:
				if (strikePlane == 2)
				{
					mesh.Visible = false;
				}
				else
				{
					mesh.Visible = true;
				}
				break;
			default:
				GD.Print("TypeSetError(Environment)");
				break;
		}
	}

	public void SwapType()
	{
		switch (strikePlane)
		{
			case 0:
				mesh.SetSurfaceOverrideMaterial(0, blueMaterial);
				break;
			case 1:
				mesh.SetSurfaceOverrideMaterial(0, yellowMaterial);
				break;
			case 2:
				mesh.SetSurfaceOverrideMaterial(0, redMaterial);
				break;
			default:
				GD.Print("TypeSetError(Enemy)");
				break;
		}
	}
}
