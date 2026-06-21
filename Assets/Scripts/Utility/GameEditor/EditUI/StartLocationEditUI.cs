using System.Collections.Generic;

namespace GameEditor
{
    public class StartLocationEditUI : EditorUIBase
    {
        public List<LocationButton> locations;

        public override void Init(DailyData data)
        {
            base.Init(data);

            RefreshButtons();
        }

        public void ChangeLocation(WorldVector newLocation)
        {
            CurrentData.startLocation = newLocation;
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            foreach (var btn in locations)
            {
                if (btn.location == CurrentData.startLocation)
                {
                    btn.OnSelect();
                    continue;
                }
                btn.OnSelect(false);
            }
        }
    }
}
