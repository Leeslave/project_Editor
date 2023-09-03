using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObject_Obs : MonoBehaviour
{
    public void StageOn()
    {
        gameObject.SetActive(true);
    }
    public void StageOff()
    {
        gameObject.SetActive(false);
    }
}
