using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    float cameraX = 0;
    float cameraY = 0;
    float cameraZ = -1;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(cameraX, cameraY, cameraZ);
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
}
