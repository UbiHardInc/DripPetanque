using UnityEditor;
using UnityEngine;
using static AudioClipData;
using static DialogueData;
using static SentenceData;

[CustomEditor(typeof(DialogueData))]
public class DialogueDataEditor : CustomEditorBase
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

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
        SerializedProperty sentenceDisplayStyle = serializedObject.FindProperty("sentenceDisplayStyle");
        AddPopup(ref sentenceDisplayStyle, "Sentence display style", typeof(SentenceDisplayStyle));
        //EnumField("Sentence display style", ref m_dialogueData.sentenceDisplayStyle);

        if (m_dialogueData.sentenceDisplayStyle == DialogueData.SentenceDisplayStyle.Type)
        {
            FloatField("Typing delay (s) : ", serializedObject.FindProperty("typingDelay"), 15, 175, 50, "Time (in second) between typing");
            GUILayout.Space(5);
        }

        SerializedProperty textType = serializedObject.FindProperty("textType");
        AddPopup(ref textType, "Text type : ", typeof(TextType));
        //EnumField("Text type : ", ref m_dialogueData.textType);
        GUILayout.Space(5);
        SerializedProperty sentenceDataList = serializedObject.FindProperty("sentenceDatas");

        SerializedProperty nbSentences = serializedObject.FindProperty("nbSentences");
        CustomListHeader("Sentences number", "SentencesDataCount", nbSentences, sentenceDataList, ref m_dialogueData.showDialogueElements, 120, 20, "Choose sentences number for this text/dialogue", true);

        if (m_dialogueData.showDialogueElements)
        {
            if (sentenceDataList.arraySize == 0)
            {
                return;
            }
            for (int i = 0; i < nbSentences.intValue; i++)
            {
                SerializedProperty sentenceData = serializedObject.FindProperty("sentenceDatas").GetArrayElementAtIndex(i);
                ShowFeedbackSentencesData(sentenceData, m_dialogueData.textType, sentenceDataList, i);
                GUILayout.Label("________________________________________________________________________");
            }
            GUILayout.Space(5);
        }
    }

    //private List<T> GetListFromSerializedProperty<T>(SerializedProperty serializedListProperty)
    //{
    //    if(!serializedListProperty.isArray)
    //    {
    //        return null;
    //    }

    //    int arrayLength = 0;

    //    serializedListProperty.Next(true);
    //    serializedListProperty.Next(true);

    //    arrayLength = serializedListProperty.arraySize;

    //    serializedListProperty.Next(true);

    //    List<T> list = new List<T>(arrayLength);

    //    int lastIndex = arrayLength - 1;

    //    for(int i = 0; i < arrayLength; i++)
    //    {
    //        list.Add((T)serializedListProperty.GetArrayElementAtIndex(i).objectReferenceValue);
    //    }

    //    return list;
    //}

    private void ShowFeedbackSentencesData(SerializedProperty sentenceData, DialogueData.TextType textType, SerializedProperty sentenceDataList, int i)
    {
        GUILayout.BeginHorizontal();
        if (SerializedProperty.EqualContents(sentenceData, sentenceDataList.GetArrayElementAtIndex(0)))
            EditorGUI.BeginDisabledGroup(true);

        if (GUILayout.Button(EditorGUIUtility.IconContent("HoverBar_Up"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
        {
            SwapListItems(sentenceDataList, i, i - 1);
            GUI.FocusControl(null);
        }
        if (SerializedProperty.EqualContents(sentenceData, sentenceDataList.GetArrayElementAtIndex(0)))
            EditorGUI.EndDisabledGroup();

        if (SerializedProperty.EqualContents(sentenceData, sentenceDataList.GetArrayElementAtIndex(sentenceDataList.arraySize - 1)))
            EditorGUI.BeginDisabledGroup(true);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_icon dropdown@2x"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
        {
            SwapListItems(sentenceDataList, i, i + 1);
        }

        if (SerializedProperty.EqualContents(sentenceData, sentenceDataList.GetArrayElementAtIndex(sentenceDataList.arraySize - 1)))
            EditorGUI.EndDisabledGroup();

        SerializedProperty sentence = sentenceData.FindPropertyRelative("sentence");
        SerializedProperty NPCName = sentenceData.FindPropertyRelative("NPCName");

        // ObjectField
        if (sentence.stringValue != null)
            sentence.stringValue = sentence.stringValue.Trim();


        TextField(" ", sentence, 75, -3, 188, true, EditorStyles.textArea, 55, null, 170);
        GUILayout.EndHorizontal();
        GUILayout.Space(2);

        if (textType == DialogueData.TextType.Dialogue)
        {
            TextField("NPC name : ", NPCName, 75, 70, 115, true, null, 0, null, 50); //Choose PNJ name
            GUILayout.Space(2);

            //SpriteField("NPC sprite : ", ref sentenceData.NPCSprite, 75, null, 70, 115, 0, "Choose the sprite of the speaking character for this dialogue sentence"); //Choose PNJ sprite
            GUILayout.Space(2);
        }

        SerializedProperty isSkippable = sentenceData.FindPropertyRelative("isSkippable");
        SerializedProperty withSFX = sentenceData.FindPropertyRelative("withSFX");
        ToggleField("Skippable : ", isSkippable, 75, 70, 15, "On : Player can skipped that dialogue sentence"); //Choose if this text is skippable

        SerializedProperty showSfxElements = sentenceData.FindPropertyRelative("showSfxElements");
        GUILayout.BeginHorizontal();
        if(withSFX.boolValue)
        {
            GUILayout.Space(75);
            showSfxElements.boolValue = GUILayout.Toggle(showSfxElements.boolValue, "", EditorStyles.foldout, GUILayout.MaxWidth(10), GUILayout.MaxHeight(20));
            ToggleField("With SFX", withSFX, 0, 70, 15, "On : Player can add sound");
        }
        else
        {
            ToggleField("With SFX", withSFX, 75, 70, 15, "On : Player can add sound");
            showSfxElements.boolValue = false;
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        SerializedProperty audioClipData = sentenceData.FindPropertyRelative("audioClipData");

        if (showSfxElements.boolValue)
        {
            ShowAudioClipData(audioClipData);
        }

        SerializedProperty withCustomEvent = sentenceData.FindPropertyRelative("withCustomEvent");

        ToggleField("With custom event", withCustomEvent, 75, 70, 15);

        if(withCustomEvent.boolValue)
        {
            ShowOnSentenceEvent(sentenceData, i);
        }
    }

    private void ShowOnSentenceEvent(SerializedProperty sentenceData, int i)
    {
        SerializedProperty startOfEvent = sentenceData.FindPropertyRelative("startOfEvent");
        AddPopup(ref startOfEvent, "When to start event", typeof(StartOfEvent));
        //EnumField("When to start event", ref sentenceData.startOfEvent);
        
        //string eventId = sentenceData.eventIDToLaunch;

        SerializedProperty eventIDToLaunch = sentenceData.FindPropertyRelative("eventIDToLaunch");
        SerializedProperty stringID = eventIDToLaunch.FindPropertyRelative("_iD");
        TextField("Event ID", stringID);
        //sentenceData.eventIDToLaunch = eventId;

        GUILayout.Space(10);

        //SerializedProperty sentenceEvent = m_SPsentenceDataList.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(SentenceData.OnSentence));

        //serializedObject.Update(); 
        //EditorGUILayout.PropertyField(sentenceEvent);
        //serializedObject.ApplyModifiedProperties();
    }

    void ShowAudioClipData(SerializedProperty audioClipData)
    {
        SerializedProperty audioClip = audioClipData.FindPropertyRelative("audioClip");
        GUILayout.BeginHorizontal();
        GUILayout.Space(75);
        AudioField("", audioClip, 0, null, 75, 110, 0, "");
        GUILayout.EndHorizontal();

        SerializedProperty audioLoop = audioClipData.FindPropertyRelative("audioLoop");
        ToggleField("Loop : ", audioLoop, 75, 110, 15, "On : Music looping");

        SerializedProperty delay = audioClipData.FindPropertyRelative("delay");
        FloatField("Starting delay (s) : ", serializedObject.FindProperty("transitionEndTime"), 75, 110, 50, "Delay before start audio or his transition");
        GUILayout.Space(5);

        SerializedProperty startType = audioClipData.FindPropertyRelative("startType");
        AddPopup(ref startType, "Starting type : ", typeof(StartType));
        //EnumField("Starting type : ", ref audioClipData.startType, 75, 110, 75);
        GUILayout.Space(5);

        if (startType.enumValueIndex == (int)AudioClipData.StartType.FadeIn)
        {
            SerializedProperty fadeInTime = audioClipData.FindPropertyRelative("fadeInTime");
            FloatField("Fade In time (s) : ", fadeInTime, 75, 110, 50, "Time during the volume growing");
            GUILayout.Space(5);
        }

        SerializedProperty maxVolume = audioClipData.FindPropertyRelative("maxVolume");
        FloatField("Maximum volume : ", maxVolume, 75, 110, 50, "Volume in percentage");
        GUILayout.Space(5);

        SerializedProperty stopType = audioClipData.FindPropertyRelative("stopType");
        AddPopup(ref stopType, "Ending type : ", typeof(StopType));
        //EnumField("Ending type : ", ref audioClipData.stopType, 75, 110, 75);
        GUILayout.Space(5);

        if (stopType.enumValueIndex == (int)AudioClipData.StopType.FadeOut)
        {
            SerializedProperty fadeOutTime = audioClipData.FindPropertyRelative("fadeOutTime");
            FloatField("Fade Out time (s) : ", fadeOutTime, 75, 110, 50, "Time during the volume reducing");
            GUILayout.Space(5);
        }
    }
}
