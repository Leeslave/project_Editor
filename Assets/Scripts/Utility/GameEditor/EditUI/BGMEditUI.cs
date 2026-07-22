using System.Collections.Generic;
using UnityEngine;

namespace GameEditor
{
    /// <summary>
    /// 선택된 TimeData의 지역별 BGM 설정을 관리합니다.
    /// </summary>
    public class BGMEditUI : TimeDataEditorUIBase
    {
        [SerializeField] private List<WorldBGMInput> worldInputs = new();
        [SerializeField] private SoundManager bgmTester;

        private void Awake()
        {
            FindInputsIfNeeded();
        }

        private void FindInputsIfNeeded()
        {
            if (worldInputs.Count == 0)
            {
                GetComponentsInChildren(true, worldInputs);
                worldInputs.Sort((left, right) =>
                    left.transform.GetSiblingIndex().CompareTo(right.transform.GetSiblingIndex()));
            }
        }

        public override void Init(DailyData dailyData, TimeData timeData, int timeIndex)
        {
            base.Init(dailyData, timeData, timeIndex);
            FindInputsIfNeeded();

            CurrentTimeData.bgm ??= new List<BGMData>();

            int worldCount = (int)World.Max;
            for (int i = 0; i < worldInputs.Count; i++)
            {
                if (worldInputs[i] == null) continue;

                if (i >= worldCount)
                {
                    worldInputs[i].gameObject.SetActive(false);
                    continue;
                }

                World world = (World)i;
                int code = GetSavedCodeOrDefault(world);
                worldInputs[i].Init(world, code, ChangeBGM, TestPlay);
            }
        }

        private void TestPlay(int code)
        {
            if (bgmTester == null)
            {
                Debug.LogError("BGMEditUI에 BGMTester가 연결되지 않았습니다.", this);
                return;
            }

            if (code < 0 || code >= bgmTester.clips.Count)
            {
                Debug.LogError($"테스트할 BGM 코드가 범위를 벗어났습니다: {code}", this);
                return;
            }

            bgmTester.SetClip(code);
            bgmTester.Play();
        }

        private int GetSavedCodeOrDefault(World world)
        {
            BGMData savedData = CurrentTimeData.bgm.Find(data => data.location == world);
            return savedData != null ? savedData.code : (int)world;
        }

        private void ChangeBGM(World world, int code)
        {
            // 같은 지역의 중복 데이터를 방지하고, 기본 BGM이면 별도 데이터를 저장하지 않습니다.
            CurrentTimeData.bgm.RemoveAll(data => data.location == world);

            if (code == (int)world) return;

            CurrentTimeData.bgm.Add(new BGMData
            {
                location = world,
                code = code
            });
        }
    }
}
