using GameService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveWindow : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text DayTitle;
    [SerializeField] private TMP_Text RenownText;
    [SerializeField] private Image thumbnail;

    private int _day;
    private int _branch;

    public void Init(DaySave save)
    {
        _day = save.Day;
        _branch = save.Branch;
        
        DayTitle.text = $"Day {save.Day.ToString()}";
        RenownText.text = save.renown.ToString();

        if (!string.IsNullOrEmpty(save.thumbnail))
        {
            thumbnail.sprite = Resources.Load<Sprite>($"thumbnail/{save.thumbnail}");
        }
    }

    public void OnClick()
    {
        GameSystem.Instance.EnterScene("MainWorld", callback: () =>
        {
            GameSystem.SaveService.SelectDay(_day, _branch);
        });
    }
}
