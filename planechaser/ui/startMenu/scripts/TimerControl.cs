using Godot;
using System;

public partial class TimerControl : Timer
{
    public int plane = 0;
    public override void _Ready()
    {
        base._Ready();
        Start();
    }
    private void OnStartPressed()
    {
        // if timer is running, pause it
        if (IsStopped() == false)
        {
            Stop();
        }
        // else start it 
        else
        {
            Start();
        }
    }
}
