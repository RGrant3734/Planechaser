using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class EyeCounter : Label
{
    public LightBossHealthBarLogic LightHealth;
    [Export(PropertyHint.Range, "0, 2, ")]
    public int eyeColor = 0;
    [Export]
    public TextureRect blueEyeUI;
    [Export]
    public TextureRect yellowEyeUI;
    [Export]
    public TextureRect redEyeUI;
    [Export]
    public Texture2D blueEyeClosed;
    [Export]
    public Texture2D yellowEyeClosed;
    [Export]
    public Texture2D redEyeClosed;
    public override void _Ready()
    {
        LightHealth = GetParent<LightBossHealthBarLogic>();
        LightHealth.UpdateEyes += UpdateEyes;
    }
    
    public void UpdateEyes()
    {
        if(eyeColor == 0)
        {
            Text = LightHealth.blueEyesCount.ToString();
            if(LightHealth.blueEyesCount == 0)
            {
                blueEyeUI.Texture = blueEyeClosed;
                this.Visible = false;
            }
        }
        if(eyeColor == 1)
        {
            Text = LightHealth.yellowEyesCount.ToString();
            if(LightHealth.yellowEyesCount == 0)
            {
                yellowEyeUI.Texture = yellowEyeClosed;
                this.Visible = false;
            }
        }
        if(eyeColor == 2)
        {
            Text = LightHealth.redEyesCount.ToString();
            if(LightHealth.redEyesCount == 0)
            {
                redEyeUI.Texture = redEyeClosed;
                this.Visible = false;
            }
        }
    }
}
