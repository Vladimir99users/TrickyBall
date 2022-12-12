using UnityEngine;

namespace Dialog.Data
{
    using System.Collections.Generic;
    using ScriptableObjects;
    [System.Serializable]
    public class DialogChoiceData
    {
        [field: SerializeField] public string Text {get;set;}
        [field: SerializeField] public List<DialogueItemDataSO> Data {get;set;}
        [field: SerializeField] public DialogSO NextDialog {get;set;}
    }
}

