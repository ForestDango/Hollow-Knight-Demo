using System;

namespace GlobalEnums
{
    public enum ActorStates
    {
	grounded,
	idle,
	running,
	airborne,
	wall_sliding,
	hard_landing,
	dash_landing,
	no_input,
	previous
    }

    public enum AttackDirection
    {
	normal,
	upward,
	downward
    }

    public enum CollisionSide
    {
	top,
	left,
	right,
	bottom,
	other
    }

    public enum HeroSounds
    {
	FOOTSETP_RUN,
	FOOTSTEP_WALK,
	SOFT_LANDING,
	HARD_LANDING,
	JUMP,
	BACK_DASH,
	DASH,
	FALLING
    }

    public enum PhysLayers
    {
	DEFAULT,
	IGNORE_RAYCAST = 2,
	WATER = 4,
	UI,
	TERRAIN = 8,
	PLAYER,
	TRANSITION_GATES,
	ENEMIES,
	PROJECTILES,
	HERO_DETECTOR,
	TERRAIN_DETECTOR,
	ENEMY_DETECTOR,
	ITEM,
	HERO_ATTACK,
	PARTICLE,
	INTERACTIVE_OBJECT,
	BOUNCER = 24,
	SOFT_TERRAIN = 25
    }
}
