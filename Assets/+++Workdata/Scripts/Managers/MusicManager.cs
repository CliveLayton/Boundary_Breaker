using UnityEngine;

public class MusicManager : MonoBehaviour
{
    #region Variables

    public static MusicManager Instance { get; private set; }

    [Header("Audio Sources")] 
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private AudioSource uiSFXAudio;
    [SerializeField] private AudioSource inGameSFXAudio;

    [Header("Music")] 
    public AudioClip mainMenuMusic;

    [Header("VoiceLines")]

    [Header("Player Footsteps")]

    [Header("InGame SFX")]

    [Header("UI SFX")] 
    public AudioClip buttonHover;
    public AudioClip buttonPress;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Music Methods

    /// <summary>
    /// stops current music and plays the new clip
    /// </summary>
    /// <param name="clip">audio clip to play</param>
    public void PlayMusic(AudioClip clip, float fadeDuration)
    {
        musicAudio.FadingInOut(clip, fadeDuration);
    }

    /// <summary>
    /// plays ui sfx audio clip one time
    /// </summary>
    /// <param name="clip">audio clip to play</param>
    public void PlayUISFX(AudioClip clip)
    {
        uiSFXAudio.PlayOneShot(clip);
    }

    /// <summary>
    /// plays in game sfx audio clip one time
    /// </summary>
    /// <param name="clip">audio clip to play</param>
    public void PlayInGameSFX(AudioClip clip)
    {
        inGameSFXAudio.PlayOneShot(clip);
    }

    public void StopInGameSFX()
    {
        inGameSFXAudio.Stop();
    }

    #endregion
}
