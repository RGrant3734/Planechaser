using Godot;
using System;

public partial class Device : MeshInstance3D
{

    private StandardMaterial3D material;

    private Timer timer;

    private Tween currentTween;

    private bool isWhite = false;

    private Color originalAlbedo;

    private float transitionLength = 1.0f;

    public override void _Ready()
    {
        base._Ready();
        material = (StandardMaterial3D)Mesh.SurfaceGetMaterial(0);
        if (material == null)
        {
            GD.PrintErr("No StandardMaterial3D found on surface 0 of the mesh.");
            return;
        }
        timer = GetTree().Root.GetNode<Timer>("Menu/Background/Timer");
        timer.Start();
        timer.Timeout += ChangeColor;
    }

    private void ChangeColor()
    {
        if(isWhite)
        {
            return;
        }
        if(material.AlbedoColor == Colors.Cyan)
        {
            // change albedo color to red
            material.AlbedoColor = Colors.Red;
            // change emmission to red
            material.Emission = Colors.Red;
        }
        else if(material.AlbedoColor == Colors.Red)
        {
            material.AlbedoColor = Colors.Yellow;
            material.Emission = Colors.Yellow;
        }
        else
        {
            material.AlbedoColor = Colors.Cyan;
            material.Emission = Colors.Cyan;
        }
        originalAlbedo = material.AlbedoColor;
    }

    private void OnStartPressed()
    {
        // change albedo and emmission to white with tween
        if(currentTween != null && currentTween.IsRunning())
        {
            currentTween.Kill();
        }
        if(!isWhite)
        {
            originalAlbedo = material.AlbedoColor;
            currentTween = CreateTween();
            currentTween.TweenProperty(material, "albedo_color", Colors.White, transitionLength)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            currentTween.TweenProperty(material, "emission", Colors.White, transitionLength)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            isWhite = true;
        }
        else
        {
            currentTween = CreateTween();
            currentTween.TweenProperty(material, "albedo_color", originalAlbedo, transitionLength)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            currentTween.TweenProperty(material, "emission", originalAlbedo, transitionLength)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            isWhite = false;
        }
        
    }

    public bool GetIsWhite()
    {
        return isWhite;
    }
}
