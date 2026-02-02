using Godot;
using System;

public partial class WorldEnvironmentEditor : WorldEnvironment
{
    public GameMaster gameMaster;
    [Export]
    public bool isMenu;
    public override void _Ready()
    {
        if(isMenu)
            gameMaster = GetNode<GameMaster>("/root/Menu/GameMaster");
        else
            gameMaster = GetNode<GameMaster>("/root/GameMaster");
            
        gameMaster.Planeshift += Planeshift;
    }

    public void Planeshift(int plane)
    {
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
