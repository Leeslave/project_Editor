using UnityEngine;
using UnityEngine.UI;

namespace float_oat.Desktop90.Examples
{
    /// <summary>
    /// Controls the text elements for previewing the different fonts
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class FontPreviewText : MonoBehaviour
    {
        private Text textElement;
        private string initialText;

        private string seperator = " | ";

        void Start()
        {
            textElement = GetComponent<Text>();
            if (textElement == null)
            {
                Debug.LogException(new MissingComponentException("Text UI element required"), this);
            }
            initialText = textElement.text;
        }

        public void SetTextPreview(string previewText)
        {
            textElement.text = initialText + seperator + previewText;
        }
    }
}
