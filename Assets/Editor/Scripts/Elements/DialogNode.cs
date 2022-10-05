using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace DialogEditor.Elements
{
    public class DialogNode : Node
    {
       public string DialogName {get;set;}
       public List<string> Choices {get;set;}
       public string Text {get;set;}
       public DialogueType _typeDialog {get;set;}

       public void Intialize(Vector2 position)
       {
            DialogName = "Name Node";
            Choices = new List<string>();
            Text = "Dialog text";
            _typeDialog = DialogueType.None;

            SetPosition(new Rect(position,Vector2.zero));
       }

       public void Draw()
       {
            TextField dialogNameTextField = new TextField()
            {
                value = DialogName  
            };

            titleContainer.Insert(0,dialogNameTextField);

            Port inputPort = InstantiatePort(Orientation.Horizontal,Direction.Input,Port.Capacity.Multi,typeof(bool));
            inputPort.portName = "DialogConnection";

            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = new Foldout()
            {
                text = "Dialog Text"
            };

            TextField textTextFiald = new TextField()
            {
                value = Text
            };

            textFoldout.Add(textTextFiald);

            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
       }
    }
}

