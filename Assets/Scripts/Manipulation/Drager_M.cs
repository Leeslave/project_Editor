using UnityEngine.UI;
using UnityEngine;

public class Drager_M : MonoBehaviour
{
    [SerializeField]
    private Image FaceImage;
    private Image cnt;
    private Image MyImage;
    private bool IsTouch = false;
    private Color C1 = new Color(0, 0, 0, 1);
    private Vector3 s;
    private string[] d = {"국가 : " , "부서 : ", "소속 : ", "직업 : "};

    private void Awake()
    {
        MyImage = transform.GetChild(1).GetComponent<Image>();
    }

    void Update()
    {
        s = Camera.main.ScreenToWorldPoint(Input.mousePosition); s.z = 0;
        transform.position = s;
        if (Input.GetMouseButtonUp(0))
        {
            if (IsTouch)
            {
                if (DB_M.DB_Docs.PersonDataManager.CurDraggedType != 4) cnt.GetComponent<TextChanger_M>().Changer(d[DB_M.DB_Docs.PersonDataManager.CurDraggedType] + name);
                else { FaceImage.sprite = MyImage.sprite; DB_M.DB_Docs.PersonDataManager.FaceNum = name[0] - '0'; }
            }
            gameObject.SetActive(false);
            DB_M.DB_Docs.PersonDataManager.TouchAbleChange();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        IsTouch = true;
        cnt = collision.transform.GetChild(DB_M.DB_Docs.PersonDataManager.CurDraggedType).GetComponent<Image>();
        cnt.color = cnt.color + C1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IsTouch = false;
        cnt.color = cnt.color - C1;
    }

    private void OnEnable()
    {
        IsTouch = false;
        transform.SetAsLastSibling();
    }
}
