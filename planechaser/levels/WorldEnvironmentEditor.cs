using Godot;
using System;

public partial class WorldEnvironmentEditor : WorldEnvironment
{
    public GameMaster gameMaster;
    public override void _Ready()
    {
        gameMaster = GetNode<GameMaster>("/root/GameMaster");
        gameMaster.Planeshift += Planeshift;
    }

    public void Planeshift(int plane)
    {
        GD.Print("Swapped");
        if(plane == 0)
        {
            Environment.VolumetricFogAlbedo = new Color("ROYAL_BLUE");
        }
        if(plane == 1)
        {
            Environment.VolumetricFogAlbedo = new Color("PALE_GOLDENROD");
        }
        if(plane == 2)
        {
            Environment.VolumetricFogAlbedo = new Color("CRIMSON");
        }
    }
}
