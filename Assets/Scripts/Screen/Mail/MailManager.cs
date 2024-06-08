using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class MailManager : Singleton<MailManager>
{
    [SerializeField]
    private string folderPath = "/Resources/Chat/Text";
    private Dictionary<string, string> mailData = new();

    public Transform mailList;
    public TMP_Text mailPanel;
    public GameObject mailPrefab;
    public float panelSize = 60;

    
    new void Awake()
    {
        base.Awake();
        // 메일 컨텍스트 초기화
        mailList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        
        // 폴더에 있는 모든 .txt 파일
        string[] fileNames = Directory.GetFiles($"{Application.dataPath}{folderPath}/day {GameSystem.Instance.gameData.date}", "*.txt");

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


    /// <summary>
    /// 메일 보기 (활성화)
    /// </summary>
    /// <param name="title">보여줄 메일 제목</param>
    public void ActiveMail(string title)
    {
        mailPanel.text = mailData[title];
        mailPanel.gameObject.SetActive(true);
    }


    /// <summary>
    /// 메일 비활성화
    /// </summary>
    public void OffMail()
    {
        mailPanel.text = "";
        mailPanel.gameObject.SetActive(false);
    }
}
