using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility.Singletons;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
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
        Low,
        High
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
    
    // Start is called before the first frame update
    void Start()
    {
        m_radioManager = RadioManager.Instance;
        
        m_musicSource1.volume = musicVolume;
        m_musicSource2.volume = musicVolume;
        InitBallSounds();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                }
                i++;
            }
        }
    }
    
    public IEnumerator PlayBallSounds(BallSFXType sfxType, bool firstRolling = false)
    {
        AudioClip clip = null;
        int rndClip = 1;
        switch (sfxType)
        {
            case BallSFXType.ground:
                rndClip = Random.Range(1, m_ballGroundNumber + 1);
                m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            case BallSFXType.rolling:
                if (firstRolling)
                {
                    m_ballStillRolling = true;
                }
                rndClip = Random.Range(1, m_ballRollingNumber + 1);
                m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            case BallSFXType.ballAir:
                rndClip = Random.Range(1, m_ballToBallAirNumber + 1);
                m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            case BallSFXType.ball:
                rndClip = Random.Range(1, m_ballToBallGroundNumber + 1);
                m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
            case BallSFXType.swoop:
                rndClip = Random.Range(1, m_swoopNumber + 1);
                m_ballSFXLibrary.SoundAudioClips.TryGetValue(sfxType.ToString() + rndClip.ToString(), out clip);
                break;
        }
        
        //Debug.LogError("Ball sounds played : " + sfxType.ToString() + rndClip.ToString());
        
        m_ballSfxSource.PlayOneShot(clip);

        if (sfxType == BallSFXType.ground || sfxType == BallSFXType.ballAir)
        {
            yield return new WaitUntil(() => m_ballSfxSource.isPlaying == false);
            
            yield return PlayBallSounds(BallSFXType.rolling, true);
        }

        if (sfxType == BallSFXType.rolling)
        {
            yield return new WaitUntil(() => m_ballSfxSource.isPlaying == false);
            if (m_ballStillRolling)
            {
                yield return PlayBallSounds(BallSFXType.rolling);
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

    public IEnumerator SwitchBattleMusic(BattleFilters filter)
    {
        if (filter != m_actualFilter)
        {
            //Debug.LogError("Battle filter switch called.");
            
            m_actualFilter = filter;
        
            AudioSource playingSource = m_musicSource1.isPlaying ? m_musicSource1 : m_musicSource2;
            AudioSource nextSource = m_musicSource1.isPlaying ? m_musicSource2 : m_musicSource1;
            AudioClip nextSourceClip = null;

            switch (filter)
            {
                case BattleFilters.High:
                    m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleFullHigh", out nextSourceClip);
                    break;
                case BattleFilters.Low:
                    m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleFullLow", out nextSourceClip);
                    break;
                case BattleFilters.None:
                    m_battleMusicLibrary.SoundAudioClips.TryGetValue("battleFull", out nextSourceClip);
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
        
            playingSource.Stop();
        }
    }

    #endregion

    #region PlayFunctions

    public void PlayMusic(bool isBattle, string soundName)
    {
        SoundLibrary selectedLibrary = isBattle ? m_battleMusicLibrary : m_radioLibrary;
        
        AudioClip clip = null;
        selectedLibrary.SoundAudioClips.TryGetValue(soundName, out clip);

        m_musicSource1.clip = clip;
        m_musicSource1.Play(); 
        
    }

    public IEnumerator PlayMusicAndWaitEnd(bool isBattle, string soundName)
    {
        if (!m_isMusicWaiting)
        {
            SoundLibrary selectedLibrary = isBattle ? m_battleMusicLibrary : m_radioLibrary;
        
            AudioClip clip = null;
            selectedLibrary.SoundAudioClips.TryGetValue(soundName, out clip);
            
            m_isMusicWaiting = true;
            yield return new WaitUntil(() => m_musicSource1.isPlaying == false);
            m_isMusicWaiting = false;

            m_musicSource1.clip = clip;
            m_musicSource1.Play();
            if (soundName.StartsWith("music"))
            {
                StartCoroutine(m_radioManager.ShowMusicDataInUI());
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
            AudioClip clip = null;
            m_UiSFXLibrary.SoundAudioClips.TryGetValue(soundName, out clip);
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
}