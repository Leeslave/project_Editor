using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;


//Making Dot Line Between Two Object
//Anchor Must be Center
public class MakeLine : MonoBehaviour
{
    public GameObject Dot;
    public GameObject Res;
    public GameObject Submit;

    GameObject Line1;
    GameObject Line2;

    List<GameObject> DotList = new List<GameObject>();
    float LineTime = 0.05f;

    public void EndLine()
    {
        foreach (var a in DotList) Destroy(a);
        ChangeColor(Line1);
        ChangeColor(Line2);
    }
    void ChangeColor(GameObject a)
    {
        switch (a.tag)
        {
            case "ReportText":
                a.GetComponent<TMP_Text>().color = Color.black;
                break;
            default:
                for (int i = 0; i < a.transform.childCount; i++) a.transform.GetChild(i).GetComponent<TMP_Text>().color = Color.black;
                break;
        }
    }
   
    public Vector3 ParentPointCorrect(GameObject cnt)       // Correct Position Betweeen GameObject and Canvas
    {
        Vector3 LayOutCorrect = MyUi.UIPosition(cnt);
        if (cnt.tag == "Scroll") LayOutCorrect = new Vector3(LayOutCorrect.x, LayOutCorrect.y, LayOutCorrect.z);
        if (cnt.tag == "Canvas") return Vector3.zero;
        else return LayOutCorrect + ParentPointCorrect(cnt.transform.parent.gameObject);
    }

    public void DrawDotLine(GameObject a1, GameObject a2)
    {
        Line1 = a1; Line2 = a2;
        Vector3 Position1 = ParentPointCorrect(a1);
        Vector3 Position2 = ParentPointCorrect(a2);
        Vector3 mid = Vector3.zero;
        if (Position1.x > Position2.x) 
        mid = (new Vector3(Position1.x - MyUi.UISize(a1).x * 0.5f, Position1.y, Position1.z) + 
               new Vector3(Position2.x + MyUi.UISize(a2).x * 0.5f, Position2.y, Position2.z)) / 2;
        else
        mid = (new Vector3(Position2.x - MyUi.UISize(a2).x * 0.5f, Position2.y, Position2.z) + 
               new Vector3(Position1.x + MyUi.UISize(a1).x * 0.5f, Position1.y, Position1.z)) / 2;
        DrawDot(Position1, mid, MyUi.UISize(a1));
        DrawDot(Position2, mid, MyUi.UISize(a2));
    }

    void DrawDot(Vector3 Start, Vector3 End, Vector2 SizeOfStart)
    {
        float IntervalX = MyUi.UISize(Dot).x;
        if (Start.x > End.x) StartCoroutine(DrawDotLineX(new Vector3(Start.x - SizeOfStart.x * 0.5f - 50, Start.y, Start.z), IntervalX * (-2) , End));
        else StartCoroutine(DrawDotLineX(new Vector3(Start.x + SizeOfStart.x * 0.5f + 50, Start.y, Start.z), IntervalX * 2, End));
    }
    IEnumerator DrawLineY(Vector3 Pos, float Ch, Vector3 Goal)      // Draw Line of Y;
    {
        if (Ch > 0) 
        {
            if (Pos.y > Goal.y)
            {
                yield return new WaitForSeconds(LineTime);
                GameObject tmp = Instantiate(Res, gameObject.transform); DotList.Add(tmp);
                MyUi.ChangeUIPosition(ref tmp, new Vector3(Goal.x, Goal.y + Ch, Goal.z));
                Submit.SetActive(true);
                Debug.Log(Submit.name);
                yield break;
            }
        }
        else { if (Pos.y < Goal.y) yield break; }
        GameObject cnt = Instantiate(Dot, gameObject.transform);
        MyUi.ChangeUIPosition(ref cnt, Pos);
        DotList.Add(cnt);
        yield return new WaitForSeconds(LineTime);
        StartCoroutine(DrawLineY(new Vector3(Pos.x, Pos.y + Ch, Pos.z), Ch, Goal));
    }
    
    IEnumerator DrawDotLineX(Vector3 Pos, float Ch, Vector3 Goal)       // Draw Line of X
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
        MyUi.ChangeUIPosition(ref cnt, Pos);
        yield return new WaitForSeconds(LineTime);
        StartCoroutine(DrawDotLineX(new Vector3(Pos.x + Ch, Pos.y, Pos.z),Ch,Goal));
    }
}
