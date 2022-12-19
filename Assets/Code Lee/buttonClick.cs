using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (gameObject.CompareTag("RightButton"))
        {
            Debug.Log("Click!");
            GameObject.FindWithTag("MainCamera").GetComponent<CameraMove>().moveRight();
        }
        if (gameObject.CompareTag("LeftButton"))
        {
            GameObject.FindWithTag("MainCamera").GetComponent<CameraMove>().moveLeft();
        }
        if (gameObject.CompareTag("ForwardButton"))
        {
            GameObject.FindWithTag("MainCamera").GetComponent<CameraMove>().moveForward();
        }
        if (gameObject.CompareTag("BackButton"))
        {
            GameObject.FindWithTag("MainCamera").GetComponent<CameraMove>().moveBack();
        }
        if (gameObject.CompareTag("Screen"))
        {
            SceneManager.LoadScene("Screen");
        }
    }
}
