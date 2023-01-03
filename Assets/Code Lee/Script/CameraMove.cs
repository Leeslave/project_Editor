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


    // Start is called before the first frame update
    void Start()
    {
        //gameObject.transform.position = new Vector3(cameraX, cameraY, cameraZ);
        //DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void moveToScreen()
    {
        saveX = gameObject.transform.position.x;
        saveY = gameObject.transform.position.y;

        gameObject.transform.position = new Vector3(1000, 1000, cameraZ);
    }

    public void moveToMain()
    {
        gameObject.transform.position = new Vector3(saveX, saveY, cameraZ);
    }

    public void moveToGameScene()
    {
        gameObject.SetActive(false);
    }

    public void moveToMainScene()
    {
        gameObject.SetActive(true);
    }
}
