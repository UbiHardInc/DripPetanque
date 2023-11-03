using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;

public class CustomEditorBase : Editor
{
	protected DialogueData m_dialogueData;
	protected SerializedProperty m_sentence;

    private void OnEnable()
    {
		m_dialogueData = (DialogueData)target;
        //m_SPsentenceDataList = serializedObject.FindProperty("sentenceDatas");
    }

    protected void CustomListHeader(string name, string controlName, SerializedProperty desiredCount, SerializedProperty listToPopulate, ref bool showListElements, int maxWidth = 110, int maxHeight = 20,
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
		IntField("", desiredCount, 0, 0, 30, null);

		if (desiredCount.intValue < 0)
			desiredCount.intValue = 0;
		//Add an element
		if (desiredCount.intValue <= 0)
		{
			EditorGUI.BeginDisabledGroup(true);
		}
		if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus@2x"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
		{
			desiredCount.intValue--;
			PopulateList(listToPopulate, desiredCount);
		}
		if (desiredCount.intValue <= 0)
		{
			EditorGUI.EndDisabledGroup();
		}
		//Delete an element
		GUILayout.Space(1);
		if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)))
		{
			desiredCount.intValue++;
			PopulateList(listToPopulate, desiredCount);
		}
		GUILayout.EndHorizontal();
	}

	public void PopulateList(SerializedProperty list, SerializedProperty editorListCount)
    {
		while(list.arraySize < editorListCount.intValue)
        {
			if(list.arrayElementType == "SentenceData")

            {
				AddSentenceData(list);
			}
        }

		while(list.arraySize > editorListCount.intValue)
        {
			if (list.arrayElementType == "SentenceData")
			{
				RemoveSentenceData(list);
			}
        }
    }

	private void AddSentenceData(SerializedProperty list)
    {
		list.arraySize++;
        serializedObject.ApplyModifiedProperties();
    }

	private void RemoveSentenceData(SerializedProperty list)
    {
		int lastIndex = list.arraySize - 1;

		list.DeleteArrayElementAtIndex(lastIndex);
    }

	protected void SwapListItems(SerializedProperty list, int currentIndex, int swapIndex)
	{
		//var tempIp = list[swapIndex];
		//list[swapIndex] = list[currentIndex];
		//list[currentIndex] = tempIp;

        if (currentIndex == swapIndex) return;

        if (currentIndex > swapIndex) (currentIndex, swapIndex) = (swapIndex, currentIndex);

        list.MoveArrayElement(currentIndex, swapIndex);

        if (swapIndex - currentIndex != 0) list.MoveArrayElement(swapIndex - 1, currentIndex);
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

	protected void ToggleField(string labelString, SerializedProperty propertyToEdit, int padding = 0, int maxLabelWidth = 135, int maxFloatFieldWidth = 150, string tooltip = null)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));
        propertyToEdit.boolValue = EditorGUILayout.Toggle(propertyToEdit.boolValue, GUILayout.MaxWidth(maxFloatFieldWidth));
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

	protected void IntField(string labelString, SerializedProperty fieldInt, int padding = 0, int maxLabelWidth = 135, int maxFloatFieldWidth = 150, string tooltip = null)
	{
		if (labelString != "")
			GUILayout.BeginHorizontal();
		GUILayout.Space(padding);
		GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));

		fieldInt.intValue = EditorGUILayout.IntField(fieldInt.intValue, GUILayout.MaxWidth(maxFloatFieldWidth));
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

	//protected void EnumField<T>(string labelString, SerializedProperty enumToEdit, Type typeOfEnum, int padding = 0, int maxLabelWidth = 135, int maxEnumFieldWidth = 95, string tooltip = "") where T : struct
	//{
	//	GUILayout.BeginHorizontal();
	//	GUILayout.Space(padding);
	//	if (!string.IsNullOrEmpty(labelString))
	//		GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));
	//	enumToEdit.enumValueIndex = (int)(T)EditorGUILayout.EnumPopup((Enum)enumToEdit.enumValueIndex, GUILayout.MaxWidth(maxEnumFieldWidth));
	//	GUILayout.EndHorizontal();
	//}

    protected void AddPopup(ref SerializedProperty ourSerializedProperty, string nameOfLabel, Type typeOfEnum)
    {
        Rect ourRect = EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginProperty(ourRect, GUIContent.none, ourSerializedProperty);
        EditorGUI.BeginChangeCheck();

        int actualSelected = 1;
        int selectionFromInspector = ourSerializedProperty.intValue;
        string[] enumNamesList = System.Enum.GetNames(typeOfEnum);
        actualSelected = EditorGUILayout.Popup(nameOfLabel, selectionFromInspector, enumNamesList);
        ourSerializedProperty.intValue = actualSelected;

        EditorGUI.EndProperty();
        EditorGUILayout.EndHorizontal();
    }

    protected void AudioField(string labelString, SerializedProperty audioFieldToEdit, int padding = 0, AudioClip clipToReplace = null, int maxLabelWidth = 135, int maxClipFieldWidth = 150,
        int maxClipFieldHeight = 0, string tooltip = null)
    {

        if (labelString != "")
            GUILayout.BeginHorizontal();
        GUILayout.Space(padding);
        if (labelString != "")
            GUILayout.Label(new GUIContent(labelString, tooltip), GUILayout.MaxWidth(maxLabelWidth));
        if (maxClipFieldHeight > 0)
            audioFieldToEdit.objectReferenceValue = EditorGUILayout.ObjectField(audioFieldToEdit.objectReferenceValue, typeof(AudioClip), false, GUILayout.MaxWidth(maxClipFieldWidth), GUILayout.MaxHeight(maxClipFieldHeight)) as AudioClip;
        else
            audioFieldToEdit.objectReferenceValue = EditorGUILayout.ObjectField(audioFieldToEdit.objectReferenceValue, typeof(AudioClip), false, GUILayout.MaxWidth(maxClipFieldWidth)) as AudioClip;

        if (clipToReplace != null)
        {
            if (clipToReplace != audioFieldToEdit.objectReferenceValue)
            {
                clipToReplace = (AudioClip)audioFieldToEdit.objectReferenceValue;
                EditorUtility.SetDirty(clipToReplace);
            }
        }
        if (labelString != "")
            GUILayout.EndHorizontal();
    }

    protected void TextField(string labelString, SerializedProperty propertyToEdit, int padding = 0, int maxLabelWidth = 135, int maxTextFieldWidth = 150,
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
				propertyToEdit.stringValue = EditorGUILayout.TextArea(propertyToEdit.stringValue, style, GUILayout.MaxWidth(maxTextFieldWidth));
			else
				propertyToEdit.stringValue = EditorGUILayout.TextArea(propertyToEdit.stringValue, style, GUILayout.MaxWidth(maxTextFieldWidth), GUILayout.MinHeight(minTextFieldHeight));
		}
		else
			propertyToEdit.stringValue = EditorGUILayout.TextField(propertyToEdit.stringValue, GUILayout.MaxWidth(maxTextFieldWidth));

		if (propertyToEdit.stringValue != null)
		{
			GUILayout.Label(propertyToEdit.stringValue.Length.ToString() + " / " + maxChar.ToString());
		}
		GUILayout.EndHorizontal();
	}
}
