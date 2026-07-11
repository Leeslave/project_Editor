using System.Collections.Generic;
using UnityEngine;

namespace GameEditor
{
    public class WorkEditUI : EditorUIBase
    {
        public Transform workContent;
        private GameObject workPrefab => workContent.GetChild(0).gameObject;
        private readonly List<(Work data, WorkInput obj)> works = new();

        public override void Init(DailyData data)
        {
            base.Init(data);

            // 기존 panels 초기화
            foreach (var work in works)
            {
                Destroy(work.obj.gameObject);
            }

            foreach (Work work in CurrentData.workList)
            {
                var newPanel = Instantiate(workPrefab, workContent);
                
                WorkInput workInput = newPanel.GetComponent<WorkInput>();
                workInput.Init(work);
                workInput.OnRemoved += RemoveWork;

                works.Add((work, workInput));
                newPanel.SetActive(true);
            }
        }

        public void AddNew()
        {
            var newInput = Instantiate(workPrefab, workContent).GetComponent<WorkInput>();
            var newWork = new Work();
            
            works.Add((newWork, newInput));
            newInput.Init(newWork);
            newInput.OnRemoved += RemoveWork;
            
            newInput.gameObject.SetActive(true);
        }

        public void RemoveWork(WorkInput work)
        {
            works.RemoveAll(tuple => tuple.obj == work);
            CurrentData.workList.Remove(work.workData);
            Destroy(work.gameObject);
        }
    }
}