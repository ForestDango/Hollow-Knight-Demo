using System;
using InControl;

public class HeroActions : PlayerActionSet
{
    public PlayerAction left;
    public PlayerAction right;
    public PlayerAction up;
    public PlayerAction down;
    public PlayerAction menuSubmit;
    public PlayerAction menuCancel;
    public PlayerTwoAxisAction moveVector;
    public PlayerAction attack;
    public PlayerAction jump;
    public PlayerAction dash;
    public PlayerAction cast;
    public PlayerAction focus;
    public PlayerAction quickCast;
    public PlayerAction openInventory;

    public PlayerAction pause;
    public HeroActions()
    {
	menuSubmit = CreatePlayerAction("Submit");
	menuCancel = CreatePlayerAction("Cancel");
	left = CreatePlayerAction("Left");
	left.StateThreshold = 0.3f;
	right = CreatePlayerAction("Right");
	right.StateThreshold = 0.3f;
	up = CreatePlayerAction("Up");
	up.StateThreshold = 0.3f;
	down = CreatePlayerAction("Down");
	down.StateThreshold = 0.3f;
	moveVector = CreateTwoAxisPlayerAction(left, right, down, up);
	moveVector.LowerDeadZone = 0.15f;
	moveVector.UpperDeadZone = 0.95f;
	attack = CreatePlayerAction("Attack");
	jump = CreatePlayerAction("Jump");
	dash = CreatePlayerAction("Dash");
	cast = CreatePlayerAction("Cast");
	focus = CreatePlayerAction("Focus");
	quickCast = CreatePlayerAction("QuickCast");
	openInventory = CreatePlayerAction("Inventory");

	pause = CreatePlayerAction("Pause");
    }
}
