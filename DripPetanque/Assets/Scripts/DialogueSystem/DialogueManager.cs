using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using static DialogueData;
using System;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private InputActionReference m_inputActionRef;

    public static Dictionary<DialogueIDWraper, EventHandler> EventHandlers = new Dictionary<DialogueIDWraper, EventHandler>();

    public event Action OnDialogueEnded;


    //----------------------------------------------------------
    #region Serialized Fields
    //----------------------------------------------------------
    [SerializeField] private float m_clickCooldown;
    [SerializeField] private GameObject m_dialogueCanvas;
    [SerializeField] private CanvasGroup m_dialogueMainContainerCanvasGroup;
    [SerializeField] private CanvasGroup m_nextButtonCanvasGroup;
    [SerializeField] private TMP_Text m_dialogueText;
    [SerializeField] private GameObject m_dialogueTextContainer;
    //[SerializeField] private Image m_talkerImage;
    [SerializeField] private CanvasGroup m_talkerNameCanvasGroup;
    [SerializeField] private TMP_Text m_talkerNameText;
    [SerializeField] private AnimationCurve m_fadeCurve;
    [SerializeField] private AnimationCurve m_swipeCurve;

    [Header("DoTween Zoom Sequence Reference")]
    [UnityEngine.Serialization.FormerlySerializedAs("_minAnchorAnimZoomStart")]
    [SerializeField] private Vector2 m_minAnchorAnimZoomStart;
    [UnityEngine.Serialization.FormerlySerializedAs("_maxAnchorAnimZoomStart")]
    [SerializeField] private Vector2 m_maxAnchorAnimZoomStart;
    [UnityEngine.Serialization.FormerlySerializedAs("_minAnchorAnimZoomEnd")]
    [SerializeField] private Vector2 m_minAnchorAnimZoomEnd;
    [UnityEngine.Serialization.FormerlySerializedAs("_maxAnchorAnimZoomEnd")]
    [SerializeField] private Vector2 m_maxAnchorAnimZoomEnd;
    #endregion

    //----------------------------------------------------------
    #region Private Fields
    //----------------------------------------------------------
    private DialogueData m_currentDialogueData;
    private readonly List<SentenceData> m_sentenceDatas = new List<SentenceData>();
    private AudioSource m_audioSource;
    private int m_sentenceIndex;
    private float m_currentTransTime;
    private bool m_haveFinishDialogue;
    private bool m_isSkippable;
    private bool m_isClickedOnce;
    private bool m_canClickAgain;
    private TextType m_textType;
    private Sequence m_openDialogueSequence;
    private Sequence m_closeDialogueSequence;
    private int m_endOfDialogueIndex;
    #endregion

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void StartDialogue(DialogueData dialogueData, int fromSentence = -1, int toSentence = -1)
    {
        if(fromSentence != -1)
        {
            if(toSentence != -1 && toSentence > fromSentence)
            {
                _ = StartCoroutine(StartDialogueFromCustomSentenceBoundariesCoroutine(dialogueData, fromSentence, toSentence));
            }
            else
            {
                _ = StartCoroutine(StartDialogueFromCustomSentenceBoundariesCoroutine(dialogueData, fromSentence, fromSentence + 1));
            }

        }
        else
        {
            _ = StartCoroutine(StartDialogueCoroutine(dialogueData));
        }
    }

    private void ToggleDialogueInterfaceElements(bool state)
    {
        if (state)
        {
            _ = m_talkerNameCanvasGroup.DOFade(1, 0.5f).From(0);
        }
        else
        {
            m_talkerNameCanvasGroup.alpha = 0f;
            m_dialogueText.text = "";
            m_nextButtonCanvasGroup.alpha = 0;
        }
        m_dialogueTextContainer.gameObject.SetActive(state);
        //m_talkerImage.gameObject.SetActive(state);
    }

    private IEnumerator StartDialogueCoroutine(DialogueData dialogueData)
    {
        m_haveFinishDialogue = false;
        m_sentenceIndex = 0;
        m_dialogueText.text = "";
        m_textType = dialogueData.textType;
        yield return new WaitForEndOfFrame();
        m_sentenceDatas.Clear();
        m_currentDialogueData = dialogueData;
        foreach (SentenceData sentenceData in dialogueData.sentenceDatas)
        {
            m_sentenceDatas.Add(sentenceData);
        }

        m_endOfDialogueIndex = m_sentenceDatas.Count;

        //Set up dialogue UI element at first start, then update at each ComputeSentences()
        if (m_textType == TextType.Dialogue)
        {
            //m_talkerImage.gameObject.SetActive(true);
            m_talkerNameText.gameObject.SetActive(true);
            //m_talkerImage.sprite = m_sentenceDatas[m_sentenceIndex].NPCSprite;
            m_talkerNameText.text = m_sentenceDatas[m_sentenceIndex].NPCName;

        }

        ShowDialogue();
    }

    private IEnumerator StartDialogueFromCustomSentenceBoundariesCoroutine(DialogueData dialogueData, int fromSentence, int toSentence)
    {
        m_haveFinishDialogue = false;
        m_sentenceIndex = 0;
        m_dialogueText.text = "";
        m_textType = dialogueData.textType;
        yield return new WaitForEndOfFrame();
        m_sentenceDatas.Clear();
        m_currentDialogueData = dialogueData;

        for(int i = fromSentence; i <= toSentence; i++)
        {
            m_sentenceDatas.Add(dialogueData.sentenceDatas[i]);
        }

        //Set up dialogue UI element at first start, then update at each ComputeSentences()
        if (m_textType == TextType.Dialogue)
        {
            //m_talkerImage.gameObject.SetActive(true);
            m_talkerNameText.gameObject.SetActive(true);
            //m_talkerImage.sprite = m_sentenceDatas[m_sentenceIndex].NPCSprite;
            m_talkerNameText.text = m_sentenceDatas[m_sentenceIndex].NPCName;

        }

        ShowDialogue();
    }

    private void ShowDialogue()
    {
        m_currentTransTime = m_currentDialogueData.transitionStartTime;
        DialogueZoomIn(m_currentTransTime);
        //CheckStartDisplayType();
    }

    private void ComputeSentences()
    {
        Debug.Log(m_sentenceIndex);
        SentenceData currentSentenceData = m_sentenceDatas[m_sentenceIndex];
        m_dialogueText.text = currentSentenceData.sentence;

        if(currentSentenceData.withCustomEvent && currentSentenceData.startOfEvent == SentenceData.StartOfEvent.DuringSentenceTyping)
        {
            EventHandlers[currentSentenceData.eventIDToLaunch].LaunchDuringSentenceEvent();
            StartEvent(currentSentenceData);
        }

        //m_nextButtonObj.SetActive(m_sentenceIndex != m_sentenceDatas.Count - 1);

        if (m_textType == TextType.Dialogue)
        {
            //m_talkerImage.gameObject.SetActive(true);
            m_talkerNameText.gameObject.SetActive(true);
            //m_talkerImage.sprite = currentSentenceData.NPCSprite;
            m_talkerNameText.text = currentSentenceData.NPCName;
        }
        else if (m_textType == TextType.Text)
        {
            //m_talkerImage.gameObject.SetActive(false);
            m_talkerNameText.gameObject.SetActive(false);
        }

        StopAllCoroutines();

        switch (m_currentDialogueData.sentenceDisplayStyle)
        {
            case SentenceDisplayStyle.Direct:
                m_dialogueText.text = currentSentenceData.sentence;
                m_nextButtonCanvasGroup.alpha = 1;
                break;

            case SentenceDisplayStyle.Type:
                _ = StartCoroutine(TypeSentence(currentSentenceData));
                break;

            default:
                Debug.LogError("Error !");
                break;
        }
    }

    private void StartEvent(SentenceData currentSentenceData)
    {
    }

    public void DisplayNextDialogue(InputAction.CallbackContext context)
    {
        if (m_currentDialogueData.sentenceDatas[m_sentenceIndex].typingIsComplete)
        {
            m_sentenceIndex++;
            //feedbackSFXAudioSource.PlayOneShot(m_nextDialogueButtonClickSound);
            m_isClickedOnce = false;
        }

        if (m_sentenceIndex < m_sentenceDatas.Count)
        {
            ComputeSentences();
        }
        else
        {
            //Callback();
            EndDialogue();
        }

    }
    public void DisplayNextDialogueCinematic()
    {
        if (m_currentDialogueData.sentenceDatas[m_sentenceIndex].typingIsComplete)
        {
            m_sentenceIndex++;
            //feedbackSFXAudioSource.PlayOneShot(m_nextDialogueButtonClickSound);
            m_isClickedOnce = false;
        }

        if (m_sentenceIndex < m_sentenceDatas.Count)
        {
            ComputeSentences();
        }
        else
        {
            //Callback();
            EndDialogue();
        }

    }

    private IEnumerator ClickCooldownCoroutine()
    {
        m_canClickAgain = false;
        yield return new WaitForSeconds(m_clickCooldown);
        m_canClickAgain = true;
    }

    private void EndDialogue()
    {
        m_currentTransTime = m_currentDialogueData.transitionEndTime;
        DialogueZoomOut(m_currentTransTime);
        m_haveFinishDialogue = true;

        m_inputActionRef.action.performed -= DisplayNextDialogue;
        OnDialogueEnded?.Invoke();
    }

    private IEnumerator TypeSentence(SentenceData currentSentenceData)
    {
        currentSentenceData.typingIsComplete = false;
        m_isSkippable = currentSentenceData.isSkippable;
        m_dialogueText.text = "";
        m_nextButtonCanvasGroup.alpha = 0;
        //if (!m_isSkippable)
        //{
        //    m_nextButtonObj.SetActive(false);
        //}
        if (!m_isClickedOnce)
        {
            m_isClickedOnce = true;
            foreach (char letter in currentSentenceData.sentence.ToCharArray())
            {
                m_dialogueText.text += letter;
                yield return new WaitForSeconds(m_currentDialogueData.typingDelay);
            }
        }
        else
        {
            m_dialogueText.text = currentSentenceData.sentence;
        }

        if (currentSentenceData.withCustomEvent && currentSentenceData.startOfEvent == SentenceData.StartOfEvent.AfterSentenceTyping)
        {
            StartEvent(currentSentenceData);
        }

        currentSentenceData.typingIsComplete = true;
        m_nextButtonCanvasGroup.alpha = 1;
        //m_nextButtonObj.SetActive(m_sentenceIndex != m_sentenceDatas.Count - 1);
    }

    #region Dialogue Transition in

    private void DialogueFadeIn(float transitionDuration)
    {
        m_dialogueCanvas.SetActive(true);
        m_openDialogueSequence = DOTween.Sequence();
        _ = m_openDialogueSequence.Append(m_dialogueMainContainerCanvasGroup.DOFade(1, transitionDuration).From(0).SetEase(m_fadeCurve));

        _ = m_openDialogueSequence.OnComplete(() =>
        {
            ComputeSentences();
        });
        _ = m_openDialogueSequence.Play();
    }

    private void DialogueZoomIn(float transitionDuration)
    {
        m_dialogueCanvas.SetActive(true);
        m_nextButtonCanvasGroup.gameObject.SetActive(!m_currentDialogueData.IsCinematicDialogue);
        m_openDialogueSequence = DOTween.Sequence();

        RectTransform dialogueMainContainerRectTransform = m_dialogueMainContainerCanvasGroup.GetComponent<RectTransform>();

        m_closeDialogueSequence.Kill();

        _ = m_openDialogueSequence.Insert(0f, dialogueMainContainerRectTransform.DOAnchorMin(m_minAnchorAnimZoomEnd, transitionDuration).From(m_minAnchorAnimZoomStart).SetEase(Ease.OutQuart));
        _ = m_openDialogueSequence.Insert(0f, dialogueMainContainerRectTransform.DOAnchorMax(m_maxAnchorAnimZoomEnd, transitionDuration).From(m_maxAnchorAnimZoomStart).SetEase(Ease.OutQuart));

        _ = m_openDialogueSequence.OnComplete(() =>
        {
            ToggleDialogueInterfaceElements(true);
            m_inputActionRef.action.performed += DisplayNextDialogue;
            ComputeSentences();
        });

        _ = m_openDialogueSequence.Play();
    }

    
    #endregion

    #region Dialogue Transition out

    private void DialogueFadeOut(float transitionDuration)
    {
        Sequence sequence = DOTween.Sequence();

        m_nextButtonCanvasGroup.alpha = 0;
        _ = sequence.Append(m_dialogueMainContainerCanvasGroup.DOFade(0, transitionDuration).From(1).SetEase(m_fadeCurve));
        //sequence.Join(m_dialogueText.GetComponent<TMP_Text>().DOFade(0, transitionDuration).SetEase(m_fadeCurve));
        //if (m_textType == DialogueData.TextType.dialogue)
        //{
        //    sequence.Join(m_talkerImage.GetComponent<Image>().DOFade(0, transitionDuration).SetEase(m_fadeCurve));
        //    sequence.Join(m_talkerNameText.GetComponent<TMP_Text>().DOFade(0, transitionDuration).SetEase(m_fadeCurve));
        //}

        _ = sequence.OnComplete(() =>
        {
            //setImageColorAlpha(m_dialogueContent, 1);
            //setTextColorAlpha(m_dialogueText, 1);
            //if (m_textType == DialogueData.TextType.dialogue)
            //{
            //    setImageColorAlpha(m_talkerImage.gameObject, 1);
            //    setTextColorAlpha(m_talkerNameText, 1);
            //}
            m_dialogueCanvas.SetActive(false);
            m_haveFinishDialogue = true;
        });

        _ = sequence.Play();
    }

    private void DialogueZoomOut(float transitionDuration)
    {
        Vector2 originPos = m_dialogueCanvas.GetComponent<RectTransform>().localScale;

        m_dialogueCanvas.SetActive(true);
        ShowNPCImage(true);
        m_closeDialogueSequence = DOTween.Sequence();
        RectTransform dialogueMainContainerRectTransform = m_dialogueMainContainerCanvasGroup.GetComponent<RectTransform>();

        _ = m_closeDialogueSequence.Insert(0f, dialogueMainContainerRectTransform.DOAnchorMin(m_maxAnchorAnimZoomStart, transitionDuration).From(m_minAnchorAnimZoomEnd).SetEase(Ease.OutQuart));
        _ = m_closeDialogueSequence.Insert(0f, dialogueMainContainerRectTransform.DOAnchorMax(m_maxAnchorAnimZoomStart, transitionDuration).From(m_maxAnchorAnimZoomEnd).SetEase(Ease.OutQuart));

        ToggleDialogueInterfaceElements(false);

        _ = m_closeDialogueSequence.OnComplete(() =>
        {
            m_dialogueCanvas.SetActive(false);
            //m_dialogueCanvas.GetComponent<RectTransform>().localScale = originPos;
            m_haveFinishDialogue = true;
        });
        _ = m_closeDialogueSequence.Play();
    }
    #endregion

    private void ShowNPCImage(bool isShown)
    {
        if (m_textType == TextType.Dialogue)
        {
            //m_talkerImage.gameObject.SetActive(isShown);
            //npcNameGO.SetActive(isShown);
            m_talkerNameText.gameObject.SetActive(isShown);
        }
    }

}
