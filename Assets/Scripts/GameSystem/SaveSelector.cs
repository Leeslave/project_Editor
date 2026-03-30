using GameService;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSelector : MonoBehaviour
{
    private int date;
    private DaySave saveData;
    
    [Header("UI Info")]
    [SerializeField] private TMP_Text dateInfo;
    [SerializeField] private Image dateImage;
    [SerializeField] private TMP_Text renown;

    public void Init(int date, DaySave saveData)
    {
        // Init Data
        this.date = date;
        this.saveData = saveData;
        
        // Set UI
        if (date > 0) dateInfo.text = $"Day {date.ToString()}";
        if (!string.IsNullOrEmpty(saveData.thumbnail)) dateImage.sprite = Resources.Load<Sprite>(saveData.thumbnail);       // TODO: Data Load Path
        renown.text = saveData.renown.ToString();
    }

    public void LoadDay()
    {
        var dayService = ServiceProvider.Get<IDayService>();
        dayService.Date = date;
        SceneManager.LoadScene("MainWorld");
    }
}
