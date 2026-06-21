using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Utility;

namespace GameEditor
{
    public class DayBuilder : Singleton<DayBuilder>
    {
        /**
         * 날짜데이터 빌드 시스템
         */
        [Header("수정중 상태")]
        [SerializeField] private bool onEdit;

        [Header("파일 수정")]
        public string fileName;
        private DailyData _data;

        [Space(20)] 
        [Header("게임 파일 정보")] 
        public List<string> dataFiles = new();

        [Space(20)] 
        [Header("데이터 오브젝트")] 
        public GameObject buttonPrefab;
        public GameObject dataList;
        public RectTransform dataScroll;
        public List<GameObject> dataButtons = new();
        public TMP_InputField newFileName;
        
        [Space(10)]
        [Header("수정 UI")]
        public GameObject dayPanel;
        public List<EditorUIBase> editors;


        /// 데이터파일 리스트 불러오기
        public void OnLoadClick()
        {
            EditorLogger.Log("Start Load");
            // 데이터 목록 불러오기
            dataFiles.Clear();
            dataFiles = DataLoader.GetDayDataNames();

            // 기존 버튼들 삭제
            foreach (var obj in dataButtons)
            {
                Destroy(obj);
            }

            dataButtons.Clear();

            // 새로 버튼 생성
            foreach (var file in dataFiles)
            {
                GameObject button = Instantiate(buttonPrefab, dataScroll);
                dataButtons.Add(button);

                TMP_Text text = button.GetComponentInChildren<TMP_Text>();
                if (text is not null) text.text = file;


                button.GetComponent<Button>().onClick.AddListener(() => EditDayData(file));
            }

            dayPanel.SetActive(false);
            dataList.SetActive(true);
        }


        /// <summary>
        /// DayData 수정 시작
        /// </summary>
        /// <remarks>날짜 파일을 불러온 후 </remarks>
        private void EditDayData(string gameFile)
        {
            var gameData = DataLoader.GetDayData(gameFile);
            if (gameData is null) return;
            
            fileName = gameFile;
            Debug.Log($"Start Editing {gameFile}");
            
            // DayEditor 실행으로 전달
            InitEdit();
        }


        /// <summary>
        /// DayData 수정 시작
        /// </summary>
        /// <remarks>날짜 파일을 불러온 후 </remarks>
        public void MakeNewDayData()
        {
            fileName = newFileName.text;
            Debug.Log($"Create New Editing {fileName}");
            
            // DayEditor 실행으로 전달
            InitEdit();
        }

        /// <summary>
        /// 수정 UI 초기화 및 연결
        /// </summary>
        private void InitEdit()
        {
            dayPanel.SetActive(true);
            dataList.SetActive(false);
            
            onEdit = true;
            foreach (var obj in editors)
            {
                obj.Init(_data);
            }
        }

        /// <summary>
        /// DayData 수정 시작
        /// </summary>
        /// <remarks>날짜 파일을 불러온 후 </remarks>
        public void Submit()
        {
            if (!onEdit) return;

            DataLoader.SaveDayData(fileName, _data);
            EditorLogger.Log($"Data Saved {fileName}");
            fileName = "";
            onEdit = false;
            dayPanel.SetActive(false);
            dataList.SetActive(true);
        }
    }
}
