using UnityEngine;
using System.Collections.Generic;

namespace Dialog.ScriptableObjects
{
    using Data.Save;

    [CreateAssetMenu(fileName ="new item",menuName ="Item/Local" )]
    public class DialogueItemDataSO : ScriptableObject
    {
        [field: SerializeField] public DialogueItemSaveData Data{get;set;}
    }
}