using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility.SerializedDictionary;

[CreateAssetMenu(fileName = nameof(SoundLibrary), menuName = "Scriptables/Sound/" + nameof(SoundLibrary))]
public class SoundLibrary : ScriptableObject
{
    public SerializedDictionary<string, AudioClip> SoundAudioClips;
}
