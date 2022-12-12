using UnityEngine;
using Dialog.ScriptableObjects;
using System.Collections.Generic;

namespace Dialog.Data.Save
{
    [System.Serializable]
    public class DialogChoiseSaveData
    {

        // тут сохраняются данные для превращения в нод для диалога
        [field: SerializeField] public string ChoiceText {get;set;}
        [field: SerializeField] public string NodeID {get;set;}
        [field: SerializeField] public List<DialogueItemDataSO> ItemData {get;set;}
    }
}