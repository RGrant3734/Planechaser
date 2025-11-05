using Godot;
using System;

public partial class CameraRevolve : Camera3D
{
    [Export]
    public MeshInstance3D device;

    [Export]
    public float distance = 5.0f;

    [Export]
    public float orbitSpeed = 1.0f;

    private float angle = 0.0f;

    [Export]
    private float zoomDistance = 0.2f;

    [Export]
    private float zoomFOV = 10.0f;

    [Export]
    private float transitionLength = 1.0f;

    private bool isZoomed = false;
    private Tween currentTween;

    public override void _Ready()
    {
        base._Ready();
        if(device == null)
        {
            GD.PrintErr("No Mesh3D found");
            return;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(device == null || isZoomed)
        {
            return;
        }
        angle += orbitSpeed * (float)delta;
        Vector3 newPos = new Vector3(
            device.GlobalPosition.X + distance * Mathf.Cos(angle),
            device.GlobalPosition.Y,
            device.GlobalPosition.Z + distance * Mathf.Sin(angle)
        );
        GlobalPosition = newPos;
        LookAt(device.GlobalPosition, Vector3.Up);
    }

    private void OnStartPressed()
    {
        // move camera into the device zooming into it
         if (currentTween != null && currentTween.IsRunning())
        {
            currentTween.Kill();
        }

        currentTween = CreateTween();

        if (!isZoomed)
        {
            isZoomed = true;
            // zoom in
            Vector3 targetPosition = device.GlobalPosition + (GlobalPosition - device.GlobalPosition).Normalized() * zoomDistance;
            
            currentTween.TweenProperty(this, "global_position", targetPosition, transitionLength)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);

            currentTween.TweenProperty(this, "fov", zoomFOV, transitionLength)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
        }
        else
        {
            // Zoom out to original position
            Vector3 originalPosition = new Vector3(
                device.GlobalPosition.X + distance * Mathf.Cos(angle),
                device.GlobalPosition.Y,
                device.GlobalPosition.Z + distance * Mathf.Sin(angle)
            );
            
            currentTween.TweenProperty(this, "global_position", originalPosition, transitionLength)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);

            currentTween.TweenProperty(this, "fov", 75.0f, transitionLength)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
            
            ZoomToggle();
        }
    }
    async private void ZoomToggle()
    {
        await ToSignal(GetTree().CreateTimer(2.0f), "timeout");
        isZoomed = false;
    }
}
