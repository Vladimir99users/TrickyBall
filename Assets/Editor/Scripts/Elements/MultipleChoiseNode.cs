using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor.Elements
{
    using Utilities;
    public class MultipleChoiseNode : DialogNode
    {
       internal override void Intialize(DialogGraphView dialogGraphView, Vector2 position)
       {
            base.Intialize(dialogGraphView, position);
            _typeDialog = DialogueType.MultipleChoise;
            Choices.Add("new Choise");
       }

        internal override void Draw()
        {
            base.Draw();
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
                Port choicePort = CreateSingleChoice("New Choice");
                Choices.Add("New Choise");

                outputContainer.Add(choicePort);
            } );

            addChoiceButton.AddToClassList(".dialog-node_button");
            mainContainer.Insert(1,addChoiceButton);
        }

        private Port CreateSingleChoice(string textChoice)
        {
            
                Port choicePort = this.CreatPort("Choice",Orientation.Horizontal,Direction.Output,Port.Capacity.Single);
                Button deleteChoiceButton = DialogElementUtility.CreateButton("Delete", () =>{
                    
                });

                deleteChoiceButton.AddToClassList(".dialog-node__button");

                TextField choiceTextFiled = DialogElementUtility.CreatTextField(textChoice);

                choiceTextFiled.AddClasses(
                    ".dialog-node_textfield",
                    ".dialog-node_choice-textfield",
                    ".dialog-node_textfield_hidden"
                );

                choicePort.Add(choiceTextFiled);
                choicePort.Add(deleteChoiceButton);
                
                return choicePort;
        }
    } 
}

