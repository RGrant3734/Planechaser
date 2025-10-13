using Godot;
using System;

public partial class GameMaster : Node3D
{
    [Signal]
    public delegate void PlaneshiftEventHandler(int nextdimension);

    public int currentDimension = 1;
    public int numDimension = 3;
    public override void _Ready()
    {
        EmitSignal(SignalName.Planeshift, currentDimension);
    }


    public void Shift(int newDimension)
    {
        currentDimension = newDimension;
        GD.Print("Swapping Dimensions");
        EmitSignal(SignalName.Planeshift, newDimension);
    }
}
