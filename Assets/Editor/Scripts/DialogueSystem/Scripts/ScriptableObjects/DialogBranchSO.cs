using System.Collections.Generic;
using UnityEngine;
using DialogEditor.Enumerations;
using Dialog.Data;
using Dialog.Data.Save;

namespace Dialog.ScriptableObjects
{
    public class DialogBranchSO: ScriptableObject
    {
        [field: SerializeField] public string DialogName {get;set;}
        [field: SerializeField] public string NextID {get;set;}
        [field: SerializeField] public List<DialogueItemDataSO> ItemData {get;set;}
        [field: SerializeField] public DialogChoiceData Next {get;set;}
        
        [field: SerializeField] public DialogueType Type {get;set;}

        public void Initialize(string dialogName,string id,List<DialogueItemDataSO> itemData, DialogueType type, bool isStartingDialogue)
        {
            DialogName = dialogName;
            NextID = id;
            ItemData = itemData;
            Type = type;
        }
    }
}

