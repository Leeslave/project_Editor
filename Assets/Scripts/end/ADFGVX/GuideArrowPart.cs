using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrowPart : MonoBehaviour
{
    private SpriteRenderer guideArrow_0;
    private SpriteRenderer guideArrow_1;
    private SpriteRenderer guideArrow_2;

    void Start()
    {
        guideArrow_0 = transform.Find("GuideArrow").GetComponent<SpriteRenderer>();
        guideArrow_1 = transform.Find("GuideArrow (1)").GetComponent<SpriteRenderer>();
        guideArrow_2 = transform.Find("GuideArrow (2)").GetComponent<SpriteRenderer>();
    }

    public void Rotate180()
    {
        guideArrow_0.transform.Rotate(new Vector3(0, 0, 180));
        guideArrow_1.transform.Rotate(new Vector3(0, 0, 180));
        guideArrow_2.transform.Rotate(new Vector3(0, 0, 180));
    }
}
