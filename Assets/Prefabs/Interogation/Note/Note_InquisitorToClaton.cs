using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note_InquisitorToClaton : Note
{

    protected override void Start()
    {
        base.Start();
        direction = 1;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (onHit)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ClatonPos")
        {
            onHit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "ClatonPos")
        {
            onHit = false;
            Destroy(this.gameObject, 3.0f);
        }
    }
}
