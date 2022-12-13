using UnityEngine;
using Dialog.ScriptableObjects;
namespace Dialog.Data
{

    [System.Serializable]
    public class DialogChoiceData
    {
        [field: SerializeField] public string Text {get;set;}
        [field: SerializeField] public DialogSO NextDialog {get;set;}
    }

    [System.Serializable]
    public class DialogBranchData
    {
        [field: SerializeField] public string NextNodeID {get;set;}
        [field: SerializeField] public DialogBranchSO NextDialog {get;set;}
    }
}

