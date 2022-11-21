using UnityEngine;

namespace Dialog.Data
{
    using ScriptableObjects;
    [System.Serializable]
    public class DialogChoiceData
    {
        [field: SerializeField] public string Text {get;set;}
        [field: SerializeField] public DialogSO NextDialog {get;set;}
    }
}

