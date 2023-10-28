using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CustomEditorBase : Editor
{
	protected DialogueData m_dialogueData;
	protected SerializedProperty m_SPsentenceDataList;
	protected SerializedProperty m_SPTransitionStartTime;

    private void OnEnable()
    {
		m_dialogueData = (DialogueData)target;
        //m_SPsentenceDataList = serializedObject.FindProperty("sentenceDatas");
    }

    protected void CustomListHeader<T>(string name, string controlName, ref int desiredCount, ref List<T> listToPopulate, ref bool showListElements, int maxWidth = 110, int maxHeight = 20,
		   string tooltip = null, bool withToggle = true, bool desiredCountCanBeZero = false)
	{
		GUILayout.BeginHorizontal();
		if (withToggle)
		{
			showListElements = GUILayout.Toggle(showListElements, new GUIContent(name, tooltip), EditorStyles.foldout, GUILayout.MaxWidth(maxWidth), GUILayout.MaxHeight(maxHeight));
		}
		else
		{
			GUILayout.Label(new GUIContent(name, tooltip), GUILayout.MaxWidth(maxWidth));
			showListElements = true;
		}


		GUI.SetNextControlName(controlName);
		IntField("", ref desiredCount, 0, 0, 30, null);

		if (desiredCount < 0)
			desiredCount = 0;
		//Add an element
		if (desiredCount <= 0)
		{
			EditorGUI.BeginDisabledGroup(true);
		}
		if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus@2x"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
		{
			desiredCount--;
			PopulateList(listToPopulate, desiredCount);
		}
		if (desiredCount <= 0)
		{
			EditorGUI.EndDisabledGroup();
		}
		//Delete an element
		GUILayout.Space(1);
		if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
		{
			desiredCount++;
			PopulateList(listToPopulate, desiredCount);
		}
		GUILayout.EndHorizontal();
	}

	public void PopulateList<T>(List<T> list, int editorListCount)
    {
		while(list.Count < editorListCount)
        {
			if(typeof(T) == typeof(SentenceData))
			{
				AddSentenceData(m_dialogueData.sentenceDatas);
			}
        }

		while(list.Count > editorListCount)
        {
			if (typeof(T) == typeof(SentenceData))
			{
				RemoveSentenceData(m_dialogueData.sentenceDatas);
			}
        }
    }

	private void AddSentenceData(List<SentenceData> list)
    {
		list.Add(new SentenceData());
    }

	private void RemoveSentenceData(List<SentenceData> list)
    {
		int lastIndex = list.Count - 1;

		list.Remove(list[lastIndex]);
    }

	protected void SwapListItems<T>(List<T> list, int currentIndex, int swapIndex)
	{
		var tempIp = list[swapIndex];
		list[swapIndex] = list[currentIndex];
		list[currentIndex] = tempIp;
	}

	protected void SpriteField(string labelString, ref Sprite fieldSprite, int padding = 0, Image imageSpriteToReplace = null, int maxLabelWidth = 135, int maxSpriteFieldWidth = 150,
		int maxSpriteFieldHeight = 0, string tooltip = null)
	{

		if (labelString != "")
			GUILayout.BeginHorizontal();

		GUILayout.Space(padding);

		if (labelString != "")
			GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));

		if (maxSpriteFieldHeight > 0)
			fieldSprite = EditorGUILayout.ObjectField(fieldSprite, typeof(Sprite), false, GUILayout.MaxWidth(maxSpriteFieldWidth), GUILayout.MaxHeight(maxSpriteFieldHeight)) as Sprite;
		else
			fieldSprite = EditorGUILayout.ObjectField(fieldSprite, typeof(Sprite), false, GUILayout.MaxWidth(maxSpriteFieldWidth)) as Sprite;

		if (imageSpriteToReplace != null)
		{
			if (imageSpriteToReplace.sprite != fieldSprite)
			{
				imageSpriteToReplace.sprite = fieldSprite;
				EditorUtility.SetDirty(imageSpriteToReplace.gameObject);
			}
		}
		if (labelString != "")
			GUILayout.EndHorizontal();
	}

	protected void ToggleField(string labelString, ref bool fieldBool, int padding = 0, int maxLabelWidth = 135, int maxFloatFieldWidth = 150, string tooltip = null)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));
		fieldBool = EditorGUILayout.Toggle(fieldBool, GUILayout.MaxWidth(maxFloatFieldWidth));
		GUILayout.EndHorizontal();
	}

	protected void FloatField(string labelString, SerializedProperty propertyToEdit, int padding = 0, int maxLabelWidth = 135, int maxFloatFieldWidth = 150, string tooltip = null)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));

        propertyToEdit.floatValue = EditorGUILayout.FloatField(propertyToEdit.floatValue, GUILayout.MaxWidth(maxFloatFieldWidth));
		GUILayout.EndHorizontal();
	}

	protected void IntField(string labelString, ref int fieldInt, int padding = 0, int maxLabelWidth = 135, int maxFloatFieldWidth = 150, string tooltip = null)
	{
		if (labelString != "")
			GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));

		fieldInt = EditorGUILayout.IntField(fieldInt, GUILayout.MaxWidth(maxFloatFieldWidth));
		if (labelString != "")
			GUILayout.EndHorizontal();
	}

	protected void Label(string labelString, int padding = 0, int maxLabelWidth = 135)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		GUILayout.Label(labelString, GUILayout.MaxWidth(maxLabelWidth));
		GUILayout.EndHorizontal();
	}

	protected void EnumField<T>(string labelString, ref T @enum, int padding = 0, int maxLabelWidth = 135, int maxEnumFieldWidth = 95, string tooltip = "") where T : struct
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		if (!string.IsNullOrEmpty(labelString))
			GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));
		@enum = (T)(object)EditorGUILayout.EnumPopup((System.Enum)(object)@enum, GUILayout.MaxWidth(maxEnumFieldWidth));
		GUILayout.EndHorizontal();
	}

    protected void AudioField(string labelString, ref AudioClip fieldClip, int padding = 0, AudioClip clipToReplace = null, int maxLabelWidth = 135, int maxClipFieldWidth = 150,
        int maxClipFieldHeight = 0, string tooltip = null)
    {

        if (labelString != "")
            GUILayout.BeginHorizontal();
        GUILayout.Space(padding);
        if (labelString != "")
            GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));
        if (maxClipFieldHeight > 0)
            fieldClip = EditorGUILayout.ObjectField(fieldClip, typeof(AudioClip), false, GUILayout.MaxWidth(maxClipFieldWidth), GUILayout.MaxHeight(maxClipFieldHeight)) as AudioClip;
        else
            fieldClip = EditorGUILayout.ObjectField(fieldClip, typeof(AudioClip), false, GUILayout.MaxWidth(maxClipFieldWidth)) as AudioClip;

        if (clipToReplace != null)
        {
            if (clipToReplace != fieldClip)
            {
                clipToReplace = fieldClip;
                EditorUtility.SetDirty(clipToReplace);
            }
        }
        if (labelString != "")
            GUILayout.EndHorizontal();
    }

    protected void TextField(string labelString, ref string fieldString, int padding = 0, int maxLabelWidth = 135, int maxTextFieldWidth = 150,
	bool textArea = false, GUIStyle style = null, int minTextFieldHeight = 0, string tooltip = null, int maxChar = 200)
	{

		if (style == null)
			style = new GUIStyle("textArea");
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		if (labelString != "")
			GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));

		if (textArea)
		{
			if (minTextFieldHeight == 0)
				fieldString = EditorGUILayout.TextArea(fieldString, style, GUILayout.MaxWidth(maxTextFieldWidth));
			else
				fieldString = EditorGUILayout.TextArea(fieldString, style, GUILayout.MaxWidth(maxTextFieldWidth), GUILayout.MinHeight(minTextFieldHeight));
		}
		else
			fieldString = EditorGUILayout.TextField(fieldString, GUILayout.MaxWidth(maxTextFieldWidth));

		if (fieldString != null)
		{
			GUILayout.Label(fieldString.Length.ToString() + " / " + maxChar.ToString());
		}
		GUILayout.EndHorizontal();
	}
}
