using Godot;
using System;

public partial class ShadowBossHealthBarLogic : ProgressBar
{
    [Export] public shadowController bossScript;

    public override void _Ready()
    {
        // set max value to boss max health
        MaxValue = (float)bossScript.Get("currentHealth");
        Value = (float)bossScript.Get("currentHealth");
    }
    public override void _Process(double delta)
    {
        // update health bar value to boss current health
        Value = (float)bossScript.Get("currentHealth");
    }
}
