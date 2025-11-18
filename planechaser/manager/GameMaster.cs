using Godot;
using System;

public partial class GameMaster : Node3D
{
    [Signal]
    public delegate void PlaneshiftEventHandler(int nextdimension);
    [Signal]
    public delegate void SpawnWaveEventHandler();
    public int currentDimension = 0;
    public int numDimension = 3;
    // Starts with shift to make sure all is on the same plane
    public override void _Ready()
    {
        EmitSignal(SignalName.Planeshift, currentDimension);
    }

    // Shifts to the next dimension
    public void Shift(int newDimension)
    {
        currentDimension = newDimension;
        EmitSignal(SignalName.Planeshift, newDimension);
        EmitSignal(SignalName.SpawnWave);
    }
}
