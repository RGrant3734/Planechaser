using Godot;
using System;

public partial class BloodBossHealthBarLogic : Control
{
    [Export] public bloodController bossScript1;
    [Export] public bloodController bossScript2;
    [Export] public bloodController bossScript3;

    private ProgressBar hpBar1;
    private ProgressBar hpBar2;
    private ProgressBar hpBar3;

    public override void _Ready()
    {
        hpBar1 = GetNode<ProgressBar>("Bar1");
        hpBar2 = GetNode<ProgressBar>("Bar2");
        hpBar3 = GetNode<ProgressBar>("Bar3");
        // set max value to boss max health
        hpBar1.MaxValue = (float)bossScript1.Get("currentHealth");
        hpBar1.Value = (float)bossScript1.Get("currentHealth");
        hpBar2.MaxValue = (float)bossScript2.Get("currentHealth");
        hpBar2.Value = (float)bossScript2.Get("currentHealth");
        hpBar3.MaxValue = (float)bossScript3.Get("currentHealth");
        hpBar3.Value = (float)bossScript3.Get("currentHealth");
    }
    public override void _Process(double delta)
    {
        // update health bar value to boss current health
        hpBar1.Value = (float)bossScript1.Get("currentHealth");
        hpBar2.Value = (float)bossScript2.Get("currentHealth");
        hpBar3.Value = (float)bossScript3.Get("currentHealth");
    }
}