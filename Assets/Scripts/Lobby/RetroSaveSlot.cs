using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class RetroSaveSlot : MonoBehaviour
{
    public Button button;
    public Image titleBar;
    public Image preview;
    public Image fileSelection;
    public Text title;
    public Text fileName;
    public Text renown;

    public void Bind(DaySave save, Sprite sprite, bool selected, UnityAction onSelect)
    {
        title.text = RetroSaveDesktop.BranchName(save.Branch);
        fileName.text = RetroSaveDesktop.FileName(save);
        renown.text = $"명성 {save.renown}";
        RetroSaveUi.SetPicture(preview, sprite);
        titleBar.color = selected ? RetroSaveUi.Blue : RetroSaveUi.Gray;
        fileSelection.color = selected ? RetroSaveUi.Blue : Color.clear;
        fileName.color = selected ? Color.white : Color.black;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onSelect);
    }
}
