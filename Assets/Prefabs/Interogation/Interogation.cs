using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interogation : MonoBehaviour
{
    [Header("¸ÞÆ®·Î³ð ÇÁ¸®Æé")]
    public Metronome metronome;

    [Header("BeatPerMinute")]
    public int beatPerMinute;

    private bool hasStarted;

    private double currentTime;
    private int currentNoteLine;

    public string fileName;
    private List<Dictionary<string, object>> data;

    [Header("³ëÆ®")]
    public GameObject note_ClatonToInquisitor;
    public GameObject note_InquisitorToClaton;

    private void Start()
    {
        data = CSVReader.Read("Assets/Workspace_LeeJungWoo/Prefab/Interogation/" + fileName + ".csv");
        currentNoteLine = 1;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            hasStarted = true;
            currentTime = 0d;
        }

        InstantiateNote();
    }

    private void InstantiateNote()
    {
        if (!hasStarted)
            return;

        currentTime += Time.deltaTime;

        if (currentTime >= 60d / beatPerMinute)
        {
            Dictionary<string, object> currentNote = data[currentNoteLine - 1];

            if (currentNote["Claton"].ToString() != "-")
            {
                GameObject note = Instantiate(note_ClatonToInquisitor, GameObject.Find("Claton_Pos (0)").transform.position, new Quaternion());
                note.transform.Find("MarkText").GetComponent<TextMeshPro>().text = currentNote["Claton"].ToString();
                note.GetComponent<Note>().SetBPM(beatPerMinute);
            }
            if (currentNote["Inquisitor"].ToString() != "-")
            {
                GameObject note = Instantiate(note_InquisitorToClaton, GameObject.Find("Inquisitor_Pos (0)").transform.position, new Quaternion());
                note.transform.Find("MarkText").GetComponent<TextMeshPro>().text = currentNote["Inquisitor"].ToString();
                note.GetComponent<Note>().SetBPM(beatPerMinute);
            }

            currentNoteLine++;
            currentTime -= 60d / beatPerMinute;
        }
    }

}
