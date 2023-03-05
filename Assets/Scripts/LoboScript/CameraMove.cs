using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    float cameraX = 0;
    float cameraY = 0;
    float cameraZ = -1;

    float saveX = 0;
    float saveY = 0;

    /// 카메라 위치 옮기기 (왼오위아래) 25 단위 이동
    public void moveForward()
    {
        cameraY += 25;
        gameObject.transform.position = new Vector3(cameraX, cameraY, cameraZ);
    }

    public void moveBack()
    {
        Debug.Log("qwerrt");
        cameraY -= 25;
        gameObject.transform.position = new Vector3(cameraX, cameraY, cameraZ);
    }

    public void moveRight()
    {
        cameraX += 25;
        gameObject.transform.position = new Vector3(cameraX, cameraY, cameraZ);
    }

    public void moveLeft()
    {
        cameraX -= 25;
        gameObject.transform.position = new Vector3(cameraX, cameraY, cameraZ);
    }

    /// <summary>
    /// 스크린 위치로 카메라 이동
    /// save 좌표로 이전 좌표 기억
    /// </summary>
    public void moveToScreen()
    {
        saveX = gameObject.transform.position.x;
        saveY = gameObject.transform.position.y;

        gameObject.transform.position = new Vector3(1000, 1000, cameraZ);
    }

    /// <summary>
    /// 저장된 이전 위치로 카메라 이동
    /// save 좌표로 이전 좌표 불러오기
    /// </summary>
    public void moveToMain()
    {
        gameObject.transform.position = new Vector3(saveX, saveY, cameraZ);
    }

    /// <summary>
    /// 카메라 비활성화
    /// </summary>
    public void moveToGameScene()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 저장된 이전 위치로 카메라 이동
    /// save 좌표로 이전 좌표 불러오기
    /// </summary>
    public void moveToMainScene()
    {
        gameObject.SetActive(true);
    }
}
