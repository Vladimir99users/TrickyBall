using System.Collections.Generic;
using UnityEngine;
using DialogEditor.Enumerations;
using Dialog.Data;

namespace Dialog.ScriptableObjects
{

    public class DialogSO : ScriptableObject
    {
        [field: SerializeField] public string DialogName {get;set;}
        [field: SerializeField] public string Text {get;set;}
        [field: SerializeField] public List<DialogChoiceData> Choices {get;set;}
        [field: SerializeField] public DialogueType Type {get;set;}
        [field: SerializeField] public bool IsStartingDialogue {get;set;}

        public void Initialize(string dialogName,string text,List<DialogChoiceData> choices, DialogueType type, bool isStartingDialogue)
        {
            DialogName = dialogName;
            Text = text;
            Choices = choices;
            Type = type;
            IsStartingDialogue = isStartingDialogue;
        }

    }
}

