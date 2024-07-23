using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialBorder : MonoBehaviour
{
    [SerializeField] bool IsRect;
    [SerializeField] Material Mat;
    [SerializeField] RectTransform TextBox;
    [SerializeField] RectTransform CanvasRect;
    [SerializeField] GameObject Target;

    TMP_Text TextDetail;
    RectTransform SelfRect;
    Image image;
    EventTrigger trigger;

    Vector2 Pos;
    Vector2 ScreenSize;
    Vector2 ObjSize;

    float MaxSize;


    private void Awake()
    {
        SelfRect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        Mat = image.material;
        trigger = GetComponent<EventTrigger>();
        TextDetail = TextBox.GetChild(0).GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    public float Init(GameObject Target,string text,bool IsHighlight,bool KeepEvent)
    {
        if (!KeepEvent)
        {
            try
            {
                trigger.triggers.Clear();
            }
            catch(System.Exception e)
            {
                print($"Error At Clear!");
            }
        }
        TutorialSetting.instance.CurActive = this;
        this.Target = Target;
        ObjSize = Vector2.one;
        Transform ObjTrans = Target.transform;

        while (ObjTrans.parent != null)
        {
            ObjSize *= ObjTrans.localScale;
            ObjTrans = ObjTrans.parent;
        }
        if (Target.TryGetComponent(out RectTransform Rect)) ObjSize *= Rect.rect.size;
        
        if(!IsRect) ObjSize *= 1.2f;
        MaxSize = Mathf.Max(ObjSize.x / 1200, ObjSize.y / 1200);

        

        gameObject.SetActive(true);

        StartCoroutine(Test(text,IsHighlight));

        return Mathf.Max(ObjSize.x, ObjSize.y);
    }

    private void Update()
    {
        CalcPosition();

        if (IsRect)
        {
            Mat.SetVector("_MaskCenter", new Vector4(Pos.x / Screen.width, (Pos.y + Screen.width * 0.125f) / Screen.width));
            Mat.SetFloat("_X", ObjSize.x * 0.9f / 1200);
            Mat.SetFloat("_Y", ObjSize.y * 0.9f / 1200);
        }
        else
        {
            Mat.SetVector("_MaskCenter", new Vector4(Pos.x / Screen.width, (Pos.y + Screen.width * 0.125f) / Screen.width));
            Mat.SetFloat("_Radius", MaxSize);
        }
    }

    IEnumerator Test(string text, bool IsHighlight)
    {
        TextDetail.text = "";
        TextBox.gameObject.SetActive(false);
        WaitForSeconds ts = new WaitForSeconds(0.01f);

        if (IsHighlight)
        {
            if (IsRect)
            {
                Vector2 Gap = (ObjSize - new Vector2(1200, 1200))*0.05f;
                ObjSize.x = ObjSize.y = 1200;

                for (int i = 0; i < 20; i++)
                {
                    yield return ts;
                    ObjSize += Gap;
                }
            }
            else
            {
                float Gap = (MaxSize - 1) * 0.05f;
                MaxSize = 1;

                for(int i = 0; i < 20; i++)
                {
                    yield return ts;
                    MaxSize += Gap;                
                }
            }
        }

        yield return new WaitForSeconds(0.2f);

        TextDetail.text = text;
        TextBox.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox);
        TextDetail.color = new Color(1, 1, 1, 0);
        Color ColorCnt = new Color(0, 0, 0, 0.1f);

        for (int i = 0; i < 10; i++)
        {
            TextDetail.color += ColorCnt;
            yield return ts;
        }

        TutorialSetting.instance.SetEvent(trigger);
    }

    public void ChangeTarget(GameObject Target)
    {
        this.Target = Target;
    }


    void CalcPosition()
    {
        Pos = Camera.main.WorldToScreenPoint(Target.transform.position);
        Vector2 TextPos = new Vector2(Pos.x * 1200 / Screen.width - 600, Pos.y * 900 / Screen.height - 450 - ObjSize.y * 0.7f - TextBox.rect.height * 0.6f);

        if (TextPos.x > 600 - TextBox.rect.width * 0.5f) TextPos.x = 600 - TextBox.rect.width * 0.5f;
        else if (TextPos.x < -600 + TextBox.rect.width * 0.5f) TextPos.x = -600 + TextBox.rect.width * 0.5f;


        if (TextPos.y - TextBox.rect.height * 0.5f < -450) TextPos.y += (ObjSize.y + TextBox.rect.height) * 1.2f;

        if (TextPos.y + TextBox.rect.height * 0.5f > 450 || TextPos.y - TextBox.rect.height * 0.5f < -450)
            TextPos.y =  Pos.y * 900 / Screen.height - 450 + 100;


        TextBox.localPosition = TextPos;
    }

    public void InActiveRay()
    {
        image.raycastTarget = false;
    }

    public void ActiveRay()
    {
        image.raycastTarget = true;
    }
}
