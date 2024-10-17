using System;
using UnityEngine;

public abstract class SkippableSequence : MonoBehaviour
{
    public abstract void Begin();
    public abstract bool IsPlaying { get; }
    public abstract void Skip();
    public abstract bool IsSkipped { get; }
    public abstract float FadeByController { get; set; }
}
