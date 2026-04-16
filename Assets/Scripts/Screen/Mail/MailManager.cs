using GameService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks; // Task 사용을 위해 추가
using TMPro;
using UnityEngine;
using Utility;

[Serializable]
public struct MailInfo
{
    public string Title;
    public string Content;

    public MailInfo(string title, string content)
    {
        Title = title;
        Content = content;  
    }
}

public class MailManager : Singleton<MailManager>
{
    /**
     * 메일 파일 관리
     * - 메일 파일 로드
     * - 날짜에 따라 해당 메일 추가
     */
    private IDayService _dayService;
    
    [SerializeField] private string mailDataFolderName = "MailData";
    [SerializeField] private List<MailInfo> mailData = new();

    public Transform mailList;
    public TMP_Text mailPanel;
    public GameObject mailPrefab;
    public float panelSize = 60;

    async void Start()
    {
        _dayService = ServiceProvider.Get<IDayService>();
    
        // UI 초기화
        if (mailList)
        {
            mailList.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        }
        await LoadMailAsync();
    }

    private async Task LoadMailAsync()
    {
        // StreamingAssets/MailData/Day 1
        string targetFolderPath = Path.Combine(Application.streamingAssetsPath, mailDataFolderName, $"Day {_dayService.Day}");

        if (!Directory.Exists(targetFolderPath))
        {
            return;
        }

        try 
        {
            string[] fileNames = await Task.Run(() => Directory.GetFiles(targetFolderPath, "*.txt"));

            foreach (string fileName in fileNames)
            {
                string fileTitle = Path.GetFileNameWithoutExtension(fileName);
                
                // 파일 내용 비동기 읽기
                string fileContent = await File.ReadAllTextAsync(fileName);
                
                // 데이터 저장
                MailInfo newMail = new(fileTitle, fileContent);
                mailData.Add(newMail);

                // 4. UI 생성
                CreateMailUI(fileTitle);
            }
        }
        catch (Exception e)
        {
            EditorLogger.LogError($"메일 로드 중 오류 발생: {e.Message}");
        }
    }

    private async Task LoadAllMailAsync()
    {
        // day 0일차부터 순회
        for (int i = 0; i <= _dayService.Day; i++)
        {
            // PATH: StreamingAssets/MailData/Day ?
            string targetFolderPath = Path.Combine(Application.streamingAssetsPath, mailDataFolderName, $"Day {i}");

            // No Mail in Day
            if (!Directory.Exists(targetFolderPath))
            {
                continue; 
            }

            try 
            {
                // 해당 날짜 폴더 안의 모든 파일 로드
                string[] fileNames = await Task.Run(() => Directory.GetFiles(targetFolderPath, "*.txt"));

                foreach (string fileName in fileNames)
                {
                    string fileTitle = Path.GetFileNameWithoutExtension(fileName);
                
                    // 비동기로 텍스트 로드
                    string fileContent = await File.ReadAllTextAsync(fileName);
                
                    // 데이터 저장
                    MailInfo newMail = new(fileTitle, fileContent);
                    mailData.Add(newMail);

                    // 4. UI 생성
                    CreateMailUI(fileTitle);
                }
            }
            catch (Exception e)
            {
                EditorLogger.LogError($"{i}일차 메일을 읽는 중 오류 발생: {e.Message}");
            }
        }
    }

    private void CreateMailUI(string title)
    {
        GameObject newObject = Instantiate(mailPrefab, mailList);
        
        if (newObject.TryGetComponent<MailPanel>(out var panel))
        {
            panel.Set(title);
        }

        // 레이아웃 크기 조정 (VerticalLayoutGroup 미사용 시 유지)
        mailList.GetComponent<RectTransform>().sizeDelta += new Vector2(0, panelSize);
    }

    public void ActiveMail(string title)
    {
        // Find 사용 시 데이터가 없으면 예외가 날 수 있으므로 체크
        var target = mailData.Find(x => x.Title == title);
        if (string.IsNullOrEmpty(target.Title)) return;

        mailPanel.text = target.Content;
        mailPanel.gameObject.SetActive(true);
    }

    public void OffMail()
    {
        mailPanel.text = "";
        mailPanel.gameObject.SetActive(false);
    }
}