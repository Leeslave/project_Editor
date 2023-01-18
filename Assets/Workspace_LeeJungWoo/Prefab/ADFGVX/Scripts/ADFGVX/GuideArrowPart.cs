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
        guideArrow_0 = GetComponentsInChildren<SpriteRenderer>()[0];
        guideArrow_1 = GetComponentsInChildren<SpriteRenderer>()[1];
        guideArrow_2 = GetComponentsInChildren<SpriteRenderer>()[2];
    }

    public void Rotate180()
    {
        guideArrow_0.transform.Rotate(new Vector3(0,0,180));
        guideArrow_1.transform.Rotate(new Vector3(0, 0, 180));
        guideArrow_2.transform.Rotate(new Vector3(0, 0, 180));
    }
}
