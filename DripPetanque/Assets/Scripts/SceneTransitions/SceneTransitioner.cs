using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtility.Recorders;
using UnityUtility.SceneReference;
using UnityUtility.Timer;

public class SceneTransitioner : MonoBehaviour
{
    public enum TransitionAction
    {
        LoadScene,
        UnloadScene,
    }

    private enum TransitionStep
    {
        FadeIn,
        DoTransitionAction,
        FadeOut,
        Done,
    }

    public event Action<SceneReference> OnTransitionStart;
    public event Action<SceneReference> OnTransitionEnd;

    [SerializeField] private SceneReference m_sceneToTransitionTo;

    [SerializeField] private ScreenCache m_cache;

    [SerializeField] private Timer m_fadeTimer;

    [SerializeField] private TransitionAction m_transitionAction = TransitionAction.LoadScene;


    [NonSerialized] private bool m_isTransitioning;
    [NonSerialized] private TransitionStep m_currentStep;

    [NonSerialized] private HierarchicalRecorder m_recorder;

    [ContextMenu("Transition")]
    public void StartTransition()
    {
        m_recorder = new HierarchicalRecorder();
        m_recorder.BeginEvent("SceneTransition");
        m_isTransitioning = true;
        OnTransitionStart?.Invoke(m_sceneToTransitionTo);
        StartFadeIn();
    }

    private void Update()
    {
        if (m_isTransitioning)
        {
            switch (m_currentStep)
            {
                case TransitionStep.FadeIn:
                    UpdateFadeIn(Time.deltaTime);
                    break;
                case TransitionStep.DoTransitionAction:
                    break;
                case TransitionStep.FadeOut:
                    UpdateFadeOut(Time.deltaTime);
                    break;
                case TransitionStep.Done:
                default:
                    break;
            }
        }
    }

    private void StartFadeIn()
    {
        m_recorder.BeginEvent("FadeIn");
        m_currentStep = TransitionStep.FadeIn;
        m_cache.UpdateCache(0.0f);
        m_cache.Show(true);

        m_fadeTimer.Start();
    }

    private void UpdateFadeIn(float deltaTime)
    {
        if (m_fadeTimer.Update(deltaTime))
        {
            m_fadeTimer.Stop();
            m_cache.UpdateCache(1.0f);
            DoTransitionAction();
            return;
        }
        m_cache.UpdateCache(m_fadeTimer.Progress);
    }

    private void DoTransitionAction()
    {
        m_currentStep = TransitionStep.DoTransitionAction;
        switch (m_transitionAction)
        {
            case TransitionAction.LoadScene:
                LoadScene();
                break;
            case TransitionAction.UnloadScene:
                UnloadScene();
                break;
            default:
                break;
        }
    }

    private void LoadScene()
    {
        m_recorder.EndEvent();
        m_recorder.BeginEvent("Scene loading");
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(m_sceneToTransitionTo, LoadSceneMode.Additive);
        loadOperation.completed += OnSceneActionOver;
    }

    private void UnloadScene()
    {
        m_recorder.EndEvent();
        m_recorder.BeginEvent("Scene unloading");
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(m_sceneToTransitionTo);
        unloadOperation.completed += OnSceneActionOver;
    }

    private void OnSceneActionOver(AsyncOperation operation)
    {
        operation.completed -= OnSceneActionOver;
        StartFadeOut();
    }

    private void StartFadeOut()
    {
        m_recorder.EndEvent();
        m_recorder.BeginEvent("FadeOut");
        m_currentStep = TransitionStep.FadeOut;
        m_cache.UpdateCache(1.0f);
        m_cache.Show(true);

        m_fadeTimer.Start();
    }

    private void UpdateFadeOut(float deltaTime)
    {
        if (m_fadeTimer.Update(deltaTime))
        {
            m_fadeTimer.Stop();
            EndTransition();
            m_cache.Show(false);
            m_cache.UpdateCache(0.0f);
            return;
        }
        m_cache.UpdateCache(1.0f - m_fadeTimer.Progress);
    }

    private void EndTransition()
    {
        m_currentStep = TransitionStep.Done;
        m_isTransitioning = false;
        OnTransitionEnd?.Invoke(m_sceneToTransitionTo);

        m_recorder.EndEvent();
        m_recorder.EndEvent();
    }
}