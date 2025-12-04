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
    private ProgressBar shieldBar1;
    private ProgressBar shieldBar2;
    private ProgressBar shieldBar3;

    public override void _Ready()
    {
        hpBar1 = GetNode<ProgressBar>("Bar1");
        hpBar2 = GetNode<ProgressBar>("Bar2");
        hpBar3 = GetNode<ProgressBar>("Bar3");
        shieldBar1 = GetNode<ProgressBar>("ShieldBar1");
        shieldBar2 = GetNode<ProgressBar>("ShieldBar2");
        shieldBar3 = GetNode<ProgressBar>("ShieldBar3");
        // set max value to boss max health
        hpBar1.MaxValue = (float)bossScript1.Get("currentHealth");
        hpBar1.Value = (float)bossScript1.Get("currentHealth");
        hpBar2.MaxValue = (float)bossScript2.Get("currentHealth");
        hpBar2.Value = (float)bossScript2.Get("currentHealth");
        hpBar3.MaxValue = (float)bossScript3.Get("currentHealth");
        hpBar3.Value = (float)bossScript3.Get("currentHealth");
        shieldBar1.MaxValue = (float)bossScript1.Get("armorMax");
        shieldBar1.Value = (float)bossScript1.Get("armorMax");
        shieldBar2.MaxValue = (float)bossScript2.Get("armorMax");
        shieldBar2.Value = (float)bossScript2.Get("armorMax");
        shieldBar3.MaxValue = (float)bossScript3.Get("armorMax");
        shieldBar3.Value = (float)bossScript3.Get("armorMax");
    }
    public override void _Process(double delta)
    {
        // update health bar value to boss current health
        hpBar1.Value = (float)bossScript1.Get("currentHealth");
        hpBar2.Value = (float)bossScript2.Get("currentHealth");
        hpBar3.Value = (float)bossScript3.Get("currentHealth");
        shieldBar1.Value = (float)bossScript1.Get("armor");
        shieldBar2.Value = (float)bossScript2.Get("armor");
        shieldBar3.Value = (float)bossScript3.Get("armor");
    }
}