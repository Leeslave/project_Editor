using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneSetting : MonoBehaviour
{
    float cameraX = 5000;
    float cameraY = 5000;
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
}
