using System;
using TMPro;
using UnityEngine;

namespace GameEditor
{
    public class WorkInput : MonoBehaviour
    {
        public TMP_InputField workName;
        public TMP_Dropdown workCode;
        public TMP_InputField stage;

        public Work workData;

        public event Action<WorkInput> OnRemoved;

        public void Init(Work work)
        {
            workData = work;
            workName.text = workData.name;
            workCode.value = workCode.options.FindIndex(op => op.text == work.code);
            stage.text = workData.stage.ToString();
            
            workName.onValueChanged.AddListener((value) => workData.name = value);
            workCode.onValueChanged.AddListener((_) => workData.code = workCode.options[workCode.value].text);
            stage.onValueChanged.AddListener((value) => workData.stage = int.Parse(value));
        }

        public void Remove()
        {
            OnRemoved?.Invoke(this);
            Destroy(gameObject);
        }
    }
}