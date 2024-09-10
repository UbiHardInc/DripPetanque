using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponsivePixelPerUnitSlicedImage : MonoBehaviour
{
    //To get the resolution factor, multiply your actual ppu that fits you with your actual width resolution
    public int resolutionMultPPUFactor = 1152;

    private Image imageToApply;
    // Start is called before the first frame update
    void Start()
    {
        imageToApply = this.GetComponent<Image>();
        //Debug.LogError("Screen width : " + Screen.width);
        float ppum = (float)resolutionMultPPUFactor / (float)Screen.width;
        //Debug.LogError("ppum = " + ppum);
        imageToApply.pixelsPerUnitMultiplier = ppum;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
