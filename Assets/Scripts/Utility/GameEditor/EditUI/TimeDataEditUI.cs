using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
    /// <summary>
    /// 네 개의 시간대 버튼과 하나의 공용 TimeDataPanel을 연결합니다.
    /// </summary>
    public class TimeDataEditUI : EditorUIBase
    {
        private const int TimeDataCount = 4;

        [Header("시간대 선택")]
        [SerializeField] private Toggle[] editTimeButtons = new Toggle[TimeDataCount];

        [Header("공용 편집 패널")]
        [SerializeField] private GameObject timeDataPanel;
        [SerializeField] private List<TimeDataEditorUIBase> timeDataEditors = new();

        public int SelectedIndex { get; private set; } = -1;
        public TimeData SelectedTimeData =>
            CurrentData != null && SelectedIndex >= 0 && SelectedIndex < TimeDataCount
                ? CurrentData.dayTimes[SelectedIndex]
                : null;

        private void Awake()
        {
            for (int i = 0; i < editTimeButtons.Length; i++)
            {
                int index = i;
                if (editTimeButtons[i] != null)
                {
                    editTimeButtons[i].onValueChanged.AddListener((bool active) => {
                        if (active)
                        {
                            EditTime(index);
                        }
                    });
                }
            }
        }

        public override void Init(DailyData data)
        {
            base.Init(data);
            EnsureTimeDataArray();
            Close();
        }

        /// <summary>
        /// 인덱스에 해당하는 TimeData를 공용 패널에서 편집하기 시작합니다.
        /// Unity Button OnClick에서도 직접 호출할 수 있습니다.
        /// </summary>
        public void EditTime(int index)
        {
            if (CurrentData == null)
            {
                Debug.LogError("TimeDataEditUI가 초기화되지 않았습니다.", this);
                return;
            }

            if (index < 0 || index >= TimeDataCount)
            {
                Debug.LogError($"TimeData 인덱스는 0~{TimeDataCount - 1}만 사용할 수 있습니다: {index}", this);
                return;
            }

            EnsureTimeDataArray();
            SelectedIndex = index;

            foreach (TimeDataEditorUIBase editor in timeDataEditors)
            {
                if (editor != null)
                {
                    editor.Init(CurrentData, SelectedTimeData, SelectedIndex);
                }
            }

            if (timeDataPanel != null)
            {
                timeDataPanel.SetActive(true);
            }
        }

        public void Close()
        {
            SelectedIndex = -1;
            if (timeDataPanel != null)
            {
                timeDataPanel.SetActive(false);
            }
        }

        private void EnsureTimeDataArray()
        {
            if (CurrentData == null) return;

            TimeData[] oldTimes = CurrentData.dayTimes;
            if (oldTimes == null || oldTimes.Length != TimeDataCount)
            {
                var normalizedTimes = new TimeData[TimeDataCount];
                if (oldTimes != null)
                {
                    Array.Copy(oldTimes, normalizedTimes, Math.Min(oldTimes.Length, TimeDataCount));
                }

                CurrentData.dayTimes = normalizedTimes;
            }

            for (int i = 0; i < TimeDataCount; i++)
            {
                CurrentData.dayTimes[i] ??= new TimeData();
            }
        }
    }
}
