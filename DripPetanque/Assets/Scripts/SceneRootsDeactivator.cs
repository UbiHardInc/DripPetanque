using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRootsDeactivator : MonoBehaviour
{
    [NonSerialized] private Dictionary<GameObject, bool> m_rootsLastStates = new Dictionary<GameObject, bool>();

    public void DisableSceneRoots()
    {
        m_rootsLastStates.Clear();
        foreach (GameObject obj in gameObject.scene.GetRootGameObjects())
        {
            m_rootsLastStates.Add(obj, obj.activeSelf);
            obj.SetActive(false);
        }
    }

    public void ResetRootsStates()
    {
        foreach(KeyValuePair<GameObject, bool> pair in m_rootsLastStates)
        {
            pair.Key.SetActive(pair.Value);
        }
    }
}
