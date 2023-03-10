using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnimOrderController : MonoBehaviour
{
    /**
    *   애니메이션을 배열 순서대로 실행
    */
    public AnimBase[] anims;
    public GameObject Power;
    // Use this for initialization

    public void AnimStart()
    {
        StartCoroutine(PlayAnimInOrder());
    }

    IEnumerator PlayAnimInOrder()
    {
        foreach (AnimBase iter in anims)
        {
            yield return StartCoroutine(iter.PlayAnim());
        }
        Power.GetComponent<PowerOn>().TopInfoOff();

    }
}
