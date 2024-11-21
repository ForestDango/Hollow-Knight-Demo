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
	INACTIVE, //非活跃
	MAIN_MENU, //主菜单
	LOADING, //加载
	ENTERING_LEVEL, //进入场景
	PLAYING, //游玩
	PAUSED, //暂停
	EXITING_LEVEL, //里面场景
	CUTSCENE, //过场
	PRIMER //先前的
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
	LOGO, //logo界面
	MAIN_MENU, //主菜单界面
	OPTIONS_MENU, //选项界面
	GAMEPAD_MENU, //手柄界面
	KEYBOARD_MENU, //键盘界面
	SAVE_PROFILES, //保存确认界面
	AUDIO_MENU, //声音设置界面
	VIDEO_MENU, //视频设置界面
	EXIT_PROMPT, //退出游戏确认界面
	OVERSCAN_MENU, //分辨率界面
	GAME_OPTIONS_MENU, //游戏选项界面
	ACHIEVEMENTS_MENU, //成就界面
	QUIT_GAME_PROMPT, //退出游戏确认界面
	RESOLUTION_PROMPT, //分辨率界面
	BRIGHTNESS_MENU, //亮度界面
	PAUSE_MENU, //暂停菜单界面
	PLAY_MODE_MENU, //游戏模式界面（普通，钢魂，寻神者）
	EXTRAS_MENU, //额外内容界面
	REMAP_GAMEPAD_MENU, //重新绑定手柄按键界面
	EXTRAS_CONTENT_MENU, //额外内容界面
	ENGAGE_MENU, //确认界面
	NO_SAVE_MENU //不保存界面
    }

    public enum MapZone
    {
	NONE,
	TEST_AREA,
	KINGS_PASS,
	CLIFFS,
	TOWN,
	CROSSROADS,
	GREEN_PATH,
	FOG_CANYON,
	SHAMAN_TEMPLE,
	QUEENS_STATION,
	GODS_GLORY,
    }

    public enum HeroActionButton
    {
	JUMP,
	ATTACK,
	DASH,
	SUPER_DASH,
	CAST,
	QUICK_MAP,
	INVENTORY,
	MENU_SUBMIT,
	MENU_CANCEL,
	DREAM_NAIL,
	UP,
	DOWN,
	LEFT,
	RIGHT,
	QUICK_CAST,
	MENU_PANE_LEFT,
	MENU_PANE_RIGHT
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
	TAKE_HIT,
	WALLJUMP,
	WALLSLIDE
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
    public enum SupportedLanguages
    {
	EN = 44,
	FR = 82,
	DE = 37,
	ZH = 199,
	ES = 57,
	KO = 117,
	JA = 109,
	IT = 105,
	PT = 147,
	RU = 154
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

    public enum SceneType
    {
	GAMEPLAY,
	MENU,
	LOADING,
	CUTSCENE,
	TEST
    }
}
