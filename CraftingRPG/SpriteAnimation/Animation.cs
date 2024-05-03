using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.SpriteAnimation;

public class Animation : IAnimation
{
    protected int UniqueAnimationFrames { get; set; }
    protected double AnimationFrameDuration { get; set; }
    protected Point Size { get; set; }
    protected bool IsLooping { get; set; }
    
    protected int CurrentAnimationFrame;
    protected readonly Point StartingPosition;
    protected double FrameTimer;
    protected bool IsOver;
    
    public Animation(int frames, double interval, Point size, bool loop = true, int x = 0, int y = 0)
    {
        UniqueAnimationFrames = frames;
        AnimationFrameDuration = interval;
        Size = size;
        IsLooping = loop;
        
        CurrentAnimationFrame = 0;
        StartingPosition = new Point(x, y);
        FrameTimer = 0;
    }

    public virtual void Reset()
    {
        CurrentAnimationFrame = 0;
        FrameTimer = 0;
        IsOver = false;
    }

    public virtual void Update(GameTime gameTime)
    {
        FrameTimer += gameTime.ElapsedGameTime.TotalSeconds;

        if (!IsCurrentFrameOver()) return;
        
        FrameTimer = 0;
        CurrentAnimationFrame++;
            
        if (CurrentAnimationFrame < UniqueAnimationFrames) return;
            
        if (IsLooping)
        {
            CurrentAnimationFrame = 0;
        }
        else
        {
            CurrentAnimationFrame--;
            IsOver = true;
        }
    }

    public virtual Rectangle GetSourceRectangle()
    {
        var position = StartingPosition + new Point(CurrentAnimationFrame * Size.X, 0);
        return new Rectangle(position, Size);
    }

    public virtual bool IsAnimationOver()
    {
        return IsOver;
    }

    public virtual int GetCurrentFrame() => CurrentAnimationFrame;

    public virtual double GetDuration() => UniqueAnimationFrames * AnimationFrameDuration;

    protected virtual bool IsCurrentFrameOver() => FrameTimer >= AnimationFrameDuration;
}