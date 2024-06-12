using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DialogueData), menuName = "Scriptables/Dialogues/" + nameof(DialogueData))]
public class DialogueData : ScriptableObject
{
    public enum SentenceDisplayStyle { Direct, Type } // sentences display style enum
    public SentenceDisplayStyle sentenceDisplayStyle = new SentenceDisplayStyle(); // sentences display style
    public float transitionStartTime;
    public float transitionEndTime;
    public float typingDelay = 0.02f; // "Type" style delay (between each typed letters)
    [NonReorderable]
    public List<SentenceData> sentenceDatas = new List<SentenceData>();
    public int nbSentences;
    public bool showDialogueElements;
    public enum TextType { Text, Dialogue }
    public TextType textType;
    public bool IsCinematicDialogue;

    public NextStateDatas NextStateDatas;
}

[System.Serializable]
public class SentenceData
{
    public bool isSkippable = true; // Can player skip this sentence
    public string NPCName;
    public Sprite NPCSprite;
    [TextArea(2, 10)]
    public string sentence;
    [HideInInspector]
    public bool showData; // Level editor reference only !
    public AudioClipData audioClipData = new AudioClipData();
    public bool showSfxElements;
    public bool withSFX;
    public bool typingIsComplete = true; //for sentence in "Type" style
    public bool withCustomEvent;
    public enum StartOfEvent { BeforeSentenceTyping, DuringSentenceTyping, AfterSentenceTyping }
    public StartOfEvent startOfEvent;
    public DialogueIDWraper eventIDToLaunch;
}

[System.Serializable]
public class AudioClipData
{
    public AudioClip audioClip;
    //public enum AudioType { music, sfx } // ambient audio (only one instance on scene) or sound effect
    //public AudioType audioType = new AudioType();
    public bool audioLoop; // loop, t/f
    public enum StartType { Immediate, FadeIn } // instant start y/n
    public StartType startType = new StartType();
    public float fadeInTime = 0; // fadein duration
    public float delay = 0; // delay before start playing
    public enum StopType { Immediate, FadeOut } // instant stop y/n
    public StopType stopType = new StopType();
    public float fadeOutTime = 0; // fadeout duration
    public float maxVolume = 100; // audio volume (oh, really?)
    public bool showaudioClipElements;

}
