using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//Making Dot Line Between Two Object
//Anchor Must be Center
public class MakeLine : MonoBehaviour
{
    public GameObject Dot;
    public GameObject Res;

    List<GameObject> DotList = new List<GameObject>();
    float LineTime = 0.05f;

    public void EndLine()
    {
        foreach (var a in DotList) Destroy(a);
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
        Vector3 Position1 = ParentPointCorrect(a1);
        Vector3 Position2 = ParentPointCorrect(a2);
        Vector3 mid = (Position1 + Position2) / 2;
        DrawDot(Position1, mid, MyUi.UISize(a1));
        DrawDot(Position2, mid, MyUi.UISize(a2));
    }

    void DrawDot(Vector3 Start, Vector3 End, Vector2 SizeOfStart)
    {
        float IntervalX = MyUi.UISize(Dot).x;
        if (Start.x > End.x) StartCoroutine(DrawDotLineX(Start, IntervalX * (-2) , End));
        else StartCoroutine(DrawDotLineX(Start, IntervalX * 2, End));
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
