using Godot;
using System;

public partial class lightBody : baseEnemy
{
	protected override void OnSpawn()
	{
		SwapType(gameMaster.currentDimension);
	}
}
