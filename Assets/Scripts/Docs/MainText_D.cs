using TMPro;
using UnityEngine;

public class MainText_D : MonoBehaviour
{
    [SerializeField] TextMannager_D TD;
    TMP_Text Text;

    private void Awake()
    {
        Text = GetComponentInChildren<TMP_Text>();
    }

}
