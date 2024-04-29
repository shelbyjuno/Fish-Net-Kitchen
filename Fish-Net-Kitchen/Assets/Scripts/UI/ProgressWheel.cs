using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressWheel : MonoBehaviour
{
    [Header("References")]
    public Image progressWheel;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public float lerpSpeed = 5;

    private float targetProgress;

    public void SetProgress(float progress, bool invert = false)
    {
        if(float.IsNaN(progress)) progress = 0;

        if(invert) progress = 1 - progress;
        targetProgress = progress;
    }

    public void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1 : 0;
    }

    void Update()
    {
        progressWheel.fillAmount = Mathf.Lerp(progressWheel.fillAmount, targetProgress, lerpSpeed * Time.deltaTime);
    }
}
