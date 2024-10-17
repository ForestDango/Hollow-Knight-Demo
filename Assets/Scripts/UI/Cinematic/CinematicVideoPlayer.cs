using System;

public abstract class CinematicVideoPlayer : IDisposable
{
    protected CinematicVideoPlayerConfig Config
    {
	get
	{
	    return config;
	}
    }

    public CinematicVideoPlayer(CinematicVideoPlayerConfig config)
    {
	this.config = config;
    }

    public virtual void Dispose()
    {
    }

    public abstract bool IsLoading { get; }
    public abstract bool IsPlaying { get; }
    public abstract bool IsLooping { get; set; }
    public abstract float Volume { get; set; }
    public abstract void Play();
    public abstract void Stop();
    public virtual float CurrentTime
    {
	get
	{
	    return 0f;
	}
    }
    public virtual void Update()
    {
    }

    public static CinematicVideoPlayer Create(CinematicVideoPlayerConfig config)
    {
	return new XB1CinematicVideoPlayer(config);
    }

    private CinematicVideoPlayerConfig config;
}
