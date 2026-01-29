using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRingController : MonoBehaviour
{
    [SerializeField] private RectTransform ringCircle;
    [SerializeField] private RectTransform ringSquare;

    public void Apply(HighlightShape shape, Vector2 canvasPos, TutorialHighlightConfig config)
    {
        ringCircle.gameObject.SetActive(shape == HighlightShape.Circle);
        ringSquare.gameObject.SetActive(shape == HighlightShape.Spuare);

        if (shape == HighlightShape.Circle)
        {
            ringCircle.anchoredPosition = canvasPos;
            float d = config.cRadius * 2f;
            ringCircle.sizeDelta = new Vector2(d, d);
        }
        else if (shape == HighlightShape.Spuare)
        {
            ringSquare.anchoredPosition = canvasPos;
            ringSquare.sizeDelta = new Vector2(config.sWidth, config.sHeight);
        }
    }
}
