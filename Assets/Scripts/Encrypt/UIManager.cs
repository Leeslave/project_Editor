using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject hintCanvas;
    // Start is called before the first frame update
    void Start() 
    { 
        Canvas.SetActive(false);
        hintCanvas.SetActive(false);
    }

    public void ActivateUI() { Canvas.SetActive(true); }
    public void UnactivateUI() { Canvas.SetActive(false); }
    public void ActivateHintUI() { hintCanvas.SetActive(true); }
    public void UnactivateHintUI() { hintCanvas.SetActive(false); }
}
