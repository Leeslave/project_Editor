
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Warnings_DG : MonoBehaviour
{
    Image IM;
    WaitForSeconds Sec = new WaitForSeconds(0.05f);
    Color s = new Color(0, 0, 0, 0.02f);

    private void Awake()
    {
        IM = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(EE());
    }

    IEnumerator EE()
    {
        for(int i = 0; i < 2; i++)
        {
            for (int x = 0; x < 10; x++) { IM.color -= s; yield return Sec; }
            for (int x = 0; x < 10; x++) { IM.color += s; yield return Sec; }
        }
        gameObject.SetActive(false);
    }
}
