using System.Collections;
using GameService;
using TMPro;
using UnityEngine;


public class MainWorldLoader : ServiceBase<ILoadService>, ILoadService
{
    private string loadText;

    private Coroutine _loading;
    public bool IsLoading => _loading != null;
    
    [Header("Loading Settings")]
    [SerializeField] private GameObject loadingObject;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float textDelay;
    [SerializeField] private float fadeDelay;
    
    
    public void Init(DailyData data)
    {
        loadText = $"{data.date}\n{data.startLocation}";
        
        StartLoading();
    }

    public void StartLoading()
    {
        if (IsLoading) return;
        if (!loadingObject) return;
        
        _loading = StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        loadingObject.SetActive(true);
        
        text.text = "";
        foreach (char c in loadText)
        {
            text.text += c;
            yield return new WaitForSeconds(textDelay);
        }
        yield return new WaitForSeconds(fadeDelay);
        
        loadingObject.SetActive(false);
        _loading = null;
    }
}
