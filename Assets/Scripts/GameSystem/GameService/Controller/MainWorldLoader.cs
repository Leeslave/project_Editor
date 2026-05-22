using System.Collections;
using GameService;
using TMPro;
using UnityEngine;


public class MainWorldLoader : ServiceBase<ILoadService>, ILoadService
{
    private string loadText;

    private Coroutine _loading;
    public bool IsLoading => _loading != null;
    
    private TMP_Text _text;
    [SerializeField] private float textDelay;
    
    
    public void Init(DailyData data)
    {
        loadText = $"{data.date}\n{data.startLocation}";
    }

    public void StartLoading()
    {
        if (IsLoading) return;
        _loading = StartCoroutine(Loading());
    }

    private IEnumerator Loading()
    {
        foreach (char c in loadText)
        {
            _text.text += c;
            yield return new WaitForSeconds(textDelay);
        }
        
        _loading = null;
    }
}
