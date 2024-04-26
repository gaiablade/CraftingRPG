using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.SpriteAnimation;

public class Animation
{
    private int UniqueAnimationFrames { get; set; }
    private double AnimationFrameDuration { get; set; }
    private Point Size { get; set; }
    private bool IsLooping { get; set; }
    
    private int CurrentAnimationFrame;
    private readonly Point StartingPosition;
    private double FrameTimer;
    private bool IsOver = false;
    
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

    public void Reset()
    {
        CurrentAnimationFrame = 0;
        FrameTimer = 0;
        IsOver = false;
    }

    public void Update(GameTime gameTime)
    {
        FrameTimer += gameTime.ElapsedGameTime.TotalSeconds;

        if (!(FrameTimer >= AnimationFrameDuration)) return;
        
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

    public Rectangle GetSourceRectangle()
    {
        var position = StartingPosition + new Point(CurrentAnimationFrame * Size.X, 0);
        return new Rectangle(position, Size);
    }

    public bool IsAnimationOver()
    {
        return IsOver;
    }

    public int GetCurrentFrame() => CurrentAnimationFrame;
}