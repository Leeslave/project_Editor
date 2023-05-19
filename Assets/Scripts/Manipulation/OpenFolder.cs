using System.Collections.Generic;
using UnityEngine;

public class OpenFolder : MonoBehaviour
{

    private List<string> Files = new List<string>();
    private void Awake()
    {
        MyUi.textReader("Manipulation"+name,ref Files);
    }

}
