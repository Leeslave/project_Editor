namespace GameEditor
{
    /// <summary>
    /// TimeDataPanel 내부의 개별 데이터 편집 UI가 상속하는 기반 클래스입니다.
    /// </summary>
    public abstract class TimeDataEditorUIBase : EditorUIBase
    {
        protected TimeData CurrentTimeData { get; private set; }
        protected int CurrentTimeIndex { get; private set; } = -1;

        /// <summary>
        /// 선택된 시간대의 데이터를 UI에 연결합니다.
        /// </summary>
        public virtual void Init(DailyData dailyData, TimeData timeData, int timeIndex)
        {
            base.Init(dailyData);
            CurrentTimeData = timeData;
            CurrentTimeIndex = timeIndex;
        }
    }
}
