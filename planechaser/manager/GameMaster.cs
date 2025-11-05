using Godot;
using System;

public partial class GameMaster : Node3D
{
    [Signal]
    public delegate void PlaneshiftEventHandler(int nextdimension);

    public int currentDimension = 0;
    public int numDimension = 3;
    public override void _Ready()
    {
        EmitSignal(SignalName.Planeshift, currentDimension);
    }


    public void Shift(int newDimension)
    {
        currentDimension = newDimension;
        EmitSignal(SignalName.Planeshift, newDimension);
    }
}
