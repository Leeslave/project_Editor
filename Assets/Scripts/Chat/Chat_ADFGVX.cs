using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat_ADFGVX : Chat
{
    [Header("게임 매니저")]
    public ADFGVX GameManager;

    public override void LoadLine(int line)
    {
        GameManager.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2);
        base.LoadLine(line);
    }

    protected override void SetLayerAtEnd()
    {
        if(currentLineData["SetLayer"].ToString()==null)
            return;

        int[] layer = new int[12];
        string str = currentLineData["SetLayer"].ToString();
        for(int i=0;i<str.Length;i++)
        {
            if(str[i]=='d')
                layer[i] = 0;
            else if(str[i]=='c')
                layer[i] = 2;
        }
        GameManager.SetPartLayerWaitForSec(0f, layer[0], layer[1], layer[2], layer[3], layer[4], layer[5], layer[6], layer[7], layer[8], layer[9], layer[10], layer[11]);
    }
}
