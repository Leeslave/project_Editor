using UnityEngine;
using UnityEngine.UI;

namespace DialogueConceptTest
{
    // Deliberately independent of GameSystem, save data and production dialogue services.
    public sealed class ComicDialoguePreview : MonoBehaviour
    {
        public Text dialogue;
        public GameObject choices;
        public GameObject history;
        public Button[] choiceButtons;
        public Button[] controls;
        private int page;
        private readonly string[] pages =
        {
            "진실을 쓰는 데에도 대가가 따르지.\n하지만 우리가 침묵하면,\n이 사건은 영원히 묻히고 말 거야.",
            "기록을 다시 살펴봐.\n우리가 놓친 단서가 있을지도 몰라.\n어떤 선택을 할지는 자네에게 맡기겠네.",
            "오늘의 취재는 여기까지 하세.\n내일은 더 많은 진실이 기다리고 있을 테니."
        };

        private void Awake()
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                int index = i;
                choiceButtons[i].onClick.AddListener(() => SelectChoice(index));
            }
            controls[0].onClick.AddListener(() => ShowPage(0));
            controls[1].onClick.AddListener(() => choices.SetActive(!choices.activeSelf));
            controls[2].onClick.AddListener(() => history.SetActive(!history.activeSelf));
            controls[3].onClick.AddListener(() => ShowPage(pages.Length - 1));
            controls[4].onClick.AddListener(() => ShowPage(page - 1));
            controls[5].onClick.AddListener(() => ShowPage((page + 1) % pages.Length));
            ShowPage(0);
        }

        private void ShowPage(int index)
        {
            page = Mathf.Clamp(index, 0, pages.Length - 1);
            dialogue.text = pages[page];
        }

        private void SelectChoice(int index)
        {
            dialogue.text = index == 0
                ? "기사를 공개하겠다고?\n좋아. 그렇다면 마지막까지 책임지고 쓰게."
                : "서두를 필요는 없지.\n확실한 증거를 확보한 다음에 다시 이야기하세.";
        }
    }
}
