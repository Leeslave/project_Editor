using TMPro;

namespace GameEditor
{
    public class DateEditUI : EditorUIBase
    {
        public TMP_InputField yearField;
        public TMP_InputField monthField;
        public TMP_InputField dayField;
        
        public override void Init(DailyData data)
        {
            base.Init(data);
            
            yearField.text = data.date.year.ToString();
            yearField.onValueChanged.AddListener(delegate
            {
                CurrentData.date.year = int.Parse(yearField.text);
            });
            
            monthField.text = data.date.month.ToString();
            monthField.onValueChanged.AddListener(delegate
            {
                CurrentData.date.month = int.Parse(monthField.text);
            });
            
            dayField.text = data.date.day.ToString();
            dayField.onValueChanged.AddListener(delegate
            {
                CurrentData.date.day = int.Parse(dayField.text);
            });
        }
    }
}
