using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Canon))]
public class CanonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Shoot"))
        {
            Shoot();
        }
        base.OnInspectorGUI();
    }

    private void Shoot()
    {
        (target as Canon).Shoot();
    }
}
