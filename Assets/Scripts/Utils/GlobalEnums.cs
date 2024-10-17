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

    public enum CameraFadeType
    {
	LEVEL_TRANSITION,
	HERO_DEATH,
	HERO_HAZARD_DEATH,
	JUST_FADE,
	START_FADE
    }

    public enum CancelAction
    {
	DoNothing,
	GoToMainMenu,
	GoToOptionsMenu,
	GoToVideoMenu,
	GoToPauseMenu,
	LeaveOptionsMenu,
	GoToExitPrompt,
	GoToProfileMenu,
	GoToControllerMenu,
	ApplyRemapGamepadSettings,
	ApplyAudioSettings,
	ApplyVideoSettings,
	ApplyGameSettings,
	ApplyKeyboardSettings,
	GoToExtrasMenu,
	ApplyControllerSettings,
	GoToExplicitSwitchUser,
	ReturnToProfileMenu
    }

    public enum CollisionSide
    {
	top,
	left,
	right,
	bottom,
	other
    }

    public enum DamageMode
    {
	FULL_DAMAGE,
	HAZARD_ONLY,
	NO_DAMAGE
    }

    public enum GameState
    {
	INACTIVE,
	MAIN_MENU,
	LOADING,
	ENTERING_LEVEL,
	PLAYING,
	PAUSED,
	EXITING_LEVEL,
	CUTSCENE,
	PRIMER
    }

    public enum GatePosition
    {
	top,
	right,
	left,
	bottom,
	door,
	unknown
    }
    public enum MainMenuState
    {
	LOGO, //logo����
	MAIN_MENU, //���˵�����
	OPTIONS_MENU, //ѡ�����
	GAMEPAD_MENU, //�ֱ�����
	KEYBOARD_MENU, //���̽���
	SAVE_PROFILES, //����ȷ�Ͻ���
	AUDIO_MENU, //�������ý���
	VIDEO_MENU, //��Ƶ���ý���
	EXIT_PROMPT, //�˳���Ϸȷ�Ͻ���
	OVERSCAN_MENU, //�ֱ��ʽ���
	GAME_OPTIONS_MENU, //��Ϸѡ�����
	ACHIEVEMENTS_MENU, //�ɾͽ���
	QUIT_GAME_PROMPT, //�˳���Ϸȷ�Ͻ���
	RESOLUTION_PROMPT, //�ֱ��ʽ���
	BRIGHTNESS_MENU, //���Ƚ���
	PAUSE_MENU, //��ͣ�˵�����
	PLAY_MODE_MENU, //��Ϸģʽ���棨��ͨ���ֻ꣬Ѱ���ߣ�
	EXTRAS_MENU, //�������ݽ���
	REMAP_GAMEPAD_MENU, //���°��ֱ���������
	EXTRAS_CONTENT_MENU, //�������ݽ���
	ENGAGE_MENU, //ȷ�Ͻ���
	NO_SAVE_MENU //���������
    }

    public enum HazardType
    {
	NON_HAZARD,
	SPIKES,
	ACID,
	LAVA,
	PIT
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
	FALLING,
	TAKE_HIT
    }

    public enum HeroTransitionState
    {
	WAITING_TO_TRANSITION,
	EXITING_SCENE,
	WAITING_TO_ENTER_LEVEL,
	ENTERING_SCENE,
	DROPPING_DOWN
    }
    public enum SkipPromptMode
    {
	SKIP_PROMPT,
	SKIP_INSTANT,
	NOT_SKIPPABLE,
	NOT_SKIPPABLE_DUE_TO_LOADING
    }

    public enum UIState
    {
	INACTIVE,
	MAIN_MENU_HOME,
	LOADING,
	CUTSCENE,
	PLAYING,
	PAUSED,
	OPTIONS
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
	HERO_BOX,
	GRASS,
	ENEMY_ATTACK,
	BOUNCER = 24,
	SOFT_TERRAIN = 25,
	CORPSE
    }
}
