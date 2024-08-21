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

    bool IsScreen = false;

    float MaxSize;

    float CamSize = 1200;
    float ReverseCamSize;

    private void Awake()
    {
        SelfRect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        Mat = image.material;
        trigger = GetComponent<EventTrigger>();
        TextDetail = TextBox.GetChild(0).GetComponent<TMP_Text>();
        gameObject.SetActive(false);
        ReverseCamSize = 1 / CamSize;
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
                print($"{e} At Clear");
            }
        }

        TutorialSetting.instance.CurActive = this;
        this.Target = Target;

        if (Target == null)
        {
            ObjSize = Vector2.zero;
            StartCoroutine(Test(text, IsHighlight));
            return 0;
        }

        ObjSize = Vector2.one;

        Transform ObjTrans = Target.transform;

        IsScreen = false;
        while (ObjTrans != null)
        {
            ObjSize *= ObjTrans.localScale;
            if (ObjTrans.TryGetComponent(out Canvas cv)) IsScreen = cv.renderMode == RenderMode.ScreenSpaceOverlay;
            ObjTrans = ObjTrans.parent;
        }


        if(!IsScreen) ObjSize *= (500 / Camera.main.orthographicSize);


        if (Target.TryGetComponent(out RectTransform Rect))
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
            ObjSize *= Rect.rect.size;
        }
        else
        {
            if(Target.TryGetComponent(out SpriteRenderer Sprite)) ObjSize *= Sprite.sprite.bounds.size;
        }
        
        if(!IsRect) ObjSize *= 1.2f;

        MaxSize = Mathf.Max(ObjSize.x * ReverseCamSize, ObjSize.y * ReverseCamSize);

        CalcPosition();

        if (IsRect)
        {
            Mat.SetVector("_MaskCenter", new Vector4(Pos.x / Screen.width, (Pos.y + Screen.width * 0.125f) / Screen.width));
            Mat.SetFloat("_X", ObjSize.x * 0.9f / CamSize);
            Mat.SetFloat("_Y", ObjSize.y * 0.9f / CamSize);
        }
        else
        {
            Mat.SetVector("_MaskCenter", new Vector4(Pos.x / Screen.width, (Pos.y + Screen.width * 0.125f) / Screen.width));
            Mat.SetFloat("_Radius", MaxSize);
        }

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
            Mat.SetFloat("_X", ObjSize.x * 0.9f * ReverseCamSize);
            Mat.SetFloat("_Y", ObjSize.y * 0.9f * ReverseCamSize);
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
        WaitForSecondsRealtime ts = new WaitForSecondsRealtime(0.01f);

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
            yield return new WaitForSecondsRealtime(0.2f);
        }

        if (text.Length != 0)
        {
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
        }

        TutorialSetting.instance.SetEvent(trigger);
    }

    public void ChangeTarget(GameObject Target)
    {
        this.Target = Target;
    }

    Vector2 TextPos = Vector2.zero;
    void CalcPosition()
    {
        if (Target == null)
        {
            TextBox.localPosition = Vector2.zero;
        }
        else
        {
            if (IsScreen) Pos = Target.transform.position;
            else Pos = Camera.main.WorldToScreenPoint(Target.transform.position);

            if (IsRect) TextPos = new Vector2(Pos.x * 1200 / Screen.width - 600, Pos.y * 900 / Screen.height - 450 - ObjSize.y * 0.5f - TextBox.rect.height * 0.5f);
            else TextPos = new Vector2(Pos.x * 1200 / Screen.width - 600, Pos.y * 900 / Screen.height - 450 - ObjSize.y * 0.8f - TextBox.rect.height * 0.5f);

            if (TextPos.y - TextBox.rect.height * 0.5f < -450)
            {
                if (IsRect) TextPos.y += ObjSize.y + TextBox.rect.height;
                else TextPos.y += ObjSize.y * 1.6f + TextBox.rect.height;
            }

            if (TextPos.x > 600 - TextBox.rect.width * 0.5f) TextPos.x = 600 - TextBox.rect.width * 0.5f;
            else if (TextPos.x < -600 + TextBox.rect.width * 0.5f) TextPos.x = -600 + TextBox.rect.width * 0.5f;

            if (TextPos.y > 450 - TextBox.rect.height * 0.5f) TextPos.y = 450 - TextBox.rect.height * 0.5f;
            else if (TextPos.y < -450 + TextBox.rect.height * 0.5f) TextPos.y = -450 + TextBox.rect.height * 0.5f;

            TextBox.localPosition = TextPos;
        }
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
