using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class StagePreview : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text desciptionText;
    public RectTransform panelRect;

    public float slideDuration = 1f;
    private levelSelect currentlySelectedLevel;
    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;
    private Coroutine slideRoutine;

    private void Start()
    {
        float panelWidth = panelRect.rect.width;
        visiblePosition = Vector2.zero;
        hiddenPosition = new Vector2(Screen.width, 0);
        panelRect.anchoredPosition = hiddenPosition;
    }

    public void OpenPreview(levelSelect level)
    {
        Debug.Log("preview opening");
        currentlySelectedLevel = level;

        titleText.text = level.stageID;
        desciptionText.text = level.stageDesciption;
        StartSlide(visiblePosition);

    }

    public void OnAcceptClicked()
    {
       if(currentlySelectedLevel != null)
        {
            currentlySelectedLevel.ConfirmLoadStage();
        }
    }

    public void OnDeclineClicked()
    {
        
        currentlySelectedLevel = null;
        StartSlide(hiddenPosition);
    }

    private void StartSlide(Vector2 targetPos)
    {
        if(slideRoutine != null)
        {
            StopCoroutine(slideRoutine);
        }
        slideRoutine = StartCoroutine(SlideTo(targetPos));
    }

    private IEnumerator SlideTo(Vector2 target)
    {
        Vector2 startPos = panelRect.anchoredPosition;
        float time = 0;
        while(time < slideDuration)
        {
            panelRect.anchoredPosition = Vector2.Lerp(startPos, target, time / slideDuration);
            time += Time.deltaTime;
            yield return null;
        }
        panelRect.anchoredPosition = target;
        slideRoutine = null;
    }
}
