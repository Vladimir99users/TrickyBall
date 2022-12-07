using UnityEngine;
using Dialog.ScriptableObjects;
using System.Collections.Generic;

namespace Dialog.Data.Save
{
    [System.Serializable]
    public class DialogChoiseSaveData
    {
        [field: SerializeField] public List<DialogueItemDataSO> Data {get;set;}
        [field: SerializeField] public string NodeID {get;set;}
        [field: SerializeField] public string Text {get;set;}
    }
}