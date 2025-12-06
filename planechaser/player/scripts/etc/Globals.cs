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
    public static float PlayerFragmentParticleCount = 10f;
    
    // Persistent shop upgrade levels
    public static int HealthUpgradeLevel = 0;
    public static int ArmorUpgradeLevel = 0;
    public static int AttackUpgradeLevel = 0;
    public static int SpeedUpgradeLevel = 0;
    
    private static bool isFirstTime = true;

    // A method to sync stats FROM the player
    public static void SyncFromPlayer()
    {
        if (player == null || !isFirstTime)
            return;

        PlayerSpeed = player.speed;
        PlayerHealth = player.health;
        PlayerArmor = player.armor;
        PlayerFragmentParticleCount = player.fragmentParticleCount;

        // So that the stats persist when comming into the second and third levels
        isFirstTime = false;
    }

    public static void PlayerReset()
    {
        PlayerHealth = player.health;
        PlayerArmor = player.armor;
        player.WEAPON.PlayerReset();
    }

}
