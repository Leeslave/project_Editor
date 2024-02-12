using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine_P : MonoBehaviour
{
    private void Update()
    {
        Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); MousePos.z = 0;

    }
}
