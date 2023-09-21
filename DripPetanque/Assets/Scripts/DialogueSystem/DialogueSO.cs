using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "SO/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public DialogueData dialogueData;
}

[System.Serializable]
public class DialogueData
{
    public enum DialogueStartDisplayStyle { direct, fade, zoomIn, translation } //  dialogue display appears style enum
    public DialogueStartDisplayStyle dialogueStartDisplayStyle = new DialogueStartDisplayStyle(); // dialogue display appears style 
    public enum DialogueEndDisplayStyle { direct, fade, zoomOut, translation } // dialogue display disappears style enum
    public DialogueEndDisplayStyle dialogueEndDisplayStyle = new DialogueEndDisplayStyle(); // dialogue display disappears style 
    public enum SwipInDirection { fromDown, fromLeft, fromRight }  //  direction on arriving translation transition enum
    public SwipInDirection swipInDirection = new SwipInDirection(); //  direction  on arriving translation transition
    public enum SwipOutDirection { toDown, toLeft, toRight }  //  direction on arriving translation transition enum
    public SwipOutDirection swipOutDirection = new SwipOutDirection(); //  direction  on arriving translation transition
    public enum SentenceDisplayStyle { direct, type } // sentences display style enum
    public SentenceDisplayStyle sentenceDisplayStyle = new SentenceDisplayStyle(); // sentences display style
    public float transitionStartTime;
    public float transitionEndTime;
    public float typingDelay = 0.02f; // "Type" style delay (between each typed letters)
    [NonReorderable]
    public List<SentenceData> sentenceDatas = new List<SentenceData>();
    public int nbSentences;
    public bool withMusic;
    public enum TextType { text, dialogue }
    public TextType textType;
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
    public bool showSfxElements;
    public bool withSFX;
    public bool typingIsComplete = true; //for sentence in "Type" style
}
