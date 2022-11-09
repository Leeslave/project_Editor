using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class EncodedChipertext : MonoBehaviour
{
    
    private void Awake()
    {
        ClearText();
    }

    public void UpdateText(int ChiperNum)
    {
        string FilePath = "";
        FileInfo TxtFile = null;
        string TxtValue = "";
        string TxtTitle = "";

        FilePath = "Assets/Workspace_LeeJungWoo/ADFGVX/ChipersTxt/Chiper_" + ChiperNum + ".txt";              //ArrayNum�� ���� ���� �ٸ� ǥ�� FilePath�� ����˴ϴ�
        TxtFile = new FileInfo(FilePath);
        if (TxtFile.Exists)                                                                                 //FilePath�� ��ȿ�ϴٸ�
        {
            StreamReader Reader = new StreamReader(FilePath, System.Text.Encoding.UTF8);
            TxtTitle = Reader.ReadLine();
            TxtValue = Reader.ReadToEnd();
            
            Reader.Close();
        }
        else
            Debug.Log("Unexist Filename!");
        GetComponentsInChildren<TextMeshPro>()[0].text = TxtTitle;
        GetComponentsInChildren<TextMeshPro>()[1].text = TxtValue;
    }

    public void ClearText()
    {
        GetComponentsInChildren<TextMeshPro>()[0].text = "";
        GetComponentsInChildren<TextMeshPro>()[1].text = "";
    }
}
