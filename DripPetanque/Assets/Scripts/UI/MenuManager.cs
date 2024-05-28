using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("MainMenuVariables")]
    [SerializeField] private float timeForMenuToAppear = 6.30f;
    [Header("MainMenuObjects")]
    [SerializeField] private GameObject transitionImage;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IntroMainMenu());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator IntroMainMenu()
    {
        yield return new WaitForSeconds(timeForMenuToAppear);
        StartCoroutine(FadeInAndOutGameObject.FadeInAndOut(transitionImage, false, 1f));
    }
}
