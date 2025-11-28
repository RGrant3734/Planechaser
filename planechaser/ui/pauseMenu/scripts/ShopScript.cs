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
    private FPSController playerController;

    // string array for each button's label
    private string[] healtButtonLabels = new string[]
    {
        "HEALTH II   03",
        "HEALTH III  05",
        "HEALTH IIII 10"
    };
    private string[] armorButtonLabels = new string[]
    {
        "ARMOR II    02",
        "ARMOR III   03",
        "ARMOR IIII  05"
    };
    private string[] attackButtonLabels = new string[]
    {
        "ATTACK II   02",
        "ATTACK III  03",
        "ATTACK IIII 05"
    };
    private string[] speedButtonLabels = new string[]
    {
        "SPEED II    04",
        "SPEED III   06",
        "SPEED IIII  08"
    };

    private int healthLabelIndex = 0;
    private int armorLabelIndex = 0;
    private int attackLabelIndex = 0;
    private int speedLabelIndex = 0;

    private AnimationPlayer animationPlayer;

    private Node player;
    private string _pendingPurchase = null;
    private int _pendingCost = 0;

    [Signal]
    public delegate void FragmentParticleSpendRequestedEventHandler(int cost);

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
            // store typed ref
            playerController = (FPSController)player;
            // Connect to the player's spend-result signal so we can react when the player confirms/denies a spend
            player.Connect("FragmentParticleSpendResult", new Callable(this, nameof(OnFragmentParticleSpendResult)));
        }

        // allow processing even when the game is paused
        ProcessMode = ProcessModeEnum.Always;
    }

    public void OnHealthPressed()
    {
        // check cost of current level
        int cost = ParseCostFromButtonLabel(healthButton.Text);
        if (fragmentParticleCount < cost)
            return;
        // queue a pending purchase and send the spend request to the player
        _pendingPurchase = "health";
        _pendingCost = cost;
        EmitSignal(SignalName.FragmentParticleSpendRequested, cost);
    }
    public void OnArmorPressed()
    {
        int cost = ParseCostFromButtonLabel(armorButton.Text);
        if (fragmentParticleCount < cost)
            return;
        _pendingPurchase = "armor";
        _pendingCost = cost;
        EmitSignal(SignalName.FragmentParticleSpendRequested, cost);
    }
    public void OnAttackPressed()
    {
        int cost = ParseCostFromButtonLabel(attackButton.Text);
        if (fragmentParticleCount < cost)
            return;
        _pendingPurchase = "attack";
        _pendingCost = cost;
        EmitSignal(SignalName.FragmentParticleSpendRequested, cost);
    }
    public void OnSpeedPressed()
    {
        int cost = ParseCostFromButtonLabel(speedButton.Text);
        if (fragmentParticleCount < cost)
            return;
        _pendingPurchase = "speed";
        _pendingCost = cost;
        EmitSignal(SignalName.FragmentParticleSpendRequested, cost);
    }

    public void OnBackPressed()
    {
        animationPlayer.Play("fade_out");
    }
    
    public void OnShopPressed()
    {
        animationPlayer.Play("fade_in");
        UpdateAllButtonsInteractivity();
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
            // Keep local copy in sync with player
            if (playerController != null)
                fragmentParticleCount = playerController.fragmentParticleCount;
            else
                fragmentParticleCount += 1;
            UpdateCurrencyDisplay();
            UpdateAllButtonsInteractivity();
        }
    }

    private void OnFragmentParticleSpendResult(bool success, int newCount)
    {
        // If there is no pending purchase just update our display
        if (_pendingPurchase == null)
        {
            fragmentParticleCount = newCount;
            UpdateCurrencyDisplay();
            UpdateAllButtonsInteractivity();
            return;
        }

        string pending = _pendingPurchase;
        _pendingPurchase = null;
        _pendingCost = 0;

        if (!success)
        {
            // Failed to spend, update current count
            fragmentParticleCount = newCount;
            UpdateCurrencyDisplay();
            UpdateAllButtonsInteractivity();
            // optionally add feedback for failed purchase
            return;
        }

        // Success: apply the upgrade via the playerController
        switch (pending)
        {
            case "health":
                playerController?.OnHealthPressed();
                if (healthLabelIndex < healtButtonLabels.Length)
                    healthLabelIndex++;
                if (healthLabelIndex <= healtButtonLabels.Length)
                    healthButton.Text = healtButtonLabels[Math.Max(0, healthLabelIndex - 1)];
                if (healthLabelIndex >= healtButtonLabels.Length)
                {
                    healthButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
                    healthButton.Disabled = true;
                }
                break;
            case "armor":
                playerController?.OnArmorPressed();
                if (armorLabelIndex < armorButtonLabels.Length)
                    armorLabelIndex++;
                if (armorLabelIndex <= armorButtonLabels.Length)
                    armorButton.Text = armorButtonLabels[Math.Max(0, armorLabelIndex - 1)];
                if (armorLabelIndex >= armorButtonLabels.Length)
                {
                    armorButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
                    armorButton.Disabled = true;
                }
                break;
            case "attack":
                playerController?.OnAttackPressed();
                if (attackLabelIndex < attackButtonLabels.Length)
                    attackLabelIndex++;
                if (attackLabelIndex <= attackButtonLabels.Length)
                    attackButton.Text = attackButtonLabels[Math.Max(0, attackLabelIndex - 1)];
                if (attackLabelIndex >= attackButtonLabels.Length)
                {
                    attackButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
                    attackButton.Disabled = true;
                }
                break;
            case "speed":
                playerController?.OnSpeedPressed();
                if (speedLabelIndex < speedButtonLabels.Length)
                    speedLabelIndex++;
                if (speedLabelIndex <= speedButtonLabels.Length)
                    speedButton.Text = speedButtonLabels[Math.Max(0, speedLabelIndex - 1)];
                if (speedLabelIndex >= speedButtonLabels.Length)
                {
                    speedButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
                    speedButton.Disabled = true;
                }
                break;
        }

        fragmentParticleCount = newCount;
        UpdateCurrencyDisplay();
        UpdateAllButtonsInteractivity();
    }

    private void UpdateCurrencyDisplay()
    {
        fragmentParticleLabel.Text = new string($"FRAGMENT PARTICLES: {fragmentParticleCount}");
    }

    private int ParseCostFromButtonLabel(string text)
    {
        if (string.IsNullOrEmpty(text))
            return int.MaxValue;
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return int.MaxValue;
        var last = parts[parts.Length - 1];
        if (int.TryParse(last, out int cost))
            return cost;
        return int.MaxValue;
    }

    private void UpdateAllButtonsInteractivity()
    {
        // health
        healthButton.Disabled = fragmentParticleCount < ParseCostFromButtonLabel(healthButton.Text) || (healthLabelIndex >= healtButtonLabels.Length);
        // armor
        armorButton.Disabled = fragmentParticleCount < ParseCostFromButtonLabel(armorButton.Text) || (armorLabelIndex >= armorButtonLabels.Length);
        // attack
        attackButton.Disabled = fragmentParticleCount < ParseCostFromButtonLabel(attackButton.Text) || (attackLabelIndex >= attackButtonLabels.Length);
        // speed
        speedButton.Disabled = fragmentParticleCount < ParseCostFromButtonLabel(speedButton.Text) || (speedLabelIndex >= speedButtonLabels.Length);
    }
}
