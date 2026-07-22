using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor
{
    /// <summary>
    /// 한 지역의 BGM 드롭다운 입력을 담당합니다.
    /// </summary>
    public class WorldBGMInput : MonoBehaviour
    {
        [SerializeField] private TMP_Text locationName;
        [SerializeField] private TMP_Dropdown bgmDropdown;
        [SerializeField] private Button testPlayButton;

        private World currentWorld;
        private Action<World, int> onChanged;
        private Action<int> onTestPlay;

        private void Awake()
        {
            if (bgmDropdown == null)
            {
                bgmDropdown = GetComponentInChildren<TMP_Dropdown>(true);
            }

            if (testPlayButton == null)
            {
                testPlayButton = GetComponentInChildren<Button>(true);
            }
        }

        public void Init(
            World world,
            int code,
            Action<World, int> changedCallback,
            Action<int> testPlayCallback)
        {
            if (bgmDropdown == null)
            {
                bgmDropdown = GetComponentInChildren<TMP_Dropdown>(true);
            }

            if (testPlayButton == null)
            {
                testPlayButton = GetComponentInChildren<Button>(true);
            }

            currentWorld = world;
            onChanged = changedCallback;
            onTestPlay = testPlayCallback;

            if (locationName != null)
            {
                locationName.text = world.ToString();
            }

            if (bgmDropdown == null)
            {
                Debug.LogError($"{name}에서 BGM Dropdown을 찾을 수 없습니다.", this);
                return;
            }

            int safeCode = code;
            if (safeCode < 0 || safeCode >= bgmDropdown.options.Count)
            {
                safeCode = (int)world;
            }

            bgmDropdown.onValueChanged.RemoveListener(OnValueChanged);
            bgmDropdown.SetValueWithoutNotify(safeCode);
            bgmDropdown.RefreshShownValue();
            bgmDropdown.onValueChanged.AddListener(OnValueChanged);

            if (testPlayButton != null)
            {
                testPlayButton.onClick.RemoveListener(TestPlay);
                testPlayButton.onClick.AddListener(TestPlay);
            }
        }

        private void OnValueChanged(int code)
        {
            onChanged?.Invoke(currentWorld, code);
        }

        private void TestPlay()
        {
            if (bgmDropdown != null)
            {
                onTestPlay?.Invoke(bgmDropdown.value);
            }
        }

        private void OnDestroy()
        {
            if (bgmDropdown != null)
            {
                bgmDropdown.onValueChanged.RemoveListener(OnValueChanged);
            }


            if (testPlayButton != null)
            {
                testPlayButton.onClick.RemoveListener(TestPlay);
            }
        }
    }
}
