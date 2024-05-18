using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class MailManager : Singleton<MailManager>
{
    [SerializeField]
    private string folderPath = "Assets/Resources/Chat/Text";
    private Dictionary<string, string> mailData = new();

    public Transform mailList;
    public TMP_Text mailPanel;
    public GameObject mailPrefab;
    public float panelSize = 60;

    
    void Awake()
    {
        // 메일 컨텍스트 초기화
        mailList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        
        // 폴더에 있는 모든 .txt 파일
        string[] fileNames = Directory.GetFiles($"{folderPath}/day {GameSystem.Instance.gameData.date}", "*.txt");

        // 각 파일 이름을 순회하며 파일 제목과 내용 읽기
        foreach (string fileName in fileNames)
        {
            string fileTitle = Path.GetFileNameWithoutExtension(fileName); // 파일 제목 추출
            string fileContent = File.ReadAllText(fileName); // 파일 내용 읽음

            // 읽어온 파일 제목과 내용 저장
            mailData.Add(fileTitle, fileContent);

            GameObject newObject = Instantiate(mailPrefab, mailList);
            newObject.GetComponent<MailPanel>().Set(fileTitle);

            mailList.GetComponent<RectTransform>().sizeDelta += new Vector2(0, panelSize);
        }
    }


    public void ActiveMail(string title)
    {
        mailPanel.text = mailData[title];
        mailPanel.gameObject.SetActive(true);
    }


    public void OffMail()
    {
        mailPanel.text = "";
        mailPanel.gameObject.SetActive(false);
    }
}
