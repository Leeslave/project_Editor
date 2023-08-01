using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Detect_Obs : MonoBehaviour
{
    [SerializeField]
    string Target;
    [SerializeField]
    GameObject Text;
    [SerializeField]
    TMP_Text[] LogTexts;
    [SerializeField]
    Timer_Obs Timer;
    [SerializeField]
    TMP_Text Cnt;
    [SerializeField]
    TMP_Text Chat;

    int TextsInd = 0;
    GameObject CurTarget;
    NPC_Obs TargetObs;
    float Hor;
    float Ver;


    void Update()
    {
        Hor = Input.GetAxisRaw("Horizontal");
        Ver = Input.GetAxisRaw("Vertical");
        if (Hor * transform.position.x > 520) Hor = 0;
        if (Ver * (transform.position.y-30) > 340) Ver = 0;


        transform.Translate(new Vector3(Hor*1.5f,Ver*1.5f,0));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(CurTarget != null)
            {
                if (CurTarget.name == Target)
                {
                    LogTexts[TextsInd].text = Timer.ReturnTime();
                    LogTexts[TextsInd++].gameObject.SetActive(true);
                }
                else
                {
                    if (Text.activeSelf) Text.SetActive(false);
                    Text.SetActive(true);
                }
            }
        }
    }

    public void MakeNoise()
    {
        if (CurTarget == null) return;
        if (Chat.gameObject.activeSelf) Chat.gameObject.SetActive(false);
        Chat.gameObject.SetActive(true);
        Chat.text = TargetObs.Chats[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CurTarget = collision.gameObject;
        Cnt.text = CurTarget.name;
        TargetObs = CurTarget.GetComponent<NPC_Obs>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (CurTarget == collision.gameObject)
        {
            CurTarget = null;
            Cnt.text = "";
        }
    }
}
