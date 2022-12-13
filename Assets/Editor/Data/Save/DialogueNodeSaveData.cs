using System.Collections.Generic;
using UnityEngine;

namespace Dialog.Data.Save
{
    [System.Serializable]
    public class DialogueNodeSaveData: BaseNodeSaveData
    {
        [field: SerializeField] public string Text {get;set;}
        [field: SerializeField] public List<DialogBranchData> Choices { get; set; }  
    }
}
