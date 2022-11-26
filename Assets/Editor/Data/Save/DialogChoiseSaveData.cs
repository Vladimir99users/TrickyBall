using UnityEngine;

namespace Dialog.Data.Save
{
    [System.Serializable]
    public class DialogChoiseSaveData
    {
        [field: SerializeField] public string Text {get;set;}
        [field: SerializeField] public string NodeID {get;set;}
        [field: SerializeField] public DialogConditionSaveData Condition {get;set;}
    }
}