using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueData))]
public class DialogueDataEditor : CustomEditorBase
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EditorGUI.BeginChangeCheck();
        ShowTransitionStartTime();
        ShowTransitionEndTime();
        ShowSentenceDisplayStyle();
        
        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(serializedObject.targetObject);
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowTransitionStartTime()
    {
        FloatField("Transition's time (s) : ", serializedObject.FindProperty("transitionStartTime"), 15, 175, 50, "Appaerance time (in second)");
        GUILayout.Space(5);
    }

    private void ShowTransitionEndTime()
    {
        FloatField("Transition's time (s) : ", serializedObject.FindProperty("transitionEndTime"), 15, 175, 50, "Disappaerance time (in second)");
        GUILayout.Space(5);
    }

    private void ShowSentenceDisplayStyle()
    {
        EnumField("Sentence display style", ref m_dialogueData.sentenceDisplayStyle);

        if (m_dialogueData.sentenceDisplayStyle == DialogueData.SentenceDisplayStyle.type)
        {
            FloatField("Typing delay (s) : ", serializedObject.FindProperty("typingDelay"), 15, 175, 50, "Time (in second) between typing");
            GUILayout.Space(5);
        }

        EnumField("Text type : ", ref m_dialogueData.textType);
        GUILayout.Space(5);

        CustomListHeader("Sentences number", "SentencesDataCount", ref m_dialogueData.nbSentences, ref m_dialogueData.sentenceDatas, ref m_dialogueData.showDialogueElements, 120, 20, "Choose sentences number for this text/dialogue", true);

        if (m_dialogueData.showDialogueElements)
        {
            if (m_dialogueData.sentenceDatas.Count == 0)
            {
                return;
            }
            for (int i = 0; i < m_dialogueData.nbSentences; i++)
            {
                ShowFeedbackSentencesData(m_dialogueData.sentenceDatas[i], m_dialogueData.textType, m_dialogueData.sentenceDatas, i);
            }
            GUILayout.Space(5);
        }
    }

    private void ShowFeedbackSentencesData(SentenceData sentenceData, DialogueData.TextType textType, List<SentenceData> sentenceDataList, int i)
    {
        GUILayout.BeginHorizontal();
        if (sentenceData == sentenceDataList[0])
            EditorGUI.BeginDisabledGroup(true);

        if (GUILayout.Button(EditorGUIUtility.IconContent("HoverBar_Up"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
        {
            SwapListItems(sentenceDataList, i, i - 1);
            GUI.FocusControl(null);
        }
        if (sentenceData == sentenceDataList[0])
            EditorGUI.EndDisabledGroup();

        if (sentenceData == sentenceDataList[sentenceDataList.Count - 1])
            EditorGUI.BeginDisabledGroup(true);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_icon dropdown@2x"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
        {
            SwapListItems(sentenceDataList, i, i + 1);
        }

        if (sentenceData == sentenceDataList[sentenceDataList.Count - 1])
            EditorGUI.EndDisabledGroup();

        // ObjectField
        if (sentenceData.sentence != null)
            sentenceData.sentence = sentenceData.sentence.Trim();


        TextField(" ", ref sentenceData.sentence, 75, -3, 188, true, EditorStyles.textArea, 55, null, 170);
        GUILayout.EndHorizontal();
        GUILayout.Space(2);

        if (textType == DialogueData.TextType.dialogue)
        {
            TextField("NPC name : ", ref sentenceData.NPCName, 75, 70, 115, true, null, 0, null, 50); //Choose PNJ name
            GUILayout.Space(2);

            SpriteField("NPC sprite : ", ref sentenceData.NPCSprite, 75, null, 70, 115, 0, "Choose the sprite of the speaking character for this dialogue sentence"); //Choose PNJ sprite
            GUILayout.Space(2);
        }

        ToggleField("Skippable : ", ref sentenceData.isSkippable, 75, 70, 15, "On : Player can skipped that dialogue sentence"); //Choose if this text is skippable

        GUILayout.BeginHorizontal();
        if(sentenceData.withSFX)
        {
            GUILayout.Space(75);
            sentenceData.showSfxElements = GUILayout.Toggle(sentenceData.showSfxElements, "", EditorStyles.foldout, GUILayout.MaxWidth(10), GUILayout.MaxHeight(20));
            ToggleField("With SFX", ref sentenceData.withSFX, 0, 70, 15, "On : Player can add sound");
        }
        else
        {
            ToggleField("With SFX", ref sentenceData.withSFX, 75, 70, 15, "On : Player can add sound");
            sentenceData.showSfxElements = false;
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        if(sentenceData.showSfxElements)
        {
            ShowAudioClipData(sentenceData.audioClipData);
        }


        ToggleField("With custom event", ref sentenceData.withCustomEvent, 75, 70, 15);

        if(sentenceData.withCustomEvent)
        {
            ShowOnSentenceEvent(sentenceData, i);
        }
    }

    private void ShowOnSentenceEvent(SentenceData sentenceData, int i)
    {
        EnumField("When to start event", ref sentenceData.startOfEvent);

        //sentenceData.eventIDToLaunch = GUILayout.TextField("Event Id", GUILayout.MaxWidth(150));
        string eventId = sentenceData.eventIDToLaunch;

        TextField("Event ID", ref eventId);

        sentenceData.eventIDToLaunch = eventId;

        GUILayout.Space(10);

        //SerializedProperty sentenceEvent = m_SPsentenceDataList.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(SentenceData.OnSentence));

        //serializedObject.Update(); 
        //EditorGUILayout.PropertyField(sentenceEvent);
        //serializedObject.ApplyModifiedProperties();
    }

    void ShowAudioClipData(AudioClipData audioClipData)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(75);
        AudioField("", ref audioClipData.audioClip, 0, null, 75, 110, 0, "");
        GUILayout.EndHorizontal();

        ToggleField("Loop : ", ref audioClipData.audioLoop, 75, 110, 15, "On : Music looping");

        FloatField("Starting delay (s) : ", ref audioClipData.delay, 75, 110, 50, "Delay before start audio or his transition");
        GUILayout.Space(5);

        EnumField("Starting type : ", ref audioClipData.startType, 75, 110, 75);
        GUILayout.Space(5);

        if (audioClipData.startType == AudioClipData.StartType.fadeIn)
        {
            FloatField("Fade In time (s) : ", ref audioClipData.fadeInTime, 75, 110, 50, "Time during the volume growing");
            GUILayout.Space(5);
        }

        FloatField("Maximum volume : ", ref audioClipData.maxVolume, 75, 110, 50, "Volume in percentage");
        GUILayout.Space(5);

        EnumField("Ending type : ", ref audioClipData.stopType, 75, 110, 75);
        GUILayout.Space(5);

        if (audioClipData.stopType == AudioClipData.StopType.fadeOut)
        {
            FloatField("Fade Out time (s) : ", ref audioClipData.fadeOutTime, 75, 110, 50, "Time during the volume reducing");
            GUILayout.Space(5);
        }
    }
}
