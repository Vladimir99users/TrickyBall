using UnityEngine;
using System.Collections.Generic;
using DialogEditor.Elements;


namespace Dialog.ScriptableObjects
{
    [System.Serializable]
    public class DialogueItemDataSO
    {
        [field: SerializeField] public string NameItem {get;set;}
        [field: SerializeField] public string CountItem {get;set;}
        [field: SerializeField] public string CountRequare{get;set;}
        [field: SerializeField] public TypeAction Type {get;set;}

    }
}