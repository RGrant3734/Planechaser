using Godot;
using System;

public partial class TitleChanger : Label
{
    public TimerControl swapTimer;
    public int currentPlane;
    public override void _Ready()
    {
        base._Ready();
        swapTimer = GetTree().Root.GetNode<TimerControl>("Menu/Background/Timer");
		swapTimer.Timeout += Planeshift;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

    }

    public async void Planeshift()
    {
        await ToSignal(GetTree().CreateTimer(0.8), "timeout");
		//Menu screen shifting
		if(currentPlane == 2)
            currentPlane = 0;
        else
            currentPlane++;
		
        switch (currentPlane)
		{
			case 0:
				LabelSettings.FontColor = Colors.Cyan;
				break;
			case 1:
				LabelSettings.FontColor = Colors.Yellow;
				break;
			case 2:
				LabelSettings.FontColor = Colors.Red;
				break;
			default:
				GD.Print("TypeSetError(Title)");
				break;
		}
    }
}
