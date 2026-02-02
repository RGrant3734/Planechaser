using Godot;
using System;

public partial class EnvironmentSwapper : StaticBody3D
{
	// Starting plane (can be offset from rotation)
	[Export(PropertyHint.Range, "0, 2,")]
	public int StartTypeNumber = 0;

	[ExportGroup("Materials")]
	[Export]
	public StandardMaterial3D Plane1Material;
	[Export]
	public StandardMaterial3D Plane2Material;
	[Export]
	public StandardMaterial3D Plane3Material;

	[ExportGroup("PlaneBased")]
	// Do we want it to be hidden on a plane?
	[Export]
	public bool HideOnPlane;
	// What plane will it be hidden on?
	[Export(PropertyHint.Range, "0, 2,")]
	public int HiddenPlane;
	private GameMaster _GameMaster;
	private MeshInstance3D meshInstance;
	private CollisionShape3D collShape;
	[Export]
	private bool inMenu;
	public TimerControl swapTimer;
	public int currentPlane;
	public override void _Ready()
	{
		base._Ready();
		if(!inMenu)
		{
		_GameMaster = GetNode<GameMaster>("/root/GameMaster");
		_GameMaster.Planeshift += Planeshift;			
		}
		// Menu Screen
		if(inMenu)
		{
			swapTimer = GetTree().Root.GetNode<TimerControl>("Menu/Background/Timer");
			swapTimer.Timeout += Planeshift;
		}
		meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
		collShape = GetNode<CollisionShape3D>("CollisionShape3D");

		SwapType(StartTypeNumber);
	}

	// Main planeshift function call that cna have additions for when shifting happens
	public void Planeshift(int currDimension)
	{
		SwapType((currDimension + StartTypeNumber) % 3);
	}
	public async void Planeshift()
	{
		await ToSignal(GetTree().CreateTimer(0.9), "timeout");
		//Menu screen shifting
		if(currentPlane == 2)
            currentPlane = 0;
        else
            currentPlane++;
		SwapType((currentPlane + StartTypeNumber) % 3);
	}
	public void SwapType(int type)
	{
		// Enables and Disables based on plane and if it is supposed to be hidden on a plane
		switch (type)
		{
			case 0:
				if (HideOnPlane && HiddenPlane == 0)
				{
					meshInstance.Visible = false;
					collShape.Disabled = true;
				}
				else
				{
					meshInstance.Visible = true;
					collShape.Disabled = false;
					meshInstance.SetSurfaceOverrideMaterial(0, Plane1Material);   
				}
				break;
			case 1:
				if (HideOnPlane && HiddenPlane == 1)
				{
					meshInstance.Visible = false;
					collShape.Disabled = true;
				}
				else
				{
					meshInstance.Visible = true;
					collShape.Disabled = false;
					meshInstance.SetSurfaceOverrideMaterial(0, Plane2Material);   
				}
				break;
			case 2:
				if (HideOnPlane && HiddenPlane == 2)
				{
					meshInstance.Visible = false;
					collShape.Disabled = true;
				}
				else
				{
					meshInstance.Visible = true;
					collShape.Disabled = false;
					meshInstance.SetSurfaceOverrideMaterial(0, Plane3Material);   
				}
				break;
			default:
				GD.Print("TypeSetError(Environment)");
				break;
		}
	}
}
