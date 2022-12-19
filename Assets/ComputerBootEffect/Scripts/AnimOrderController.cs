using UnityEngine;
using System.Collections;

public class AnimOrderController : MonoBehaviour
{
    public AnimBase[] anims;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(AnimInOrder());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator AnimInOrder()
    {
        foreach (AnimBase a in anims)
        {
            yield return StartCoroutine(a.PlayAnim());
        }
    }
}
