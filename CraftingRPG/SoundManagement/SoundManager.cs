using CraftingRPG.Global;
using CraftingRPG.Interfaces;
using CraftingRPG.Timers;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace CraftingRPG.SoundManagement;

public class SoundManager
{
    public static SoundManager Instance = new();

    private Song CurrentSong;
    private bool IsSongFading = false;
    private ITimer FadeTimer;

    public Song GetCurrentSong() => CurrentSong;

    public void PlaySong(Song song, bool loop = true, float volume = 1F)
    {
        MediaPlayer.IsMuted = Flags.DebugMuteMusic;
        
        MediaPlayer.Stop();
        CurrentSong = song;
        MediaPlayer.Play(CurrentSong);
        MediaPlayer.IsRepeating = loop;
        MediaPlayer.Volume = volume;
    }

    public void FadeOut(double duration)
    {
        IsSongFading = true;
        FadeTimer = new LinearTimer(duration);
    }

    public void Update(GameTime gameTime)
    {
        if (!IsSongFading) return;
        
        FadeTimer.Update(gameTime);
        var percent = FadeTimer.GetPercent();
        var volume = CustomMath.Lerp(1, 0, percent);
        MediaPlayer.Volume = (float)volume;

        if (FadeTimer.IsDone())
        {
            IsSongFading = false;
        }
    }
}