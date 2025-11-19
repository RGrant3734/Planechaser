using Godot;
using System;

public partial class State : Node
{
	// State transition signal that states will call to transition over to a new state!
    [Signal] public delegate void TransitionEventHandler(string state);

    // Generic virtual functions that help define what a state is. If the states
    // dont override these funcitons then the base will be calle. Try protected
    public virtual void Enter(State prevState) { }
    public virtual void Exit() { }
    public virtual void Update(double delta) { }    // _Process()
    public virtual void PhysicsUpdate(double delta) { }     // _PhysicsProcess()
}
