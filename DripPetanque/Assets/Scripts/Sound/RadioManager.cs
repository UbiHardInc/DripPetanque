using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityUtility.Singletons;
using UnityUtility.Utils;
using Random = System.Random;

public class RadioManager : MonoBehaviourSingleton<RadioManager>
{
    public enum RadioClipType
    {
        music,
        interlude
    }

    private SoundManager m_soundManager;

    [Header("RadioGameObjects")]
    [SerializeField] private TMP_Text m_artistNameText;
    [SerializeField] private TMP_Text m_songNameText;
    [SerializeField] private RectTransform m_radioUI;

    [Header("Parameters")]
    [SerializeField] private float m_timeRadioUiStay = 2f;

    private readonly string m_musicTitle;
    private readonly string m_musicArtist;
    private readonly List<string> m_musicNameList = new List<string>();
    private readonly List<string> m_interludeNameList = new List<string>();
    private readonly List<string> m_masterPlaylist = new List<string>();
    private int m_playlistPlayNumber;

    private bool m_radioStarted;
    [SerializeField]private bool m_firstTimePlaying = true;

    protected override void Start()
    {
        base.Start();
        m_soundManager = SoundManager.Instance;
    }

    private void Update()
    {
        if (m_masterPlaylist.Count < m_playlistPlayNumber + 1 && m_radioStarted)
        {
            ResetMasterPlaylist();
        }

        if (!m_soundManager.IsMusicWaiting && m_radioStarted)
        {
            AddAudioToWait();
        }

        if (GameManager.Instance.CurrentSubGameManager.CorrespondingState == GameState.Exploration && !m_radioStarted)
        {
            StartRadio();
        }
    }

    public void StartRadio()
    {
        m_radioStarted = true;
        m_radioUI.gameObject.SetActive(true);
        ResetMasterPlaylist();
        m_soundManager.StopAllMusicSources();
        AddAudioToWait();
    }

    private void AddAudioToWait()
    {
        m_soundManager.PlayMusicAndWaitEnd(false, m_masterPlaylist[m_playlistPlayNumber]);
        m_playlistPlayNumber++;
    }

    private void ResetMasterPlaylist()
    {
        m_masterPlaylist.Clear();
        
        foreach (var nameAudioClip in m_soundManager.RadioLibrary.SoundAudioClips)
        {
            if (nameAudioClip.Key.StartsWith(RadioClipType.music.ToString()))
            {
                m_musicNameList.Add(nameAudioClip.Key);
            }
            else if (nameAudioClip.Key.StartsWith(RadioClipType.interlude.ToString()))
            {
                if (!m_firstTimePlaying && nameAudioClip.Key == "interlude1")
                {
                    //Do Nothing
                }
                else
                {
                    m_interludeNameList.Add(nameAudioClip.Key);
                }
                
            }
        }

        m_musicNameList.Shuffle();
        m_interludeNameList.Shuffle();

        if (m_firstTimePlaying)
        {
            m_masterPlaylist.Add("interlude1");
            m_interludeNameList.Remove("interlude1");
        }

        foreach (var interlude in m_interludeNameList)
        {
            if (m_firstTimePlaying)
            {
                m_firstTimePlaying = false;
            }
            else
            {
                m_masterPlaylist.Add(interlude);
            }
            

            if (m_musicNameList.Count >= 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    m_masterPlaylist.Add(m_musicNameList[0]);
                    m_musicNameList.RemoveAt(0);
                }
            }
            else if (m_musicNameList.Count == 1)
            {
                m_masterPlaylist.Add(m_musicNameList[0]);
                m_musicNameList.RemoveAt(0);
            }
            else
            {
                break;
            }
        }

        m_playlistPlayNumber = 0;
    }

    public IEnumerator ShowMusicDataInUI()
    {
        Debug.LogError("ShowRadioUI Called");
        m_artistNameText.text = m_masterPlaylist[m_playlistPlayNumber - 1].Split("-")[1].Replace("/", " ");
        m_songNameText.text = m_masterPlaylist[m_playlistPlayNumber - 1].Split("-")[2].Replace("/", " ");

        //Todo :
        //Call animation to show the radio UI
        m_radioUI.DOLocalMove(m_radioUI.localPosition + new Vector3(-529.9f,0f,0f), 2f);
        yield return new WaitForSeconds(m_timeRadioUiStay);
        //Call animation ti hide the radio UI
        m_radioUI.DOLocalMove(m_radioUI.localPosition + new Vector3(529.9f, 0f, 0f), 2f);
    }

    public void HideRadio()
    {
        m_radioUI.gameObject.SetActive(false);
        m_radioStarted = false;
    }
}
