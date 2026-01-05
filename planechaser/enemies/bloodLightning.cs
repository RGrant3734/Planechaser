using Godot;
using System;

public partial class bloodLightning : Area3D
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
	private GameMaster gameMaster;
	private bool lightningStriking = false;
	private bool struck = false;
	[Export]
	public PackedScene blueStrike;
	[Export]
	public PackedScene yellowStrike;
	[Export]
	public PackedScene redStrike;
	[Export]
	public MeshInstance3D mesh;
	[Export]
	public MeshInstance3D mesh2;
	[Export]
	public MeshInstance3D mesh3;
	[Export]
	public GpuParticles3D bluelightningStrike;
	[Export]
	public GpuParticles3D yellowlightningStrike;
	[Export]
	public GpuParticles3D redlightningStrike;
	[Export]
	public GpuParticles3D crackle1;
	[Export]
	public GpuParticles3D crackle2;
	public AudioStreamPlayer3D lightningCrashSound;
	[Export]
	public ArrayMesh blueCrackle;
	[Export]
	public ArrayMesh yellowCrackle;
	[Export]
	public ArrayMesh redCrackle;
	public override void _Ready()
	{
		gameMaster = GetNode<GameMaster>("/root/GameMaster");
		gameMaster.Planeshift += ShowStrikes;
		lightningCrashSound = GetNode<AudioStreamPlayer3D>("LightningCrash");
		switch (strikePlane)
		{
			case 0:
				mesh.SetSurfaceOverrideMaterial(0, blueMaterial);
				mesh2.SetSurfaceOverrideMaterial(0, blueMaterial);
				mesh3.SetSurfaceOverrideMaterial(0, blueMaterial);
				break;
			case 1:
				mesh.SetSurfaceOverrideMaterial(0, yellowMaterial);
				mesh2.SetSurfaceOverrideMaterial(0, yellowMaterial);
				mesh3.SetSurfaceOverrideMaterial(0, yellowMaterial);
				break;
			case 2:
				mesh.SetSurfaceOverrideMaterial(0, redMaterial);
				mesh2.SetSurfaceOverrideMaterial(0, redMaterial);
				mesh3.SetSurfaceOverrideMaterial(0, redMaterial);
				break;
			default:
				GD.Print("TypeSetError(Enemy)");
				break;
		}
	}
    public override void _Process(double delta)
    {
        if(struck)
		{
			this.QueueFree();
		}
		if(strikePlane == gameMaster.currentDimension)
		{
			mesh.Visible = false;
			mesh2.Visible = false;
			mesh3.Visible = false;
			crackle1.Emitting = false;
			crackle2.Emitting = false;
		}
    }


	public async void Attack()
	{
		lightningStriking = true;
		ShowStrikes(gameMaster.currentDimension);
		await ToSignal(GetTree().CreateTimer(5), "timeout");
		mesh.Visible = false;
		mesh2.Visible = false;
		mesh3.Visible = false;
		crackle1.Emitting = false;
		crackle2.Emitting = false;
		if(strikePlane == gameMaster.currentDimension)
		{
			struck = true;
			return;
		}
		lightningCrashSound.Play();
		if(strikePlane == 0)
		{
			bluelightningStrike.Emitting = true;
			var blueStrikeMark = blueStrike.Instantiate<MeshInstance3D>();
			GetTree().CurrentScene.AddChild(blueStrikeMark);
			blueStrikeMark.GlobalPosition = lightningCrashSound.GlobalPosition;
		}
		if(strikePlane == 1)
		{
			yellowlightningStrike.Emitting = true;
			var yellowStrikeMark = yellowStrike.Instantiate<MeshInstance3D>();
			GetTree().CurrentScene.AddChild(yellowStrikeMark);
			yellowStrikeMark.GlobalPosition = lightningCrashSound.GlobalPosition;
		}
		if(strikePlane == 2)
		{
			redlightningStrike.Emitting = true;
			var redStrikeMark = redStrike.Instantiate<MeshInstance3D>();
			GetTree().CurrentScene.AddChild(redStrikeMark);
			redStrikeMark.GlobalPosition = lightningCrashSound.GlobalPosition;
		}
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
					// Hit
					player.TakeDamage(75.0f);
				}
			}
		}
		lightningStriking = false;
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		struck = true;
	}

	public void ShowStrikes(int plane)
	{
		if(!lightningStriking || struck)
		{
			return;
		}

		switch (plane)
		{
			case 0:
				if (strikePlane == 0)
				{
					mesh.Visible = false;
					mesh2.Visible = false;
					mesh3.Visible = false;
					crackle1.Emitting = false;
					crackle2.Emitting = false;
				}
				else
				{
					mesh.Visible = true;
					mesh2.Visible = true;
					mesh3.Visible = true;
					crackle1.Emitting = true;
					crackle2.Emitting = true;
				}
				break;
			case 1:
				if (strikePlane == 1)
				{
					mesh.Visible = false;
					mesh2.Visible = false;
					mesh3.Visible = false;
					crackle1.Emitting = false;
					crackle2.Emitting = false;
				}
				else
				{
					mesh.Visible = true;
					mesh2.Visible = true;
					mesh3.Visible = true;
					crackle1.Emitting = true;
					crackle2.Emitting = true;  
				}
				break;
			case 2:
				if (strikePlane == 2)
				{
					mesh.Visible = false;
					mesh2.Visible = false;
					mesh3.Visible = false;
					crackle1.Emitting = false;
					crackle2.Emitting = false;
				}
				else
				{
					mesh.Visible = true;
					mesh2.Visible = true;
					mesh3.Visible = true;
					crackle1.Emitting = true;
					crackle2.Emitting = true;
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
				mesh2.SetSurfaceOverrideMaterial(0, blueMaterial);
				mesh3.SetSurfaceOverrideMaterial(0, blueMaterial);
				crackle1.DrawPass1 = blueCrackle;
				crackle2.DrawPass1 = blueCrackle;
				break;
			case 1:
				mesh.SetSurfaceOverrideMaterial(0, yellowMaterial);
				mesh2.SetSurfaceOverrideMaterial(0, yellowMaterial);
				mesh3.SetSurfaceOverrideMaterial(0, yellowMaterial);
				crackle1.DrawPass1 = yellowCrackle;
				crackle2.DrawPass1 = yellowCrackle;
				break;
			case 2:
				mesh.SetSurfaceOverrideMaterial(0, redMaterial);
				mesh2.SetSurfaceOverrideMaterial(0, redMaterial);
				mesh3.SetSurfaceOverrideMaterial(0, redMaterial);
				crackle1.DrawPass1 = redCrackle;
				crackle2.DrawPass1 = redCrackle;
				break;
			default:
				GD.Print("TypeSetError(Enemy)");
				break;
		}
	}
}
