using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonClick : MonoBehaviour
{
    public GameObject MainCamera;
    public GameObject Screen;

    private void OnMouseDown()
    {
        if (gameObject.CompareTag("RightButton"))
        {
            Debug.Log("Click!");
            MainCamera.GetComponent<CameraMove>().moveRight();
        }
        if (gameObject.CompareTag("LeftButton"))
        {
            MainCamera.GetComponent<CameraMove>().moveLeft();
        }
        if (gameObject.CompareTag("ForwardButton"))
        {
            MainCamera.GetComponent<CameraMove>().moveForward();
        }
        if (gameObject.CompareTag("BackButton"))
        {
            MainCamera.GetComponent<CameraMove>().moveBack();
        }
        if (gameObject.CompareTag("Screen"))
        {
            MainCamera.GetComponent<CameraMove>().moveToScreen();
            Screen.GetComponent<UIObjectManagement>().ObjectOnOff();
        }
    }
}
