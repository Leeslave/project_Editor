using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class MakeLine : MonoBehaviour
{
    public GameObject Dot;
    public GameObject Res;

    List<GameObject> DotList = new List<GameObject>();
    float LineTime = 0.05f;
    Vector3 middle;

    Vector2 UIPosition(GameObject a) { return a.GetComponent<RectTransform>().anchoredPosition; }
    Vector2 UISize(GameObject a) { return a.GetComponent<RectTransform>().sizeDelta; }
    void ChangeUIPosition(ref GameObject a, Vector3 l) { a.GetComponent<RectTransform>().anchoredPosition = l; }

    public void EndLine()
    {
        foreach (var a in DotList) Destroy(a);
    }
   
    public void DrawDotLine(GameObject a1, GameObject a2)
    {

    }

    void DrawDot(Vector3 Start, Vector3 End, Vector2 SizeOfStart)
    {
        float IntervalX = UISize(Dot).x;
        if (Start.x > End.x) StartCoroutine(DrawDotLineX(new Vector3(Start.x - (SizeOfStart.x / 2 + IntervalX),Start.y,Start.z) , IntervalX * (-2) , End));
        else StartCoroutine(DrawDotLineX(new Vector3(Start.x + (SizeOfStart.x / 2 + IntervalX), Start.y, Start.z), IntervalX * 2, End));
    }

    IEnumerator DrawLineY(Vector3 Pos, float Ch, Vector3 Goal)
    {
        if (Ch > 0) 
        {
            if (Pos.y > Goal.y)
            {
                yield return new WaitForSeconds(LineTime);
                GameObject tmp = Instantiate(Res, gameObject.transform); DotList.Add(tmp);
                ChangeUIPosition(ref tmp, new Vector3(Goal.x, Goal.y + Ch, Goal.z));
                yield break;
            }
        }
        else { if (Pos.y < Goal.y) yield break; }
        GameObject cnt = Instantiate(Dot, gameObject.transform);
        ChangeUIPosition(ref cnt, Pos);
        DotList.Add(cnt);
        yield return new WaitForSeconds(LineTime);
        StartCoroutine(DrawLineY(new Vector3(Pos.x, Pos.y + Ch, Pos.z), Ch, Goal));
    }
    
    IEnumerator DrawDotLineX(Vector3 Pos, float Ch, Vector3 Goal)
    {
        bool i = false;
        if (Ch > 0) { if (Pos.x > Goal.x) i = true; }
        else { if (Pos.x < Goal.x) i = true; }
        if (i)
        {
            yield return new WaitForSeconds(LineTime);
            Pos = new Vector3(Pos.x - Ch, Pos.y, Pos.z); if (Ch < 0) Ch *= -1;
            if (Pos.y > Goal.y) { Goal = new Vector3(Goal.x,Goal.y + Ch); StartCoroutine(DrawLineY(new Vector3(Pos.x, Pos.y - Ch, Pos.z), -Ch, Goal)); }
            else { Goal = new Vector3(Goal.x, Goal.y - Ch); StartCoroutine(DrawLineY(new Vector3(Pos.x, Pos.y + Ch, Pos.z), Ch, Goal)); }
            yield break;
        }
        GameObject cnt = Instantiate(Dot, gameObject.transform);
        DotList.Add(cnt);
        ChangeUIPosition(ref cnt, Pos);
        yield return new WaitForSeconds(LineTime);
        StartCoroutine(DrawDotLineX(new Vector3(Pos.x + Ch, Pos.y, Pos.z),Ch,Goal));
    }
}
