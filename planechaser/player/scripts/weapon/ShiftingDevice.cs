using Godot;
using System;

public partial class ShiftingDevice : MeshInstance3D
{
	[Export] public float frequency = 1.0f;
	[Export] public float amplitude = 0.02f;
	[Export] public float angle = 3.0f;
	private Vector3 position;
	private float origin = 0.0f;
	private float time = 0.0f;

	private GameMaster gameMaster;
	// Material stuff
	[Export] private StandardMaterial3D[] planes;
	[Export] private MeshInstance3D[] Meshes;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		// Able to find the gameMaster like a global variable
		gameMaster = GetNode<GameMaster>("/root/GameMaster");
		
        position = Position;
		origin = Position.Y;

		// Connect Planeshift signal over to Planeshift function
		gameMaster.Planeshift += Planeshift;

		/*
		Mesh1.Mesh.SurfaceSetMaterial(0, planes[0]);
		var mat = (StandardMaterial3D)Mesh1.GetActiveMaterial(0);
		mat.EmissionEnergyMultiplier = 7.0f;
		*/
		foreach(MeshInstance3D mesh in Meshes)
        {
            mesh.Mesh.SurfaceSetMaterial(0, planes[0]);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		// time aids in acquiring a new value to feed into sin. We use frequency and amplitude values
		// to create a custom bobbing animation for the resource pickup
		// Only need to wrap time if very long play sessions (days) are to be expected
		time += (float)delta;
		position.Y = origin + Mathf.Sin(time * frequency) * amplitude;
		// Update the position and rotate it to the according angle
		Position = position;
		//Rotate(Transform.Basis.Y.Normalized(), Mathf.DegToRad(angle));
        
    }

	public async void Planeshift(int currentDimension)
    {
		// Need reference to each of the meshes's materials
		StandardMaterial3D mat;
		// Loop through each mesh and assign it a unique material with emmision turned off
		foreach(MeshInstance3D mesh in Meshes)
        {
			// DUPLICATE the material so each mesh has its own one
        	var uniqueMat = (StandardMaterial3D)planes[currentDimension].Duplicate();
        
        	mesh.Mesh.SurfaceSetMaterial(0, uniqueMat);
        	uniqueMat.EmissionEnabled = true;
        	uniqueMat.EmissionEnergyMultiplier = 0.0f;
        }

		// Loop through each mesh and alter the assigned material with an emission energy of 7
		// We wait a little bit so that it looks like a count down before the player can shift
		foreach(MeshInstance3D mesh in Meshes)
        {
			await ToSignal(GetTree().CreateTimer(gameMaster.planeshiftCooldown/3), "timeout");
			mat = (StandardMaterial3D)mesh.GetActiveMaterial(0);
			mat.EmissionEnergyMultiplier = 7.0f;
        }
		
		

    }
}
