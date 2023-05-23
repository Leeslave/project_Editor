using UnityEngine.UI;
using UnityEngine;

public class Drager_M : MonoBehaviour
{
    public InfChange In;

    private Image cnt;
    private bool IsTouch = false;
    private Color C1 = new Color(0, 0, 0, 1);
    private Vector3 s;
    private string[] d = {"Job : ", "Country : " };

    void Update()
    {
        s = Camera.main.ScreenToWorldPoint(Input.mousePosition); s.z = 0;
        transform.position = s;
        if (Input.GetMouseButtonUp(0))
        {
            if (IsTouch)
            {
                if (In.s != 2) cnt.GetComponent<TextChanger_M>().Changer(d[In.s] + name);
            }
            gameObject.SetActive(false);
            In.TouchAbleChange();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        IsTouch = true;
        cnt = collision.transform.GetChild(In.s).GetComponent<Image>();
        cnt.color = cnt.color + C1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IsTouch = false;
        cnt.color = cnt.color - C1;
    }

    private void OnEnable()
    {
        Debug.Log("!");
        IsTouch = false;
        transform.SetAsLastSibling();
    }
}
