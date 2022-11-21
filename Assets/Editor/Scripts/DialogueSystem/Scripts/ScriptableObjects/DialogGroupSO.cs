using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog.ScriptableObjects
{
    public class DialogGroupSO : ScriptableObject
    {
        [field: SerializeField] public string GroupName {get;set;}

        public void Initialize(string groupName)
        {
            GroupName = groupName;
        }
    }
}

