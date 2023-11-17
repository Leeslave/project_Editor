using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideArrowPart : MonoBehaviour
{
    private GameObject guideArrow_0;
    private GameObject guideArrow_1;
    private GameObject guideArrow_2;

    void Awake()
    {
        guideArrow_0 = transform.GetChild(0).gameObject;
        guideArrow_1 = transform.GetChild(1).gameObject;
        guideArrow_2 = transform.GetChild(2).gameObject;
    }

    public void Rotate180()
    {
        guideArrow_0.transform.Rotate(new Vector3(0, 0, 180));
        guideArrow_1.transform.Rotate(new Vector3(0, 0, 180));
        guideArrow_2.transform.Rotate(new Vector3(0, 0, 180));
    }
}
