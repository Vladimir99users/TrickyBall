using UnityEngine;
using System.Collections.Generic;

namespace Dialog.ScriptableObjects
{
    using Data.Save;

    public class DialogueItemDataSAVESO : MonoBehaviour
    {
        [field: SerializeField] public List<DialogueItemDataSO> Items {get;set;}
        [field: SerializeField] public string TextChoice {get;set;}
    }
}