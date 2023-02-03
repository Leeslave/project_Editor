using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Note : MonoBehaviour
{
    private int beatPerMinute;

    protected TextMeshPro markText;
    protected bool onHit;
    protected int direction;

    private double currentTime;

    protected virtual void Start()
    {
        if (transform.Find("MarkText") != null)
            markText = transform.Find("MarkText").GetComponent<TextMeshPro>();

        currentTime = 0d;
        MoveNoteOnePos();

        onHit = false;
    }

    protected virtual void Update()
    {
        currentTime += Time.deltaTime;

        if(currentTime >= 60d / beatPerMinute)
        {
            MoveNoteOnePos();
            currentTime -= 60d / beatPerMinute;
        }
    }

    public void SetBPM(int value)
    {
        beatPerMinute = value;
    }

    public void MoveNoteOnePos()
    {
        Vector3 targetPos = new Vector3(0f, (direction * 20f), 0f) + transform.position;
        StartCoroutine(LerpNotePos(0, 60f / 280, targetPos));
    }

    private IEnumerator LerpNotePos(double currentTime, float endTime, Vector3 targetPos)
    {
        currentTime += endTime / 100;
        if (currentTime >= endTime)
            yield break;

        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, 0.05f);

        yield return new WaitForSeconds(endTime / 100);
        StartCoroutine(LerpNotePos(currentTime, endTime, targetPos));
    }

}
