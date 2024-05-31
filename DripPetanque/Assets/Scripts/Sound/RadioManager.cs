using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityUtility.Singletons;
using Random = System.Random;

public class RadioManager : MonoBehaviourSingleton<RadioManager>
{
    [Header("ManagerReferences")] 
    [SerializeField] private SoundManager m_soundManager;

    [Header("RadioGameObjects")] 
    [SerializeField] private TMP_Text artistNameText;
    [SerializeField] private TMP_Text songNameText;

    [Header("Parameters")] 
    [SerializeField] private float timeRadioUiStay = 2f;

    public enum RadioClipType
    {
        music,
        interlude
    }

    private string m_musicTitle;
    private string m_musicArtist;
    private List<string> m_musicNameList = new List<string>();
    private List<string> m_interludeNameList = new List<string>();
    private List<string> m_masterPlaylist = new List<string>();
    private int m_playlistPlayNumber;

    private void Start()
    {
        m_soundManager = SoundManager.Instance;
        
        if (GameManager.Instance.CurrentSubGameManager.CorrespondingState == GameState.Exploration)
        {
            m_soundManager.StopAllMusicSources();
            StartRadio();
        }
    }
    
    private void Update()
    {
        if (m_masterPlaylist.Count < m_playlistPlayNumber + 1)
        {
            ResetMasterPlaylist();
        }
        
        if (!m_soundManager.IsMusicWaiting)
        {
            AddAudioToWait();
        }
    }

    private void StartRadio()
    {
        ResetMasterPlaylist();
        AddAudioToWait();
    }

    private void AddAudioToWait()
    {
        StartCoroutine(m_soundManager.PlayMusicAndWaitEnd(false, m_masterPlaylist[m_playlistPlayNumber]));
        m_playlistPlayNumber++;
    }

    private void ResetMasterPlaylist()
    {
        foreach (var nameAudioClip in m_soundManager.RadioLibrary.SoundAudioClips)
        {
            if (nameAudioClip.Key.StartsWith(RadioClipType.music.ToString()))
            {
                m_musicNameList.Add(nameAudioClip.Key);
            } else if (nameAudioClip.Key.StartsWith(RadioClipType.interlude.ToString()))
            {
                m_interludeNameList.Add(nameAudioClip.Key);
            }
        }
        
        Random rng = new Random();
        m_musicNameList.OrderBy(_ => rng.Next()).ToList();
        m_interludeNameList.OrderBy(_ => rng.Next()).ToList();

        foreach (var interlude in m_interludeNameList)
        {
            m_masterPlaylist.Add(interlude);

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
        artistNameText.text = m_masterPlaylist[m_playlistPlayNumber - 1].Split("-")[1];
        songNameText.text = m_masterPlaylist[m_playlistPlayNumber - 1].Split("-")[2];
        
        //Call animation to show the radio UI
        yield return new WaitForSeconds(timeRadioUiStay);
        //Call animation ti hide the radio UI
    }
}
