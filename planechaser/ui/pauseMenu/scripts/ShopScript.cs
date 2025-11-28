using Godot;
using System;

public partial class ShopScript : Control
{
    private Button healthButton;
    private Button armorButton;
    private Button attackButton;
    private Button speedButton;

    private Label fragmentParticleLabel;
    private int fragmentParticleCount = 0;

    // string array for each button's label
    private string[] healtButtonLabels = new string[]
    {
        "HEALTH II   $149.99",
        "HEALTH III  $199.99",
        "HEALTH IIII $249.99"
    };
    private string[] armorButtonLabels = new string[]
    {
        "ARMOR II    $124.99",
        "ARMOR III   $149.99",
        "ARMOR IIII  $174.99"
    };
    private string[] attackButtonLabels = new string[]
    {
        "ATTACK II   $124.99",
        "ATTACK III  $149.99",
        "ATTACK IIII $174.99"
    };
    private string[] speedButtonLabels = new string[]
    {
        "SPEED II    $199.99",
        "SPEED III   $299.99",
        "SPEED IIII  $399.99"
    };

    private int healthLabelIndex = 0;
    private int armorLabelIndex = 0;
    private int attackLabelIndex = 0;
    private int speedLabelIndex = 0;

    private AnimationPlayer animationPlayer;

    private Node player;

    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationFinished += OnAnimationFinished;

        // set all buttons
        healthButton = GetNode<Button>("PanelContainer/ColorRect/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/HealthButton");
        armorButton = GetNode<Button>("PanelContainer/ColorRect/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/ArmorButton");
        attackButton = GetNode<Button>("PanelContainer/ColorRect/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/AttackButton");
        speedButton = GetNode<Button>("PanelContainer/ColorRect/MarginContainer/VBoxContainer/ScrollContainer/VBoxContainer/SpeedButton");
        fragmentParticleLabel = GetNode<Label>("PanelContainer/ColorRect/FragmentParticleLabel");
        
        player = GetTree().Root.GetNode<Node>("GameMaster/Player");
        if (player != null)
        {
            // Connect to the custom signal
            player.Connect("MyCustomSignal", new Callable(this, nameof(OnFragmentParticleCollected)));
        }

        // allow processing even when the game is paused
        ProcessMode = ProcessModeEnum.Always;
    }

    public void OnHealthPressed()
    {
        // change the button text to the next label
        if (healthLabelIndex > healtButtonLabels.Length)
            return;
        healthLabelIndex++;
        if(healthLabelIndex <= healtButtonLabels.Length)
            healthButton.Text = healtButtonLabels[healthLabelIndex - 1];

        // change button color to dark gray if maxed out
        if (healthLabelIndex == healtButtonLabels.Length+1)
        {
            healthButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
            // disable button
            healthButton.Disabled = true;
        }
    }
    public void OnArmorPressed()
    {
        // change the button text to the next label
        if (armorLabelIndex > armorButtonLabels.Length)
            return;
        armorLabelIndex++;
        if(armorLabelIndex <= armorButtonLabels.Length)
            armorButton.Text = armorButtonLabels[armorLabelIndex - 1];

        // change button color to dark gray if maxed out
        if (armorLabelIndex == armorButtonLabels.Length+1)
        {
            armorButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
            // disable button
            armorButton.Disabled = true;
        }
    }
    public void OnAttackPressed()
    {
        // change the button text to the next label
        if (attackLabelIndex > attackButtonLabels.Length)
            return;
        attackLabelIndex++;
        if(attackLabelIndex <= attackButtonLabels.Length)
            attackButton.Text = attackButtonLabels[attackLabelIndex - 1];

        // change button color to dark gray if maxed out
        if (attackLabelIndex == attackButtonLabels.Length+1)
        {
            attackButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
            // disable button
            attackButton.Disabled = true;
        }
    }
    public void OnSpeedPressed()
    {
        // change the button text to the next label
        if (speedLabelIndex > speedButtonLabels.Length)
            return;
        speedLabelIndex++;
        if(speedLabelIndex <= speedButtonLabels.Length)
            speedButton.Text = speedButtonLabels[speedLabelIndex - 1];

        // change button color to dark gray if maxed out
        if (speedLabelIndex == speedButtonLabels.Length+1)
        {
            speedButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
            // disable button
            speedButton.Disabled = true;
        }
    }

    public void OnBackPressed()
    {
        animationPlayer.Play("fade_out");
    }
    
    public void OnShopPressed()
    {
        animationPlayer.Play("fade_in");
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "fade_out")
            Visible = false;
    }

    private void OnFragmentParticleCollected(string message)
    {
        if (message == "FragmentParticleCollected")
        {
            fragmentParticleCount += 1;
            fragmentParticleLabel.Text = new string($"FRAGMENT PARTICLES: {fragmentParticleCount}");
        }
    }
}
