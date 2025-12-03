using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class lightController : Node3D
{
	[Signal]
	public delegate void LightDeathEventHandler();
	public int totalEyes;
	public int blueEyes;
	public int yellowEyes;
	public int redEyes;
	public float totalHealth;
	public float eyeHealth;
	public override void _Ready()
	{
		// Gets each eye, subscribes to signal and counts each one
		foreach (lightEyeController eyeball in GetTree().GetNodesInGroup("Eye"))
		{
			eyeball.EyeDeath += EyeDeath;
			totalEyes++;
			if(eyeball.nativePlane == 0)
			{
				blueEyes++;
			} else if (eyeball.nativePlane == 1)
			{
				yellowEyes++;
			}
			else
			{
				redEyes++;
				}
			eyeHealth = eyeball.totalHealth;
		}
		totalHealth = totalEyes * eyeHealth;
		//GD.Print("Blue: ", blueEyes, "| Yellow: ", yellowEyes, "| Red: ", redEyes, "| Total: ", totalEyes);
	}
	public void EyeDeath()
	{
		// When an eye dies, the health will go down for main bar until it dies
		totalHealth -= eyeHealth;
		if(totalHealth <= 0)
		{
			EmitSignal(SignalName.LightDeath);
		}
	}
    public override void _Process(double delta)
    {
        if(totalHealth <= 0)
        {
            GetTree().ChangeSceneToFile("res://levels/level2.tscn");
        }
    }

}
