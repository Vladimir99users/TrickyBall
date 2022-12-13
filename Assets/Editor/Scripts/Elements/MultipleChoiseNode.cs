using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;


namespace DialogEditor.Elements
{
    using Utilities;
    using Dialog.Data.Save;
    using Dialog.Data;

    public class MultipleChoiseNode : DialogNode
    {
       public string Text {get;set;}
       public List<DialogBranchData> Choices {get;set;}
       internal override void Intialize(string nodeName, DialogGraphView dialogGraphView, Vector2 position)
       {
            base.Intialize(nodeName,dialogGraphView, position);
            Choices = new List<DialogBranchData>();



            _typeDialog = DialogueType.MultipleChoise;
            Text = "Dialog text";

            DialogBranchData choiceData = new DialogBranchData();
            Choices.Add(choiceData);
       }

        internal override void Draw()
        {
            base.Draw();
                     // connectors
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddClasses("dialog-node_custom-data-container");
            Foldout textFoldout = DialogElementUtility.CreateFoldout( "Dialog text" );
            TextField textTextField = DialogElementUtility.CreatTextArea(Text,null, callback =>
            {
              Text = callback.newValue;
            });
            textTextField.AddClasses(
                 "dialog-node_textfield",
                 "dialog-node_quote-textfield"
            );
            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
                DrawDialogAddChoiceButton();
                foreach (var choice in Choices)
                {
                    Port choicePort = CreateSingleChoice(choice);
                    outputContainer.Add(choicePort);
                }
                RefreshExpandedState();
        }

        private void DrawDialogAddChoiceButton()
        {
            Button addChoiceButton = DialogElementUtility.CreateButton("Add choice", () =>
            {
                DialogBranchData choiceData = new DialogBranchData();

                Choices.Add(choiceData);
                
                Port choicePort = CreateSingleChoice(choiceData);

                outputContainer.Add(choicePort);
            } );

            addChoiceButton.AddToClassList("dialog-node_button");
            mainContainer.Insert(1,addChoiceButton);
        }

        private Port CreateSingleChoice(object userData)
        {
            
                Port choicePort = this.CreatPort();
                
                choicePort.userData = userData;
                DialogBranchData choiceData = (DialogBranchData) userData;
                
                Button deleteChoiceButton = DialogElementUtility.CreateButton("Delete", () =>
                {
                    if(Choices.Count == 1)
                    {
                        return;
                    }    

                    if(choicePort.connected)
                    {
                        _graphView.DeleteElements(choicePort.connections);
                    }

                    Choices.Remove(choiceData);

                    _graphView.RemoveElement(choicePort);
                });

                deleteChoiceButton.AddToClassList("dialog-node__button");

                choicePort.Add(deleteChoiceButton);
                
                return choicePort;
        }
    } 
}

