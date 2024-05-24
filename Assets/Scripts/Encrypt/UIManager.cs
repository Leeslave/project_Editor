using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject Canvas;
    // Start is called before the first frame update
    void Start() { Canvas.SetActive(false); }

    public void ActivateUI() { Canvas.SetActive(true); }
    public void UnactivateUI() { Canvas.SetActive(false); }
}
