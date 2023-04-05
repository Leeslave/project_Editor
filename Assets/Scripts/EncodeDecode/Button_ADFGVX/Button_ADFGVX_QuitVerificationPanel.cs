using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ADFGVX_QuitVerificationPanel : Button_ADFGVX
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        GameManager.verificationPart.UnvisiblePart();
        
        if(GameManager.PlayAsTutorial && GameManager.CurrentMode == ADFGVX.mode.Decoding)//복호화 튜토리얼 중이었음
        {
            if(GameManager.ReturnDecodeScore()) //복호화 튜토리얼 과정에 성공, 재시도 불필요
                GameManager.SetPartLayerWaitForSec(0f, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0);
            else                                //복호화 튜토리얼 과정에 실패, 재시도 필요
                GameManager.SetPartLayerWaitForSec(0f, 2, 0, 0, 0, 0, 2, 0, 2, 2, 2, 0, 0);
        }
        else if(GameManager.PlayAsTutorial && GameManager.CurrentMode == ADFGVX.mode.Encoding)//암호화 튜토리얼 중이었음
        {
            if(GameManager.ReturnEncodeScore()) //암호화 튜토리얼 과정에 성공, 재시도 불필요
                GameManager.SetPartLayerWaitForSec(0f, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0);
            else                                //암호화 튜토리얼 과정에 성공, 재시도 필요
                GameManager.SetPartLayerWaitForSec(0f, 2, 2, 2, 2, 2, 2, 2, 2, 0, 2, 0, 0);
        }
        else
            GameManager.SetPartLayerWaitForSec(0f, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }
}
