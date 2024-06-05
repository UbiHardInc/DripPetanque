using System;
using System.Collections;
using UnityEngine;
using UnityUtility.Singletons;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    public enum SoundGameState
    {
        MainMenu,
        Exploration,
        Petanque,
    }
    
    public enum BallSFXType
    {
        ground,
        rolling,
        ballAir,
        ball,
        swoop
    }

    public enum BattleFilters
    {
        None,
        Low
    }

    public enum BattleVersions
    {
        Intro,
        Soft,
        Intense,
    }

    public SoundLibrary RadioLibrary => m_radioLibrary;
    private RadioManager m_radioManager;

    [Header("Parameters")]
    [SerializeField] private float musicVolume = 0.5f;
    [SerializeField] private float filtersSpeedRate = 0.2f;

    [Header("MusicLibraries")]
    [SerializeField] protected SoundLibrary m_radioLibrary;
    [SerializeField] private SoundLibrary m_battleMusicLibrary;

    [Header("SoundLibraries")]
    [SerializeField] private SoundLibrary m_ballSFXLibrary;
    [SerializeField] private SoundLibrary m_UiSFXLibrary;

    [Header("AudioSources")]
    [SerializeField] private AudioSource m_musicSource1;
    [SerializeField] private AudioSource m_musicSource2;
    [SerializeField] private AudioSource m_SFXSource1;
    [SerializeField] private AudioSource m_SFXSource2;
    [SerializeField] private AudioSource m_SFXLoopSource1;
    [SerializeField] private AudioSource m_SFXLoopSource2;
    [SerializeField] private AudioSource m_ballSfxSource;

    public bool IsMusicWaiting => m_isMusicWaiting;
    protected bool m_isMusicWaiting = false;
    private SoundGameState m_soundGameState;

    //Ball variables
    //Count of every ball sfx to apply random at play
    private int m_ballGroundNumber;
    private int m_ballRollingNumber;
    private int m_ballToBallAirNumber;
    private int m_ballToBallGroundNumber;
    private int m_swoopNumber;
    private bool m_ballStillRolling = false;

    //Battle variables
    private BattleFilters m_actualFilter = BattleFilters.None;
    private BattleVersions m_actualBattleVersion = BattleVersions.Intro;
    private bool m_actualMusicSource = true;
    private bool m_isBattleMusicSwitching = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_radioManager = RadioManager.Instance;
        m_soundGameState = SoundGameState.MainMenu;

        m_musicSource1.volume = musicVolume;
        m_musicSource2.volume = musicVolume;
        InitBallSounds();
    }

    private void Update()
    {
        if (m_soundGameState == SoundGameState.Petanque)
        {
            if (!m_isBattleMusicSwitching)
            {
                if (m_actualMusicSource)
                {
                    if (!m_musicSource1.isPlaying)
                    {
                        RestartActualBattleMusic();
                    }
                }
                else
                {
                    if (!m_musicSource2.isPlaying)
                    {
                        RestartActualBattleMusic();
                    }
                }
            }
        }
    }
    
    #region BallFunctions

    private void InitBallSounds()
    {
        foreach (var soundType in Enum.GetValues(typeof(BallSFXType)))
        {
            int i = 1;
            while (m_ballSFXLibrary.SoundAudioClips.ContainsKey(soundType.ToString() + i.ToString()))
            {
                switch (soundType)
                {
                    case BallSFXType.ground:
                        m_ballGroundNumber++;
                        break;
                    case BallSFXType.rolling:
                        m_ballRollingNumber++;
                        break;
                    case BallSFXType.ballAir:
                        m_ballToBallAirNumber++;
                        break;
                    case BallSFXType.ball:
                        m_ballToBallGroundNumber++;
                        break;
                    case BallSFXType.swoop:
                        m_swoopNumber++;
                        break;
                    default:
                        break;
                }
                i++;
            }
        }
    }

    public void PlayBallSounds(BallSFXType sfxType, bool firstRolling = false)
    {
        _ = StartCoroutine(PlayBallSoundsCoroutine(sfxType, firstRolling));
    }

    private IEnumerator PlayBallSoundsCoroutine(BallSFXType sfxType, bool firstRolling = false)
    {
        AudioClip clip = null;
        int rndClip = 1;
        switch (sfxType)
        {
            case BallSFXType.ground:
                rndClip = Random.Range(1, m_ballGroundNumber + 1);
                _ = m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            case BallSFXType.rolling:
                if (firstRolling)
                {
                    m_ballStillRolling = true;
                }
                rndClip = Random.Range(1, m_ballRollingNumber + 1);
                _ = m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            case BallSFXType.ballAir:
                rndClip = Random.Range(1, m_ballToBallAirNumber + 1);
                _ = m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            case BallSFXType.ball:
                rndClip = Random.Range(1, m_ballToBallGroundNumber + 1);
                _ = m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            case BallSFXType.swoop:
                rndClip = Random.Range(1, m_swoopNumber + 1);
                _ = m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            default:
                break;
        }

        //Debug.LogError("Ball sounds played : " + sfxType.ToString() + rndClip.ToString());

        m_ballSfxSource.PlayOneShot(clip);

        if (sfxType == BallSFXType.ground || sfxType == BallSFXType.ballAir)
        {
            yield return new WaitUntil(() => m_ballSfxSource.isPlaying == false);

            yield return PlayBallSoundsCoroutine(BallSFXType.rolling, true);
        }

        if (sfxType == BallSFXType.rolling)
        {
            yield return new WaitUntil(() => m_ballSfxSource.isPlaying == false);
            if (m_ballStillRolling)
            {
                yield return PlayBallSoundsCoroutine(BallSFXType.rolling);
            }
            else
            {
                yield break;
            }
        }
    }

    public void StopBallRolling()
    {
        //Debug.LogError("Rolling Stopped");
        m_ballStillRolling = false;
        m_ballSfxSource.Stop();
    }

    #endregion

    #region BattleFunctions

    public void InitBattleMusic()
    {
        Debug.LogError("InitBattleMusic");
        m_actualBattleVersion = BattleVersions.Intro;
        m_actualFilter = BattleFilters.None;
        m_isBattleMusicSwitching = false;
        m_actualMusicSource = true;

        StopAllMusicSources();
        PlayMusic(true, "battleIntro");
    }
    
    private void RestartActualBattleMusic()
    {
        m_isBattleMusicSwitching = true;
        Debug.LogError("RestartActualMusic with : " + m_actualFilter.ToString() + " " + m_actualBattleVersion);
        switch (m_actualBattleVersion)
        {
            case BattleVersions.Intro:
                PlayMusic(true, m_actualFilter == BattleFilters.Low ? "battleSoftLow" : "battleSoft");
                m_actualBattleVersion = BattleVersions.Soft;
                break;
            case BattleVersions.Soft:
                PlayMusic(true, m_actualFilter == BattleFilters.Low ? "battleSoftLow" : "battleSoft");
                break;
            case BattleVersions.Intense:
                PlayMusic(true, m_actualFilter == BattleFilters.Low ? "battleIntenseLow" : "battleIntense");
                break;
        }

        m_isBattleMusicSwitching = false;
    }

    public void SwitchBattleMusic(BattleFilters filter, bool isChangingToIntense = false)
    {
        _ = StartCoroutine(SwitchBattleMusicCoroutine(filter, isChangingToIntense));
    }

    private IEnumerator SwitchBattleMusicCoroutine(BattleFilters filter, bool isChangingToIntense)
    {
        Debug.LogError("SwitchBattleMusic to : " + filter.ToString() + " " + m_actualBattleVersion);
        while (m_isBattleMusicSwitching)
        {
            yield return null;
        }

        if (filter != m_actualFilter || isChangingToIntense)
        {
            m_isBattleMusicSwitching = true;
            m_actualFilter = filter;

            AudioSource playingSource = m_musicSource1.isPlaying ? m_musicSource1 : m_musicSource2;
            AudioSource nextSource = m_musicSource1.isPlaying ? m_musicSource2 : m_musicSource1;
            AudioClip nextSourceClip = null;

            switch (m_actualBattleVersion)
            {
                case BattleVersions.Intro:
                    _ = filter == BattleFilters.Low
                        ? m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleIntroLow", out nextSourceClip)
                        : m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleIntro", out nextSourceClip);
                    break;
                case BattleVersions.Soft:
                    _ = filter == BattleFilters.Low
                        ? m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleSoftLow", out nextSourceClip)
                        : m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleSoft", out nextSourceClip);
                    break;
                case BattleVersions.Intense:
                    _ = filter == BattleFilters.Low
                        ? m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleIntenseLow", out nextSourceClip)
                        : m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleIntense", out nextSourceClip);
                    break;
            }

            nextSource.clip = nextSourceClip;
            nextSource.volume = 0.0f;
            nextSource.time = playingSource.time;
            nextSource.Play();

            while (nextSource.volume < musicVolume)
            {
                nextSource.volume += filtersSpeedRate * Time.deltaTime;
                playingSource.volume -= filtersSpeedRate * Time.deltaTime;

                yield return null;
            }

            m_actualMusicSource = !m_actualMusicSource;
            playingSource.Stop();
            m_isBattleMusicSwitching = false;
        }
    }

    public void SwitchIntenseBattleMusic()
    {
        m_actualBattleVersion = BattleVersions.Intense;
        SwitchBattleMusic(m_actualFilter, true);
    }

    #endregion

    #region PlayFunctions

    public void PlayMusic(bool isBattle, string soundName)
    {
        SoundLibrary selectedLibrary = isBattle ? m_battleMusicLibrary : m_radioLibrary;

        _ = selectedLibrary.SoundAudioClips.TryGetValue(soundName, out AudioClip clip);
        
        m_musicSource1.clip = clip;
        m_actualMusicSource = true;
        m_musicSource2.Stop();
        m_musicSource1.time = 0f;
        m_musicSource1.Play();
        
    }

    public void PlayMusicAndWaitEnd(bool isBattle, string soundName)
    {
        _ = StartCoroutine(PlayMusicAndWaitEndCoroutine(isBattle, soundName));
    }

    public IEnumerator PlayMusicAndWaitEndCoroutine(bool isBattle, string soundName)
    {
        if (!m_isMusicWaiting)
        {
            SoundLibrary selectedLibrary = isBattle ? m_battleMusicLibrary : m_radioLibrary;

            _ = selectedLibrary.SoundAudioClips.TryGetValue(soundName, out AudioClip clip);

            m_isMusicWaiting = true;
            yield return new WaitUntil(() => m_musicSource1.isPlaying == false);
            m_isMusicWaiting = false;

            m_musicSource1.clip = clip;
            m_musicSource1.Play();
            if (soundName.StartsWith("music"))
            {
                _ = StartCoroutine(m_radioManager.ShowMusicDataInUI());
            }
        }
        else
        {
            Debug.LogError("There is already a music waiting to be played.");
        }

    }

    public void PlayUISFX(string soundName, bool isLooping = false)
    {
        if (m_UiSFXLibrary.SoundAudioClips.ContainsKey(soundName))
        {
            _ = m_UiSFXLibrary.SoundAudioClips.TryGetValue(soundName, out AudioClip clip);
            if (isLooping)
            {
                if (m_SFXLoopSource1.isPlaying)
                {
                    m_SFXLoopSource2.PlayOneShot(clip);
                    return;
                }
                m_SFXLoopSource1.PlayOneShot(clip);
            }
            else
            {
                if (m_SFXSource1.isPlaying)
                {
                    m_SFXSource2.PlayOneShot(clip);
                    return;
                }
                m_SFXSource1.PlayOneShot(clip);
            }
        }
        else
        {
            Debug.LogError("This sound isn't in this library.");
        }
    }

    public void StopAllAudioSources()
    {
        m_musicSource1.Stop();
        m_musicSource2.Stop();
        m_SFXSource1.Stop();
        m_SFXSource2.Stop();
        m_SFXLoopSource1.Stop();
        m_SFXLoopSource2.Stop();
        m_ballSfxSource.Stop();
    }

    public void StopAllMusicSources()
    {
        m_musicSource1.Stop();
        m_musicSource2.Stop();
    }

    public void StopAllSfxSources()
    {
        m_SFXSource1.Stop();
        m_SFXSource2.Stop();
        m_SFXLoopSource1.Stop();
        m_SFXLoopSource2.Stop();
        m_ballSfxSource.Stop();
    }

    public void StopAllSfxLoopSources()
    {
        m_SFXLoopSource1.Stop();
        m_SFXLoopSource2.Stop();
    }

    #endregion

    public void UpdateState(GameState nextState)
    {
        switch (nextState)
        {
            case GameState.None:
                //Technically it's main menu so it is on it's own
                break;
            case GameState.Exploration:
                RadioManager.Instance.StartRadio();
                break;
            case GameState.Dialogue:
                //do nothing
                break;
            case GameState.Petanque:
                m_soundGameState = SoundGameState.Petanque;
                InitBattleMusic();
                break;
            case GameState.Cinematics:
                //Do nothing
                break;
        }
    }
}