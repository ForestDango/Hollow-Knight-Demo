using System;
using InControl;

public class HeroActions : PlayerActionSet
{
    public PlayerAction left;
    public PlayerAction right;
    public PlayerAction up;
    public PlayerAction down;
    public PlayerTwoAxisAction moveVector;

    public HeroActions()
    {
	left = CreatePlayerAction("Left");
	left.StateThreshold = 0.3f;
	right = CreatePlayerAction("Right");
	right.StateThreshold = 0.3f;
	up = CreatePlayerAction("Up");
	up.StateThreshold = 0.3f;
	down = CreatePlayerAction("Down");
	down.StateThreshold = 0.3f;
	moveVector = CreateTwoAxisPlayerAction(left, right, up, down);
	moveVector.LowerDeadZone = 0.15f;
	moveVector.UpperDeadZone = 0.95f;
    }
}
