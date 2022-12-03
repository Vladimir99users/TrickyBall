using UnityEngine;
using System.Collections.Generic;

namespace Dialog.ScriptableObjects
{
    using Data.Save;

    public class DialogueItemDataSAVESO : MonoBehaviour
    {
        public List<DialogueItemDataSO> Items {get;set;}
        public string TextChoice {get;set;}
        public object Type {get;set;}


    }
}