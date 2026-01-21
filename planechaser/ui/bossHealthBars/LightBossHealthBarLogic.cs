using Godot;
using System;

public partial class LightBossHealthBarLogic : ProgressBar
{
    [Export] public lightController bossScript;
    [Signal]
    public delegate void UpdateEyesEventHandler();
    public int blueEyesCount = 0;
    public int yellowEyesCount = 0;
    public int redEyesCount = 0;
    public override void _Ready()
    {
        bossScript.LightEyeDeath += EyeDeath;
        blueEyesCount = bossScript.blueEyes;
        yellowEyesCount = bossScript.yellowEyes;
        redEyesCount = bossScript.redEyes;
        // set max value to boss max health
        MaxValue = (float)bossScript.Get("totalHealth");
        Value = (float)bossScript.Get("totalHealth");
        EmitSignal(SignalName.UpdateEyes);
    }
    public override void _Process(double delta)
    {
        // update health bar value to boss current health
        Value = (float)bossScript.Get("totalHealth");
    }
    public void EyeDeath(int color)
    {
        if(color == 0)
            blueEyesCount--;
        if(color == 1)
            yellowEyesCount--;
        if(color == 2)
            redEyesCount--;
        EmitSignal(SignalName.UpdateEyes);
    }
}
