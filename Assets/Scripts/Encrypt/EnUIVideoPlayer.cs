using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class EnUIVideoPlayer : MonoBehaviour
{
    public VideoPlayer[] videoPlayers;
    public TMP_Text text;
    public string[] savedText;

    public GameObject panel;
    public GameObject anotherPanel;
    public GameObject anotherButton;
    public Button nextButton;
    public Button prevButton;
    public Button anotherNextButton;
    public Button anotherPrevButton;

    public bool isActive = false;
    [SerializeField] private int videoIndex;

    public void UnActicveSelf()
    {
        videoPlayers[videoIndex].enabled = false;
        panel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        prevButton.gameObject.SetActive(false);
        videoIndex= 0;
    }

    public void CheckStatus()
    {
        anotherButton.GetComponent<UIVideoPlayer>().UnActicveSelf();
        return;
    }
    public void ClickButton() 
    {
        videoIndex = 0;
        CheckStatus();
        Debug.Log("1234");
        anotherPanel.SetActive(false);
        panel.SetActive(true);
        nextButton.gameObject.SetActive(true);
        videoPlayers[videoIndex].enabled = true;
        text.text = savedText[videoIndex];
    }
    
    public void NextVideoPlay()
    {
        videoIndex++;
        text.text = savedText[videoIndex];
        videoPlayers[videoIndex].enabled = true;
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
        videoIndex--;
        videoPlayers[videoIndex].enabled = true;
        text.text = savedText[videoIndex];
        videoPlayers[videoIndex].Play();
        if (videoIndex < videoPlayers.Length - 1)
        {
            nextButton.gameObject.SetActive(true);
        }
        if (videoIndex == 0)
        {
            prevButton.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        panel.SetActive(false);
        nextButton.gameObject.SetActive(false);
        prevButton.gameObject.SetActive(false);
        videoIndex = 0;
    }

    
}
