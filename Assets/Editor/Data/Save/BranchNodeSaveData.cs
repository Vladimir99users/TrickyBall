using UnityEngine;
using Dialog.ScriptableObjects;
using System.Collections.Generic;

namespace Dialog.Data.Save
{
    [System.Serializable]
    public class BranchNodeSaveData : BaseNodeSaveData
    {
        public string NextIDNode {get;set;}
        [field: SerializeField] public List<DialogueItemDataSO> ItemData {get;set;}
    
        
    }
}
