using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class UIVideoPlayer : MonoBehaviour
{
    public VideoPlayer[] videoPlayers;
    public TMP_Text[] text;
    public GameObject panel;
    public Button nextButton;
    public Button prevButton;

    [SerializeField] private int videoIndex;

    public void ClickButton() 
    { 
        panel.SetActive(true);
        nextButton.gameObject.SetActive(true);
        videoIndex = 0;
        videoPlayers[videoIndex].enabled = true;
        text[videoIndex].enabled = true;
    }
    
    public void NextVideoPlay()
    {
        videoPlayers[videoIndex].enabled = false;
        text[videoIndex].enabled = false;
        videoIndex++;
        videoPlayers[videoIndex].enabled = true;
        text[videoIndex].enabled = true;
        videoPlayers[videoIndex].Play();
        if (videoIndex == 1)
        {
            prevButton.gameObject.SetActive(true);
        }
        if (videoIndex == videoPlayers.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
        }
    }

    public void PrevVideoPlay()
    {
        videoPlayers[videoIndex].enabled = false;
        text[videoIndex].enabled = false;
        videoIndex--;
        videoPlayers[videoIndex].enabled = true;
        text[videoIndex].enabled = true;
        videoPlayers[videoIndex].Play();

        if (videoIndex == 0)
        {
            prevButton.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        panel.SetActive(false);
        videoIndex = 0;
    }

    
}
