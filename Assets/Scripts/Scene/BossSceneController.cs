using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class BossSceneController : MonoBehaviour
{
    public static BossSceneController Instance;
    public delegate void SetupEventDelegate(BossSceneController self);
    public static SetupEventDelegate SetupEvent;
    public Transform heroSpawn;
    public GameObject transitionPrefab;
    public EventRegister endTransitionEvent;
    public bool doTransitionIn = true;
    public float transitionInHoldTime;
    public bool doTransitionOut = true;
    public float transitionOutHoldTime;
    private bool isTransitioningOut;
    private bool canTransition = true;
    [Space]
    [Tooltip("If scene end is handled elsewhere then leave empty. Only assign bosses here if you want the scene to end on HealthManager death event.")]
    public HealthManager[] bosses;
    private int bossesLeft;
    public float bossesDeadWaitTime = 5f;
    private int bossLevel;
    private bool endedScene;
    private bool knightDamagedSubscribed;
    private bool restoreBindingsOnDestroy = true;
    public TransitionPoint customExitPoint;
    private bool doTransition = true;
}
