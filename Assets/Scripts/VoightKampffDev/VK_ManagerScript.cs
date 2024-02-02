using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class VK_ManagerScript : MonoBehaviour
{
    private GameObject EyeViewer { get; set; }
    private GameObject PupilBone { get; set; }
    private Vector2 OriginPos { get; set; } = new Vector2(1.746719f, 0.5157298f);
    private Coroutine PupilMoveCoroutine { get; set; }
    
    private void Awake()
    {
        EyeViewer = GameObject.Find("EyeViewer");
        PupilBone = GameObject.Find("bone_10");
        StartCoroutine(StartTurn(2f, 30f, 10));
    }

    private IEnumerator StartTurn(float wait, float duration, int num)
    {
        yield return new WaitForSeconds(wait);
        Debug.Log("새로운 턴이 시작되었습니다!");
        for (var i = 0; i < num; i++)
        {
            var x = Random.Range(0, 2) == 0 ? Random.Range(-8, -3) : Random.Range(4, 9);
            var y = Random.Range(0, 2) == 0 ? Random.Range(-8, -3) : Random.Range(4, 9);
            var random = new Vector2(x, y);
            yield return PupilMoveCoroutine = StartCoroutine(MovePupil2Random(duration / num, Time.time, random));
        }
        yield return StartCoroutine(MovePupil2Origin(Time.time));
        Debug.Log("타이머가 다 되어 턴이 종료되었습니다!");
    }

    private IEnumerator MovePupil2Random(float duration, float startTime, Vector2 random)
    {
        Debug.Log($"{random} 방향으로 이동!");
        while (Time.time - startTime < duration)
        {
            var x = Input.GetAxis("Horizontal") * 10f;
            var y = Input.GetAxis("Vertical") * 10f;
            var input = new Vector2(x, y);
            var result = random + input;
            PupilBone.transform.Translate(result * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
    }

    private IEnumerator MovePupil2Origin(float startTime)
    {
        Debug.Log("눈동자를 원위치로 이동!");
        var startPos = PupilBone.transform.localPosition;
        while (Time.time - startTime < 1f)
        {
            var per = Mathf.Max(0.001f, Time.time - startTime) / 1f;
            PupilBone.transform.localPosition = Vector2.Lerp(startPos, OriginPos, per);
            yield return new WaitForSeconds(0.02f);
        }
    }
    
    public void OnPupilExit()
    {
        StartCoroutine(OnPupilExit_IE());
    }
    private IEnumerator OnPupilExit_IE()
    {
        if(ReferenceEquals(PupilMoveCoroutine, null) == false)
            StopCoroutine(PupilMoveCoroutine);
        yield return StartCoroutine(MovePupil2Origin(Time.time));
        Debug.Log("눈동자를 제어하지 못해서 턴이 종료되었습니다!");
    }
    
    
}
