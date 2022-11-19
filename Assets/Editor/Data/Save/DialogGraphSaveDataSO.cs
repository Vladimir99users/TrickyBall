using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog.Data.Save
{
    public class DialogGraphSaveDataSO
    {
        [field: SerializeField]public string FileName {get;set;}
        [field: SerializeField]public List<DialogGroupSaveData> Groups {get;set;}
        [field: SerializeField]public List<DialogNodeSaveData> Nodes {get;set;}
        [field: SerializeField]public List<string> OldGroupNames {get;set;}
        [field: SerializeField]public List<string> OldUngroupedNodeNames {get;set;}
        [field: SerializeField]public SerializableDictionary<string, List<string>> OldGroupedNodeNames {get;set;}


        public void Initialize(string fileName)
        {
            FileName = fileName;
            
            Groups = new List<DialogGroupSaveData>();
            Nodes = new List<DialogNodeSaveData>();


        }
    }
}
