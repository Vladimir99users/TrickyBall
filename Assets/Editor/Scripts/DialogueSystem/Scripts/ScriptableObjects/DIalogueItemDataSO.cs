using UnityEngine;
using System.Collections.Generic;

namespace Dialog.ScriptableObjects
{
    using Data.Save;
    [System.Serializable]
    public class DialogueItemDataSO
    {
        [field: SerializeField] public string NameItem {get;set;}
        [field: SerializeField] public string CountItem {get;set;}
        [field: SerializeField] public string CountRequare{get;set;}
        [field: SerializeField] public object Type {get;set;}

    }
}