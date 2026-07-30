using TMPro;

namespace GameEditor
{
    public class StartLocationEditUI : EditorUIBase
    {
        public TMP_Text startLocation;

        public override void Init(DailyData data)
        {
            base.Init(data);

            Refresh();
        }

        public void ChangeLocation(WorldVector newLocation)
        {
            CurrentData.startLocation = newLocation;
            Refresh();
        }

        private void Refresh()
        {
            if (startLocation != null)
            {
                startLocation.text = CurrentData.startLocation.ToString();
            }
        }
    }
}
