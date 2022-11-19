using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog.ScriptableObjects
{
    public class DialogContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName {get;set;}
        [field: SerializeField] public SerializableDictionary<DialogGroupSO, List<DialogSO>> DialogueGroups {get;set;}
        [field: SerializeField] public List<DialogSO> UngroupedDialogues {get;set;}
    
        public void Initialize(string fileName)
        {
            FileName = fileName;
            DialogueGroups = new SerializableDictionary<DialogGroupSO, List<DialogSO>>();
            UngroupedDialogues = new List<DialogSO>();
        }
    }
}

