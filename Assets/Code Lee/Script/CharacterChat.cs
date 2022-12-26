using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChat : MonoBehaviour
{
    public GameObject ChatUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        ChatUI.GetComponent<ChatUI>().Start0Day();
    }
}
