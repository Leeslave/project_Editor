using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RealEnd : MonoBehaviour
{
    bool ch;

    [SerializeField] RectTransform Rotation;
    [SerializeField] Image image;
    [SerializeField] Sprite sprite;
    [SerializeField] Sprite sprite2;
    [SerializeField] TMP_Text text;

    // ����
    // Input
    public void Ending(bool Clear)
    {
        ch = Clear;
        StartCoroutine(EEE());
    }

    // ���� �� ����
    IEnumerator EEE()
    {
        string cnt;
        WaitForSeconds w1 = new WaitForSeconds(0.05f);
        for(int i = 0; i < 2; i++)
        {
            Rotation.Rotate(new Vector3(0, 0, -10));
            yield return w1;
            Rotation.Rotate(new Vector3(0, 0, -10));
            yield return w1;
            Rotation.Rotate(new Vector3(0, 0, 10));
            yield return w1;
            Rotation.Rotate(new Vector3(0, 0, 10));
            yield return w1;
        }
        yield return new WaitForSeconds(2);
        if (ch)
        {
            image.sprite = sprite;
            cnt = "Access Success";
        }
        else
        {
            image.sprite = sprite2;
            cnt = "Access Denied";
        }
        text.gameObject.SetActive(true);
        foreach(var a in cnt)
        {
            text.text += a;
            yield return w1;
        }
        // ���� ��ư ����
        transform.GetChild(3).gameObject.SetActive(true);

        // ���� ��ư�� ���� ��� �Ҵ�
        MyUi.AddEvent(transform.GetChild(3).gameObject.GetComponent<EventTrigger>(), EventTriggerType.PointerClick,Exit);

        yield break;
    }

    // ���� ��ư
    void Exit(PointerEventData a)
    {
        Debug.Log("!");
        Application.Quit();
    }
}
