using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimizeDesktopButton : PointerButton
{
    [SerializeField] private float _minY;
    [SerializeField] private float _maxY;
    [SerializeField] private Transform _panel;
    [SerializeField] private TMP_Text _toggleLabel;
    [SerializeField] private bool _disableAnimation;
    private float _stepDistance;
    private bool _expanded = true;

    protected override void Awake()
    {
        base.Awake();
        _stepDistance = (_maxY - _minY) * 0.05f;
    }

    protected override void Click(PointerEventData data)
    {
        if (!_disableAnimation)
        {
            StartCoroutine(AnimatePanel());
        }
    }

    public void ClickCall()
    {
        Click(null);
    }

    private WaitForSeconds _stepDelay = new WaitForSeconds(0.01f);
    // 패널을 단계별로 이동하고 접힘 표시를 전환한다.
    private IEnumerator AnimatePanel()
    {
        for(int i = 0; i < 20; i++)
        {
            if (_expanded) _panel.Translate(0,-_stepDistance, 0);
            else _panel.Translate(0,_stepDistance,0);
            yield return _stepDelay;
        }
        if (_expanded) _toggleLabel.text = "+";
        else _toggleLabel.text = "-";
        _expanded = _expanded == false;
    }
}
