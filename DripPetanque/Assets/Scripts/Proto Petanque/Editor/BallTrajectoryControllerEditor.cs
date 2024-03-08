using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(BallTrajectoryController))]
public class BallTrajectoryControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying && GUILayout.Button("Start New Ball"))
        {
            (target as BallTrajectoryController).StartNewBall();
        }
        if (Application.isPlaying && GUILayout.Button("Clear All Balls"))
        {
            BallTrajectoryController castedTarget = target as BallTrajectoryController;
            castedTarget.StartCoroutine(castedTarget.ClearAllBalls());
        }
        base.OnInspectorGUI();
    }
}
