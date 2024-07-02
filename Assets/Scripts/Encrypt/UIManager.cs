using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject hintCanvas;
    
    void Start() 
    { 
        canvas.SetActive(false);
        hintCanvas.SetActive(false);
    }

    public void ActivateUI() => canvas.SetActive(true);
    public void InactivateUI() => canvas.SetActive(false);
    public void ActivateHintUI() => hintCanvas.SetActive(true);
    public void InactivateHintUI() => hintCanvas.SetActive(false);
}
