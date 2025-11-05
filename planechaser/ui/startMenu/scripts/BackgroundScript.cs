using Godot;
using System;

public partial class BackgroundScript : Node3D
{

    private bool zoomedIn = false;

    private async void OnStartPressed()
    {
        // wait a second
        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
        zoomedIn = !zoomedIn;
    }

    public bool IsZoomedIn()
    {
        return zoomedIn;
    }
}
