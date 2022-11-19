using System.Collections.Generic;
using UnityEngine;
using DialogEditor.Enumerations;


namespace Dialog.Data.Save
{

    [System.Serializable]
    public class DialogNodeSaveData
    {
        [field: SerializeField] public string ID {get;set;}
        [field: SerializeField] public string Name {get;set;}
        [field: SerializeField] public string Text {get;set;}
        [field: SerializeField] public List<DialogChoiseSaveData> Choices { get; set; }  
        [field: SerializeField] public string GroupID {get;set;}
        [field: SerializeField] public DialogueType Type {get;set;}
        [field: SerializeField] public Vector2 position;
    }
}
