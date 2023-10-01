using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;
using DG.Tweening.Core.Easing;
using static DialogueData;
using UnityEditor.Rendering;
using static AudioClipData;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static Dictionary<DialogueIDWraper, EventHandler> EventHandlers = new Dictionary<DialogueIDWraper, EventHandler>();

    [SerializeField] private float m_clickCooldown;
    [SerializeField] private GameObject m_dialogueContent;
    [SerializeField] private GameObject m_nextButtonObj;
    [SerializeField] private TMP_Text m_dialogueText;
    [SerializeField] private Image m_talkerImage;
    [SerializeField] private TMP_Text m_talkerNameText;
    [SerializeField] private AnimationCurve m_fadeCurve;
    [SerializeField] private AnimationCurve m_swipeCurve;

    private DialogueData m_currentDialogueData;
    private List<SentenceData> m_sentenceDatas = new List<SentenceData>();
    private int m_sentenceIndex;
    private float m_currentTransTime;
    private bool m_haveFinishDialogue;
    private bool m_isSkippable;
    private bool m_isClickedOnce;
    private bool m_canClickAgain;
    private TextType m_textType;


    public void StartDialogue(DialogueData dialogueData)
    {
        StartCoroutine(StartDialogueCoroutine(dialogueData));
    }

    private IEnumerator StartDialogueCoroutine(DialogueData dialogueData)
    {
        m_haveFinishDialogue = false;
        m_sentenceIndex = 0;
        m_textType = dialogueData.textType;
        yield return new WaitForEndOfFrame();
        m_sentenceDatas.Clear();
        m_currentDialogueData = dialogueData;
        foreach (SentenceData sentenceData in dialogueData.sentenceDatas)
        {
            m_sentenceDatas.Add(sentenceData);
        }
        ShowDialogue();
        //DisplayNextDialogue();
        ComputeSentences();


    }

    private void ShowDialogue()
    {
        CheckStartDisplayType();
    }

    private void ComputeSentences()
    {
        Debug.Log(m_sentenceIndex);
        SentenceData currentSentenceData = m_sentenceDatas[m_sentenceIndex];
        m_dialogueText.text = currentSentenceData.sentence;

        m_nextButtonObj.SetActive(m_sentenceIndex != m_sentenceDatas.Count - 1);

        if (m_textType == DialogueData.TextType.dialogue)
        {
            m_talkerImage.gameObject.SetActive(true);
            m_talkerNameText.transform.parent.gameObject.SetActive(true);
            m_talkerImage.sprite = currentSentenceData.NPCSprite;
            m_talkerNameText.text = currentSentenceData.NPCName;
        }
        else if (m_textType == DialogueData.TextType.text)
        {
            m_talkerImage.gameObject.SetActive(false);
            m_talkerNameText.gameObject.SetActive(false);
        }

        StopAllCoroutines();

        switch (m_currentDialogueData.sentenceDisplayStyle)
        {
            case DialogueData.SentenceDisplayStyle.direct:
                m_dialogueText.text = currentSentenceData.sentence;
                break;

            case DialogueData.SentenceDisplayStyle.type:
                StartCoroutine(TypeSentence(currentSentenceData));
                break;

            default:
                Debug.LogError("Error !");
                break;
        }
    }

    public void DisplayNextDialogue()
    {
        if (m_canClickAgain == false)
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

            StartCoroutine(ClickCooldownCoroutine());
        }

    }

    IEnumerator ClickCooldownCoroutine()
    {
        m_canClickAgain = false;
        yield return new WaitForSeconds(m_clickCooldown);
        m_canClickAgain = true;
    }

    private void EndDialogue()
    {
        CheckEndDisplayType();
        m_haveFinishDialogue = true;
    }

    IEnumerator TypeSentence(SentenceData currentSentenceData)
    {
        currentSentenceData.typingIsComplete = false;
        m_isSkippable = currentSentenceData.isSkippable;
        m_dialogueText.text = "";
        if (!m_isSkippable)
        {
            m_nextButtonObj.SetActive(false);
        }
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
        currentSentenceData.typingIsComplete = true;
        m_nextButtonObj.SetActive(m_sentenceIndex != m_sentenceDatas.Count - 1);
    }

    #region Dialogue Transition in
    private void CheckStartDisplayType()
    {
        DialogueData.DialogueStartDisplayStyle textBoxDisplayStyle = m_currentDialogueData.dialogueStartDisplayStyle;
        m_currentTransTime = m_currentDialogueData.transitionStartTime;
        switch (textBoxDisplayStyle)
        {
            case DialogueData.DialogueStartDisplayStyle.direct:
                m_dialogueContent.SetActive(true);
                m_nextButtonObj.SetActive(m_sentenceIndex != m_sentenceDatas.Count - 1);
                break;

            case DialogueData.DialogueStartDisplayStyle.fade:
                DialogueFadeIn(m_currentTransTime);
                break;

            case DialogueData.DialogueStartDisplayStyle.zoomIn:
                DialogueZoomIn(m_currentTransTime);
                break;

            case DialogueData.DialogueStartDisplayStyle.translation:
                DialogueSwipIn(m_currentTransTime);
                break;
        }
    }

    private void DialogueFadeIn(float transitionDuration)
    {
        m_dialogueContent.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(m_dialogueContent.GetComponent<Image>().DOFade(1, transitionDuration).SetEase(m_fadeCurve));
        sequence.Join(m_dialogueText.GetComponent<TMP_Text>().DOFade(1, transitionDuration).SetEase(m_fadeCurve));
        if (m_textType == DialogueData.TextType.dialogue)
        {
            sequence.Join(m_talkerImage.GetComponent<Image>().DOFade(1, transitionDuration).SetEase(m_fadeCurve));
            sequence.Join(m_talkerNameText.GetComponent<TMP_Text>().DOFade(1, transitionDuration).SetEase(m_fadeCurve));
        }
        sequence.OnComplete(() =>
        {
            m_nextButtonObj.SetActive(m_sentenceIndex != m_sentenceDatas.Count - 1);
        });
        sequence.Play();
    }

    private void DialogueZoomIn(float transitionDuration)
    {
        Vector2 originPos = m_dialogueContent.GetComponent<RectTransform>().localScale;
        m_dialogueContent.GetComponent<RectTransform>().localScale = new Vector2(0, 0);
        m_dialogueContent.SetActive(true);
        showNPCImage(true);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(m_dialogueContent.GetComponent<RectTransform>().DOScale(originPos, transitionDuration).SetEase(m_fadeCurve));
        sequence.OnComplete(() =>
        {
            m_nextButtonObj.SetActive(m_sentenceIndex != m_sentenceDatas.Count - 1);
        });
        sequence.Play();
    }

    private void DialogueSwipIn(float transitionDuration)
    {
        DialogueData.SwipInDirection swipInDirection = m_currentDialogueData.swipInDirection;
        m_dialogueContent.SetActive(true);
        showNPCImage(true);

        Vector2 originPos;
        originPos.y = m_dialogueContent.transform.position.y;
        originPos.x = m_dialogueContent.transform.position.x;
        Vector2 tempPos = originPos;

        switch (swipInDirection)
        {
            /*
            case DialogueData.SwipInDirection.fromUp:
                tempPos.y += Screen.height;
                break;
            */
            case DialogueData.SwipInDirection.fromDown:
                tempPos.y -= (Screen.height / 2) + tempPos.y / 2;
                break;

            case DialogueData.SwipInDirection.fromLeft:
                tempPos.x -= Screen.width;
                break;

            case DialogueData.SwipInDirection.fromRight:
                tempPos.x += Screen.width;
                break;
        }
        m_dialogueContent.transform.position = tempPos;
        Sequence sequence = DOTween.Sequence();
        if (/*swipInDirection == DialogueData.SwipInDirection.fromUp || */ swipInDirection == DialogueData.SwipInDirection.fromDown)
        {
            sequence.Append(m_dialogueContent.transform.DOMoveY(originPos.y, transitionDuration).SetEase(m_swipeCurve));
        }
        else if (swipInDirection == DialogueData.SwipInDirection.fromLeft || swipInDirection == DialogueData.SwipInDirection.fromRight)
        {
            sequence.Append(m_dialogueContent.transform.DOMoveX(originPos.x, transitionDuration).SetEase(m_swipeCurve));
        }
        sequence.OnComplete(() =>
        {
            m_nextButtonObj.SetActive(m_sentenceIndex != m_sentenceDatas.Count - 1);
        });
        sequence.Play();
    }
    #endregion

    #region Dialogue Transition out
    private void CheckEndDisplayType()
    {
        DialogueData.DialogueEndDisplayStyle textBoxDisplayStyle = m_currentDialogueData.dialogueEndDisplayStyle;
        m_currentTransTime = m_currentDialogueData.transitionEndTime;
        switch (textBoxDisplayStyle)
        {
            case DialogueData.DialogueEndDisplayStyle.direct:
                m_dialogueContent.SetActive(false);
                m_nextButtonObj.SetActive(false);
                m_haveFinishDialogue = true;
                break;

            case DialogueData.DialogueEndDisplayStyle.fade:
                DialogueFadeOut(m_currentTransTime);
                break;

            case DialogueData.DialogueEndDisplayStyle.zoomOut:
                DialogueZoomOut(m_currentTransTime);
                break;

            case DialogueData.DialogueEndDisplayStyle.translation:
                DialogueSwipOut(m_currentTransTime);
                break;

        }
    }

    private void DialogueFadeOut(float transitionDuration)
    {
        Sequence sequence = DOTween.Sequence();

        m_nextButtonObj.SetActive(false);
        sequence.Append(m_dialogueContent.GetComponent<Image>().DOFade(0, transitionDuration).SetEase(m_fadeCurve));
        sequence.Join(m_dialogueText.GetComponent<TMP_Text>().DOFade(0, transitionDuration).SetEase(m_fadeCurve));
        if (m_textType == DialogueData.TextType.dialogue)
        {
            sequence.Join(m_talkerImage.GetComponent<Image>().DOFade(0, transitionDuration).SetEase(m_fadeCurve));
            sequence.Join(m_talkerNameText.GetComponent<TMP_Text>().DOFade(0, transitionDuration).SetEase(m_fadeCurve));
        }

        sequence.OnComplete(() =>
        {
            //setImageColorAlpha(m_dialogueContent, 1);
            //setTextColorAlpha(m_dialogueText, 1);
            //if (m_textType == DialogueData.TextType.dialogue)
            //{
            //    setImageColorAlpha(m_talkerImage.gameObject, 1);
            //    setTextColorAlpha(m_talkerNameText, 1);
            //}
            m_dialogueContent.SetActive(false);
            m_haveFinishDialogue = true;
        });

        sequence.Play();
    }

    private void DialogueZoomOut(float transitionDuration)
    {
        Vector2 originPos = m_dialogueContent.GetComponent<RectTransform>().localScale;

        m_dialogueContent.SetActive(true);
        showNPCImage(true);
        Sequence sequence = DOTween.Sequence();

        sequence.Append(m_dialogueContent.GetComponent<RectTransform>().DOScale(new Vector2(0, 0), transitionDuration).SetEase(m_fadeCurve));
        sequence.OnComplete(() =>
        {
            m_dialogueContent.SetActive(false);
            m_dialogueContent.GetComponent<RectTransform>().localScale = originPos;
            m_haveFinishDialogue = true;
        });
        sequence.Play();
    }

    private void DialogueSwipOut(float transitionDuration)
    {
        DialogueData.SwipOutDirection swipOutDirection = m_currentDialogueData.swipOutDirection;
        Vector2 originPos;
        Vector2 targetPos;

        originPos.y = m_dialogueContent.transform.position.y;
        originPos.x = m_dialogueContent.transform.position.x;
        targetPos = originPos;

        switch (swipOutDirection)
        {
            /*
            case DialogueData.SwipOutDirection.toUp:
                targetPos.y += Screen.height;
                break;
            */

            case DialogueData.SwipOutDirection.toDown:
                targetPos.y -= (Screen.height / 2) + targetPos.y / 2;
                break;


            case DialogueData.SwipOutDirection.toLeft:
                targetPos.x -= Screen.width;
                break;


            case DialogueData.SwipOutDirection.toRight:
                targetPos.x += Screen.width;
                break;
        }
        if (swipOutDirection == DialogueData.SwipOutDirection.toDown)
        {
            m_dialogueContent.transform.DOMoveY(targetPos.y, transitionDuration).SetEase(m_swipeCurve).OnComplete(() =>
            {
                m_dialogueContent.transform.position = originPos;
                m_dialogueContent.SetActive(false);
                m_haveFinishDialogue = true;
            });
        }
        else if (swipOutDirection == DialogueData.SwipOutDirection.toLeft || swipOutDirection == DialogueData.SwipOutDirection.toRight)
        {
            m_dialogueContent.transform.DOMoveX(targetPos.x, transitionDuration).SetEase(m_swipeCurve).OnComplete(() =>
            {
                m_dialogueContent.transform.position = originPos;
                m_dialogueContent.SetActive(false);
                m_haveFinishDialogue = true;
            });
        }
    }
    #endregion

    private void showNPCImage(bool isShown)
    {
        if (m_textType == DialogueData.TextType.dialogue)
        {
            m_talkerImage.gameObject.SetActive(isShown);
            //npcNameGO.SetActive(isShown);
            m_talkerNameText.gameObject.SetActive(isShown);
        }
    }

}
