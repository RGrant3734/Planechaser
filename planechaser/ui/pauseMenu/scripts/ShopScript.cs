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

    private AudioStreamPlayer audio;

    // string array for each button's label
    private string[] healtButtonLabels = new string[]
    {
        "HEALTH I    01",
        "HEALTH II   03",
        "HEALTH III  05",
        "HEALTH IIII 10"
    };
    private string[] armorButtonLabels = new string[]
    {
        "ARMOR I     01",
        "ARMOR II    02",
        "ARMOR III   03",
        "ARMOR IIII  05"
    };
    private string[] attackButtonLabels = new string[]
    {
        "ATTACK I    01",
        "ATTACK II   02",
        "ATTACK III  03",
        "ATTACK IIII 05"
    };
    private string[] speedButtonLabels = new string[]
    {
        "SPEED I     02",
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
            // set the initial currency display from the player
            fragmentParticleCount = playerController.fragmentParticleCount;
            UpdateCurrencyDisplay();
            // set initial button labels safely
            healthButton.Text = healtButtonLabels[Math.Min(healthLabelIndex, healtButtonLabels.Length - 1)];
            armorButton.Text = armorButtonLabels[Math.Min(armorLabelIndex, armorButtonLabels.Length - 1)];
            attackButton.Text = attackButtonLabels[Math.Min(attackLabelIndex, attackButtonLabels.Length - 1)];
            speedButton.Text = speedButtonLabels[Math.Min(speedLabelIndex, speedButtonLabels.Length - 1)];
            UpdateAllButtonsInteractivity();
        }

        // allow processing even when the game is paused
        ProcessMode = ProcessModeEnum.Always;

        // music
        audio = GetNode<AudioStreamPlayer>("GameDevShopTheme");
    }

    public void OnHealthPressed()
    {
        // check costs and bounds of labels
        int cost = ParseCostFromButtonLabel(healthButton.Text);
        if (healthLabelIndex >= healtButtonLabels.Length)
            return; // already maxed out
        if (fragmentParticleCount < cost)
            return; // cannot afford
        // queue a pending purchase and send the spend request to the player
        _pendingPurchase = "health";
        _pendingCost = cost;
        EmitSignal(SignalName.FragmentParticleSpendRequested, cost);
    }
    public void OnArmorPressed()
    {
        int cost = ParseCostFromButtonLabel(armorButton.Text);
        if (armorLabelIndex >= armorButtonLabels.Length)
            return;
        if (fragmentParticleCount < cost)
            return;
        _pendingPurchase = "armor";
        _pendingCost = cost;
        EmitSignal(SignalName.FragmentParticleSpendRequested, cost);
    }
    public void OnAttackPressed()
    {
        int cost = ParseCostFromButtonLabel(attackButton.Text);
        if (attackLabelIndex >= attackButtonLabels.Length)
            return;
        if (fragmentParticleCount < cost)
            return;
        _pendingPurchase = "attack";
        _pendingCost = cost;
        EmitSignal(SignalName.FragmentParticleSpendRequested, cost);
    }
    public void OnSpeedPressed()
    {
        int cost = ParseCostFromButtonLabel(speedButton.Text);
        if (speedLabelIndex >= speedButtonLabels.Length)
            return;
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
        audio.Play();
        UpdateAllButtonsInteractivity();
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "fade_out")
        {
            Visible = false;
            audio.Stop();
        }
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
        int len;
        switch (pending)
        {
            case "health":
                playerController?.OnHealthPressed();
                len = healtButtonLabels.Length;
                // if not last-level label: advance and set text
                if (healthLabelIndex < len - 1)
                {
                    healthLabelIndex++;
                    healthButton.Text = healtButtonLabels[healthLabelIndex];
                }
                else
                {
                    // Final purchase — set text to last label and disable
                    healthButton.Text = healtButtonLabels[len - 1];
                    healthLabelIndex = len; // mark as maxed
                    healthButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
                    healthButton.Disabled = true;
                }
                {
                }
                break;
            case "armor":
                playerController?.OnArmorPressed();
                len = armorButtonLabels.Length;
                if (armorLabelIndex < len - 1)
                {
                    armorLabelIndex++;
                    armorButton.Text = armorButtonLabels[armorLabelIndex];
                }
                else
                {
                    armorButton.Text = armorButtonLabels[len - 1];
                    armorLabelIndex = len;
                    armorButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
                    armorButton.Disabled = true;
                }
                {
                }
                break;
            case "attack":
                playerController?.OnAttackPressed();
                len = attackButtonLabels.Length;
                if (attackLabelIndex < len - 1)
                {
                    attackLabelIndex++;
                    attackButton.Text = attackButtonLabels[attackLabelIndex];
                }
                else
                {
                    attackButton.Text = attackButtonLabels[len - 1];
                    attackLabelIndex = len;
                    attackButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
                    attackButton.Disabled = true;
                }
                {
                }
                break;
            case "speed":
                playerController?.OnSpeedPressed();
                len = speedButtonLabels.Length;
                if (speedLabelIndex < len - 1)
                {
                    speedLabelIndex++;
                    speedButton.Text = speedButtonLabels[speedLabelIndex];
                }
                else
                {
                    speedButton.Text = speedButtonLabels[len - 1];
                    speedLabelIndex = len;
                    speedButton.Modulate = new Color(0.3f, 0.3f, 0.3f);
                    speedButton.Disabled = true;
                }
                {
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
