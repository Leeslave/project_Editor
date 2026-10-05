using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TextMannager_D : MonoBehaviour
{
    private const int DotPoolSize = 25;
    private const int TextPoolSize = 16;
    private const float DotSpacing = 30f;

    [FormerlySerializedAs("IsRecord")]
    [SerializeField] private bool isRecord;

    [FormerlySerializedAs("TextsPref")]
    [SerializeField] private GameObject textPrefab;
    [FormerlySerializedAs("DotPref")]
    [SerializeField] private GameObject dotPrefab;
    [FormerlySerializedAs("NotTouch")]
    [SerializeField] private EventTrigger interactionBlocker;
    [FormerlySerializedAs("Message")]
    [SerializeField] private TMP_Text resultMessage;
    [FormerlySerializedAs("AbnormalBT")]
    [SerializeField] private Button abnormalButton;
    [FormerlySerializedAs("NormalBT")]
    [SerializeField] private Button normalButton;

    [NonSerialized] private List<GameObject> dots;
    [NonSerialized] private List<GameObject> textObjects;
    [NonSerialized] private List<Docs_Back> textEntries;

    public TextMannager_D OtherMannager;

    private int activeTextCount;

    public Transform CurSelection;
    private int selectedTextIndex = -1;

    public int Errors = 0;

    public int MyAns = -1;

    private void Start()
    {
        textObjects = new List<GameObject>(TextPoolSize) { textPrefab };
        dots = new List<GameObject>(DotPoolSize);
        textEntries = new List<Docs_Back>(TextPoolSize);

        for (int i = 0; i < DotPoolSize; i++)
            dots.Add(Instantiate(dotPrefab, transform));
        for (int i = 1; i < TextPoolSize; i++)
            textObjects.Add(Instantiate(textPrefab, textPrefab.transform.parent));
        foreach (GameObject textObject in textObjects)
        {
            textEntries.Add(textObject.GetComponent<Docs_Back>());
            textObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        selectedTextIndex = -1;
        CurSelection = null;
        if (textObjects != null)
        {
            foreach (GameObject textObject in textObjects)
                textObject.SetActive(false);
        }
        activeTextCount = 0;
        if (isRecord)
        {
            normalButton.interactable = true;
            abnormalButton.interactable = false;
        }
    }


    public void AddText(string text, Color color, TextAlignmentOptions align = TextAlignmentOptions.Left, bool IsTouchAble = true)
    {
        textObjects[activeTextCount].SetActive(true);
        textEntries[activeTextCount++].AddTexts(text, color, this, align, IsTouchAble);
    }

    public void Clicked(int Ind, Transform tr)
    {
        if (selectedTextIndex != -1)
            textEntries[selectedTextIndex].UnSelect();
        selectedTextIndex = Ind;
        CurSelection = tr;

        if (OtherMannager.selectedTextIndex != -1)
        {
            JudgeStart();
            OtherMannager.JudgeStart();
        }
    }

    public void JudgeStart()
    {
        interactionBlocker.gameObject.SetActive(true);
        Vector3 halfDistance = (OtherMannager.CurSelection.position - CurSelection.position) * 0.5f;
        int horizontalSteps = Mathf.FloorToInt(Mathf.Abs(halfDistance.x / DotSpacing));
        int verticalSteps = Mathf.FloorToInt(Mathf.Abs(halfDistance.y / DotSpacing));
        Vector3 horizontalStep = horizontalSteps == 0 ? Vector3.zero : halfDistance / horizontalSteps;
        horizontalStep.y = 0;
        Vector3 verticalStep = verticalSteps == 0 ? Vector3.zero : halfDistance / verticalSteps;
        verticalStep.x = 0;

        Vector3 startPosition = CurSelection.position;
        int dotCount = 0;
        for (int i = 0; i < horizontalSteps; i++)
            dots[dotCount++].transform.position = startPosition + horizontalStep * i;
        startPosition += horizontalStep * (horizontalSteps - 1);
        for (int i = 0; i <= verticalSteps; i++)
            dots[dotCount++].transform.position = startPosition + verticalStep * i;
        StartCoroutine(DotAct(dotCount));
    }

    private IEnumerator DotAct(int dotCount)
    {
        for (int index = 0; index < dotCount - 1; index++)
        {
            dots[index].SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        if (isRecord)
        {
            resultMessage.transform.position = dots[dotCount - 1].transform.position;
            resultMessage.gameObject.SetActive(true);
            if (IsCor() && OtherMannager.IsCor())
            {
                CorrectedAnswer();
                OtherMannager.CorrectedAnswer();
                resultMessage.text = "Abnormal Detection!";
                CurDocs.IsAbnormalFinded = true;
                abnormalButton.interactable = true;
                normalButton.interactable = false;
            }
            else resultMessage.text = "No Abnormal";
            MyUi.AddEvent(interactionBlocker, EventTriggerType.PointerClick,
                (PointerEventData eventData) =>
                {
                    RemoveDot();
                    OtherMannager.RemoveDot();
                    resultMessage.gameObject.SetActive(false);
                    interactionBlocker.gameObject.SetActive(false);
                    interactionBlocker.triggers.Clear();
                }
            );
        }
    }

    public void CorrectedAnswer()
    {
        textEntries[selectedTextIndex].GetCorrected();
    }

    public bool IsCor() 
    {
        return MyAns == selectedTextIndex;
    }

    public void RemoveDot()
    {
        foreach (GameObject dot in dots)
            dot.SetActive(false);
    }

    [HideInInspector] public Docs CurDocs;

    public void EndDocsTask()
    {
        DB_M.DB_Docs.ToDoList.CheckList(1, 0, true);
        CurDocs.IsDone = true;
    }
}
