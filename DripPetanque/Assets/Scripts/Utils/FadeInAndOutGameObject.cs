using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class FadeInAndOutGameObject
{
    public static IEnumerator FadeInAndOut(GameObject objectToFade, bool fadeIn, float duration)
    {
        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float startAlpha, targetAlpha;
        if (fadeIn)
        {
            startAlpha = 0;
            targetAlpha = 1;
        }
        else
        {
            startAlpha = 1;
            targetAlpha = 0;
        }


        Action<float> changeAlphaAction;

        //Check if this is a Sprite
        if (objectToFade.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            Color currentColor = spriteRenderer.color;
            changeAlphaAction = (float alpha) => spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
        //Check if Image
        else if (objectToFade.TryGetComponent(out Image image))
        {
            Color currentColor = image.color;
            changeAlphaAction = (float alpha) => image.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
        //Check if RawImage
        else if (objectToFade.TryGetComponent(out RawImage rawImage))
        {
            Color currentColor = rawImage.color;
            changeAlphaAction = (float alpha) => rawImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }
        //Check if Text 
        else if (objectToFade.TryGetComponent(out Text text))
        {
            Color currentColor = text.color;
            changeAlphaAction = (float alpha) => text.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
        }

        //Check if 3D Object
        else if (objectToFade.TryGetComponent(out MeshRenderer meshRenderer))
        {
            Color currentColor = meshRenderer.material.color;
            changeAlphaAction = (float alpha) => meshRenderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);

            //ENABLE FADE Mode on the material if not done already
            meshRenderer.material.SetFloat("_Mode", 2);
            meshRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            meshRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            meshRenderer.material.SetInt("_ZWrite", 0);
            meshRenderer.material.DisableKeyword("_ALPHATEST_ON");
            meshRenderer.material.EnableKeyword("_ALPHABLEND_ON");
            meshRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            meshRenderer.material.renderQueue = 3000;
        }
        else
        {
            yield break;
        }

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, counter / duration);

            changeAlphaAction(alpha);
            yield return null;
        }
    }
}