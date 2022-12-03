using UnityEngine;
using System.Collections.Generic;

namespace Dialog.ScriptableObjects
{
    using Data.Save;
    public class DialogueItemDataSO : ScriptableObject
    {
        [field: SerializeField] public DialogueItemSaveData Data{get;set;}
        [field: SerializeField] public string Count {get;set;}
        [field: SerializeField] public object Type {get;set;}

        public void Initialize(DialogueItemSaveData data,string count,object type) 
        {
            Data = data;
            Count = count;
            Type = type;
        }

    }
}