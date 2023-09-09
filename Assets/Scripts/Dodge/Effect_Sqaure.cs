using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Sqaure : MonoBehaviour
{
    Vector3 alpha = new Vector3(10, 7.5f, 0);
    Vector3 beta = new Vector3(70, 52.5f, 0);
    private void FixedUpdate()
    {
        transform.localScale -= alpha * Time.deltaTime * 3;
        transform.Rotate(Vector3.forward * Time.deltaTime * 10);
        if (transform.localScale.x <= 0.1f) transform.localScale = beta;
    }
}
