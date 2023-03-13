using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_VoightKampff : Chat
{
    [Header("게임 매니저")]
    public GameManager_VoightKampff GameManager;

    protected override void SetLayerDefault()
    {
        GameManager.SetLayer(0, 0, 0, 0, 0, 0, 0);
    }

    public override void LoadLine(int line)
    {
        base.LoadLine(line);

        Dictionary<string, object> currentLineData = data[line - 1];
        string react = currentLineData["React"].ToString();

        switch(react)
        {
            case "surprise":
                GameManager.GetEye().Surprise();
                break;
            case "star":
                GameManager.GetEye().DrawStar();
                break;
            case "c":
                GameManager.GetEye().DrawCSign();
                break;
            case "f":
                GameManager.GetEye().DrawFSign();
                break;
            case "n":
                GameManager.GetEye().DrawNSign();
                break;
            case "v":
                GameManager.GetEye().DrawVSign();
                break;
            case "z":
                GameManager.GetEye().DrawZSign();
                break;
        }
    }
}
