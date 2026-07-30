using TMPro;

namespace GameEditor
{
    public class FileNameUI : EditorUIBase
    {
        public TMP_Text fileName;

        public override void Init(DailyData data)
        {
            base.Init(data);

            fileName.text = DayBuilder.Instance.fileName;
        }
    }
}