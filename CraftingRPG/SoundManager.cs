using CraftingRPG.Global;
using Microsoft.Xna.Framework.Media;

namespace CraftingRPG;

public class SoundManager
{
    public static SoundManager Instance = new();

    private Song CurrentSong;

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
}