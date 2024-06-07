using System;
using System.Collections;
using UnityEngine;
using UnityUtility.CustomAttributes;
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
    public bool IsMusicWaiting => m_isMusicWaiting;

    [Title("Parameters")]
    [SerializeField] private float m_musicVolume = 0.5f;
    [SerializeField] private float m_filtersSpeedRate = 0.2f;
    [SerializeField] private float m_cityAmbianceVolume = 0.4f;

    [Title("MusicLibraries")]
    [SerializeField] protected SoundLibrary m_radioLibrary;
    [SerializeField] private SoundLibrary m_battleMusicLibrary;

    [Title("SoundLibraries")]
    [SerializeField] private SoundLibrary m_ballSFXLibrary;
    [SerializeField] private SoundLibrary m_UiSFXLibrary;

    [Title("AudioSources")]
    [SerializeField] private AudioSource m_musicSource1;
    [SerializeField] private AudioSource m_musicSource2;
    [SerializeField] private AudioSource m_SFXSource1;
    [SerializeField] private AudioSource m_SFXSource2;
    [SerializeField] private AudioSource m_SFXLoopSource1;
    [SerializeField] private AudioSource m_SFXLoopSource2;
    [SerializeField] private AudioSource m_ballSfxSource;
    [SerializeField] private AudioSource m_cityAmbianceSource;

    private RadioManager m_radioManager;

    private bool m_isMusicWaiting = false;
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
    private BattleFilters m_oldActualFilter = BattleFilters.None;
    private BattleVersions m_oldActualBattleVersion = BattleVersions.Intro;
    private bool m_actualMusicSource = true;
    private bool m_isBattleMusicSwitching = false;
    
    //Exploration
    private bool m_isInCityAmbiance = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_radioManager = RadioManager.Instance;
        UpdateState(GameManager.Instance.CurrentSubGameManager.CorrespondingState);
        GameManager.Instance.OnGameStateEntered += UpdateState;
        GameManager.Instance.OnGameStateExited += UpdateState;

        m_musicSource1.volume = m_musicVolume;
        m_musicSource2.volume = m_musicVolume;
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

            if (m_actualFilter != m_oldActualFilter)
            {
                SwitchBattleMusic();
                m_oldActualFilter = m_actualFilter;
            }

            if (m_actualBattleVersion != m_oldActualBattleVersion)
            {
                SwitchBattleMusic();
                m_oldActualBattleVersion = m_actualBattleVersion;
            }
        }

        if (m_soundGameState == SoundGameState.Exploration && !m_isInCityAmbiance)
        {
            PlayCitySound();
            m_isInCityAmbiance = true;
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

        if (sfxType is BallSFXType.ground or BallSFXType.ballAir)
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
        //Debug.LogError("InitBattleMusic");
        m_actualBattleVersion = BattleVersions.Intro;
        m_actualFilter = BattleFilters.None;
        m_isBattleMusicSwitching = false;
        m_actualMusicSource = true;

        m_radioManager.HideRadio();
        StopAllMusicSources();
        PlayMusic(true, "battleIntro");
    }

    private void RestartActualBattleMusic()
    {
        m_isBattleMusicSwitching = true;
        StopCoroutine(SwitchBattleMusicCoroutine());
        //Debug.LogError("RestartActualMusic with : " + m_actualFilter.ToString() + " " + m_actualBattleVersion);
        if (m_actualBattleVersion == BattleVersions.Intro)
        {
            m_actualBattleVersion = BattleVersions.Soft;
        }
        switch (m_actualBattleVersion)
        {
            case BattleVersions.Soft:
                PlayMusic(true, m_actualFilter == BattleFilters.Low ? "battleSoftLow" : "battleSoft");
                break;
            case BattleVersions.Intense:
                PlayMusic(true, m_actualFilter == BattleFilters.Low ? "battleIntenseLow" : "battleIntense");
                break;
            case BattleVersions.Intro:
                break;
            default:
                break;
        }

        m_isBattleMusicSwitching = false;
    }

    public void SwitchBattleMusic()
    {
        _ = StartCoroutine(SwitchBattleMusicCoroutine());
    }

    private IEnumerator SwitchBattleMusicCoroutine()
    {
        //Debug.LogError("SwitchBattleMusic to : " + m_actualFilter.ToString() + " " + m_actualBattleVersion);
        while (m_isBattleMusicSwitching)
        {
            yield return null;
        }

        m_isBattleMusicSwitching = true;

        AudioSource playingSource = m_musicSource1.isPlaying ? m_musicSource1 : m_musicSource2;
        AudioSource nextSource = m_musicSource1.isPlaying ? m_musicSource2 : m_musicSource1;
        AudioClip nextSourceClip = null;

        switch (m_actualBattleVersion)
        {
            case BattleVersions.Intro:
                _ = m_actualFilter == BattleFilters.Low
                    ? m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleIntroLow", out nextSourceClip)
                    : m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleIntro", out nextSourceClip);
                break;
            case BattleVersions.Soft:
                _ = m_actualFilter == BattleFilters.Low
                    ? m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleSoftLow", out nextSourceClip)
                    : m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleSoft", out nextSourceClip);
                break;
            case BattleVersions.Intense:
                _ = m_actualFilter == BattleFilters.Low
                    ? m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleIntenseLow", out nextSourceClip)
                    : m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleIntense", out nextSourceClip);
                break;
            default:
                break;
        }

        nextSource.clip = nextSourceClip;
        nextSource.volume = 0.0f;
        nextSource.Play();
        nextSource.time = playingSource.time;
        //Debug.LogError("Times when switching : next : " + nextSource.time + " playing : " + playingSource.time);

        while (nextSource.volume < m_musicVolume)
        {
            nextSource.volume += m_filtersSpeedRate * Time.deltaTime;
            playingSource.volume -= m_filtersSpeedRate * Time.deltaTime;

            yield return null;
        }

        m_actualMusicSource = !m_actualMusicSource;
        playingSource.Stop();
        m_isBattleMusicSwitching = false;
    }

    public void SwitchBattleFilterMusic(BattleFilters filter)
    {
        m_actualFilter = filter;
    }

    public void SwitchIntenseBattleMusic()
    {
        m_actualBattleVersion = BattleVersions.Intense;
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
        m_musicSource1.Stop();
        m_musicSource1.volume = m_musicVolume;
        m_musicSource1.time = 0.0f;
        m_musicSource1.Play();
        //Debug.LogError("Play time : " + m_musicSource1.time);

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

            if (m_soundGameState == SoundGameState.Exploration)
            {
                m_actualMusicSource = true;
                m_musicSource2.Stop();
                m_musicSource1.clip = clip;
                m_musicSource1.Play();
                if (soundName.StartsWith("music"))
                {
                    _ = StartCoroutine(m_radioManager.ShowMusicDataInUI());
                }
            }
            else
            {
                yield break;
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
    
    private void PlayCitySound()
    {
        m_UiSFXLibrary.SoundAudioClips.TryGetValue("city", out AudioClip clip);
        m_cityAmbianceSource.clip = clip;
        m_cityAmbianceSource.loop = true;
        m_cityAmbianceSource.volume = m_cityAmbianceVolume;
        m_cityAmbianceSource.Play();
        m_cityAmbianceSource.time = Random.Range(0.0f, clip.length);
    }

    private void StopCitySound()
    {
        m_cityAmbianceSource.Stop();
        m_isInCityAmbiance = false;
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
        m_cityAmbianceSource.Stop();
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
        m_cityAmbianceSource.Stop();
    }

    public void StopAllSfxLoopSources()
    {
        m_SFXLoopSource1.Stop();
        m_SFXLoopSource2.Stop();
    }

    #endregion

    public void UpdateState(GameState nextState)
    {
        //Debug.LogError("Sound game state updated with : " + nextState.ToString());
        switch (nextState)
        {
            case GameState.None:
                //Technically it's main menu so it is on it's own
                m_soundGameState = SoundGameState.MainMenu;
                StopCitySound();
                StopAllMusicSources();
                m_radioManager.HideRadio();
                break;
            case GameState.Exploration:
                m_soundGameState = SoundGameState.Exploration;
                break;
            case GameState.Dialogue:
                //do nothing
                break;
            case GameState.Petanque:
                m_soundGameState = SoundGameState.Petanque;
                StopCitySound();
                InitBattleMusic();
                break;
            case GameState.Cinematics:
                //Do nothing
                break;
            default:
                break;
        }
    }
}