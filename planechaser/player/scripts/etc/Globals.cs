using Godot;
using System;
using System.Resources;

public partial class Globals : Node
{
	// Reference to player conponent. This will act as our singleton
    // Its static here since we dont want to create a Globals object
    static public FPSController player { get; set; }
    // Persistent stats
    public static float PlayerSpeed = 10f;
    public static float PlayerHealth = 100f;
    public static float PlayerArmor = 0f;
    public static float PlayerFragmentParticleCount = 0f;
    
    // Persistent shop upgrade levels
    public static int HealthUpgradeLevel = 0;
    public static int ArmorUpgradeLevel = 0;
    public static int AttackUpgradeLevel = 0;
    public static int SpeedUpgradeLevel = 0;
    
    private static bool isFirstTime = true;

    // Other information retained
    public static int blueAmmoStart = 120;
    public static int yellowAmmoStart = 250;
    public static int redAmmoStart = 30;

    // Fun stats
    public static long gruntsKilled = 0;
    public static long knightsKilled = 0;
    public static long magesKilled = 0;
    // Flags
    public static bool shadowDefeated = false;
    public static bool lightDefeated = false;
    public static bool bloodDefeated = false;

    // Weapon resources
    public static WeaponResource blueWeapon;
    public static WeaponResource yellowWeapon;
    public static WeaponResource redWeapon;

    public override void _Ready()
    {
        yellowWeapon = GD.Load<WeaponResource>("res://player/assets/weapons/rifle/RifleResource.tres");
		blueWeapon = GD.Load<WeaponResource>("res://player/assets/weapons/sniper/SniperResource.tres");
		redWeapon = GD.Load<WeaponResource>("res://player/assets/weapons/launcher/LauncherResource.tres");
    }

    // A method to sync stats FROM the player
    public static void SyncFromPlayer()
    {
        if (player == null || !isFirstTime)
            return;

        PlayerSpeed = player.speed;
        PlayerHealth = player.health;
        PlayerArmor = player.armor;
        blueAmmoStart = blueWeapon.AmmoCount;
        yellowAmmoStart = yellowWeapon.AmmoCount;
        redAmmoStart = redWeapon.AmmoCount;
        PlayerFragmentParticleCount = player.fragmentParticleCount;

        // So that the stats persist when comming into the second and third levels
        isFirstTime = false;
    }
    public static void PlayerReset()
    {
        PlayerHealth = player.health;
        PlayerArmor = player.armor;
        blueWeapon.AmmoCount = blueAmmoStart;
        yellowWeapon.AmmoCount = yellowAmmoStart;
        redWeapon.AmmoCount = redAmmoStart;
        player.WEAPON.PlayerReset();
    }
}
