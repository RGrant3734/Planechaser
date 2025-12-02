using Godot;
using System;

public partial class LightBossHealthBarLogic : ProgressBar
{
    [Export] public lightController bossScript;

    public override void _Ready()
    {
        // set max value to boss max health
        MaxValue = (float)bossScript.Get("totalHealth");
        Value = (float)bossScript.Get("totalHealth");
    }
    public override void _Process(double delta)
    {
        // update health bar value to boss current health
        Value = (float)bossScript.Get("totalHealth");
    }
}
