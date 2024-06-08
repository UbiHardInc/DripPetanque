using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityUtility.CustomAttributes;
using UnityUtility.Singletons;
using UnityUtility.Utils;

public class RadioManager : MonoBehaviour
{
    public enum RadioClipType
    {
        music,
        interlude
    }

    [Title("RadioGameObjects")]
    [SerializeField] private TMP_Text m_artistNameText;
    [SerializeField] private TMP_Text m_songNameText;
    [SerializeField] private RectTransform m_radioUI;

    [Title("Parameters")]
    [SerializeField] private float m_timeRadioUiStay = 2f;

    [Title("Misc")]
    [SerializeField] private bool m_firstTimePlaying = true;

    [NonSerialized] private readonly string m_musicTitle;
    [NonSerialized] private readonly string m_musicArtist;
    [NonSerialized] private readonly List<string> m_musicNameList = new List<string>();
    [NonSerialized] private readonly List<string> m_interludeNameList = new List<string>();
    [NonSerialized] private readonly List<string> m_masterPlaylist = new List<string>();
    [NonSerialized] private int m_playlistPlayNumber;

    [NonSerialized] private bool m_radioStarted;

    [NonSerialized] private SoundManager m_soundManager;

    protected void Start()
    {
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
            _ = m_interludeNameList.Remove("interlude1");
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
        //Debug.LogError("ShowRadioUI Called");
        string[] splittedMusicName = m_masterPlaylist[m_playlistPlayNumber - 1].Split("-");
        m_artistNameText.text = splittedMusicName[1].Replace("/", " ");
        m_songNameText.text = splittedMusicName[2].Replace("/", " ");

        Vector3 uiDisplacement = new Vector3(-529.9f, 0f, 0f); // @TODO : If possible, get rid of this hard-coded value

        //Animation to show the radio UI
        _ = m_radioUI.DOLocalMove(m_radioUI.localPosition + uiDisplacement, 2f);
        yield return new WaitForSeconds(m_timeRadioUiStay);
        //Animation to hide the radio UI
        _ = m_radioUI.DOLocalMove(m_radioUI.localPosition - uiDisplacement, 2f);
    }

    public void HideRadio()
    {
        m_radioUI.gameObject.SetActive(false);
        m_radioStarted = false;
    }
}
