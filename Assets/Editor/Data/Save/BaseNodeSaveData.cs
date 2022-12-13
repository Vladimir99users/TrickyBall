using UnityEngine;
using DialogEditor.Enumerations;

namespace Dialog.Data.Save
{
    [System.Serializable]
    public class BaseNodeSaveData
    {
        [field: SerializeField] public string Titel {get;set;}
        [field: SerializeField] public string IDGuidBaseNode {get;set;}
        [field: SerializeField] public string GroupID {get;set;}
        [field: SerializeField] public DialogueType Type {get;set;}
        [field: SerializeField] public Vector2 position;
    }
}
