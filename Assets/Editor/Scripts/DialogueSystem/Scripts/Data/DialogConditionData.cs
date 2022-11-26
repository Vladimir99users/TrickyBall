using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog.Data
{
    [System.Serializable]
    public class DialogConditionData
    {
        [field: SerializeField] public string NameText {get;set;}
        [field: SerializeField] public string CountText {get;set;}
    }
}

