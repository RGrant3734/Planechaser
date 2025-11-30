using Godot;
using System;

public partial class AmmoCounter : Label
{	
	// Signal for updating the ammo count when shooting
	[Signal] public delegate void UpdateAmmoCountSignalEventHandler(int AmmoCount);
	// Signal for when swaping weapons 
	[Signal] public delegate void SwapWeaponAmmoCountSignalEventHandler(int AmmoCount);
	// Need reference to the player to get weapon reference
	[Export] private FPSController PLAYER;
	// Lerp target font
	private int TargetFontSize;
	// Default target font set in the editor
	private int DefaultFontSize;
	// Empty text color
	private Color empty;
	// available text color
	private Color available;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
    {
		// Grab the editor set font size
        DefaultFontSize = GetThemeFontSize("font_size");
		// Initialize target font
		TargetFontSize = DefaultFontSize;
		// Wait for the player before accessing weapon controller properties
		await ToSignal(PLAYER, "ready");
		Text = PLAYER.WEAPON.currentWeapon.AmmoCount.ToString();
	
		// Initialize text colors
		empty = new Color(1,0,0);
		available = new Color(1,1,1);

		AddThemeColorOverride("font_color", new Color(PLAYER.WEAPON.currentPlaneColor));
		// Subscribe functions for both signals
		UpdateAmmoCountSignal += AmmoSpent;
		SwapWeaponAmmoCountSignal += UpdateText;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		// Want to lerp the font back to the default every frame
		TargetFontSize = (int)Mathf.Lerp(TargetFontSize, DefaultFontSize, 0.1f);
        AddThemeFontSizeOverride("font_size", TargetFontSize);
    }

	private void AmmoSpent(int AmmoCount)
    {
		// Set the target position up which will cause the ammo count to grow
		// and smoothly come back to default position in _Process()
		TargetFontSize = DefaultFontSize * 2; 
		UpdateText(AmmoCount);
    }

	private void UpdateText(int AmmoCount)
    {
		// Set the text color appropriately 
		AddThemeColorOverride("font_color", new Color(PLAYER.WEAPON.currentPlaneColor));
	
		// Update the text based on the current weapons ammo this works 
		// For both swapping and shooting
		Text = AmmoCount > 0 ? AmmoCount.ToString() : new string("OUT!");
    }
}
