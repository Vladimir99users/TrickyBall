using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dialog.Data.Save
{
    [System.Serializable]
    [CreateAssetMenu(fileName ="new item",menuName ="Item/Local" )]
    public class DialogueItemSaveData : ScriptableObject
    {
        [field: SerializeField] public string Name {get;set;}
        [field: SerializeField] public string Count {get;set;}     
    }
}

