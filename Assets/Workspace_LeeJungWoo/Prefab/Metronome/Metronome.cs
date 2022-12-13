using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : SpineAnimation
{
    private bool flip = false;

    // Start is called before the first frame update
    void Start()
    {
        SetCurrentAnimation(AnimState.Idle,true,1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (skeletonAnimation.Skeleton.FindBone("bone3").Rotation > 90 && flip)
        {
            flip = false;
            GetComponent<AudioSource>().Play();
        }
        else if(skeletonAnimation.Skeleton.FindBone("bone3").Rotation < 90 && !flip)
        {
            flip = true;
            GetComponent<AudioSource>().Play();
        }
    }
}
