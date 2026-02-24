using UnityEngine;

public class TutorialHighlightConfig : MonoBehaviour
{
    [Header("Shape Mode")]
    public bool isSquare = false;
    public bool isCircle = false;

    [Header("Square Size")]
    public float sWidth = 200f;
    public float sHeight = 200f;

    [Header("Circle Size")]
    public float cRadius = 200f;

    // 'OnValidate'는 Inspector 창에서 스크립트의 속성(프로퍼티 값)이 수정될 때마다 호출되는 함수
    private void OnValidate()
    {
        if (isSquare && isCircle)
        {
            isCircle = false;
        }
    }

    public HighlightShape GetShape()
    {
        if (isCircle) return HighlightShape.Circle;
        if (isSquare) return HighlightShape.Spuare;
        return HighlightShape.None;
    }

}
