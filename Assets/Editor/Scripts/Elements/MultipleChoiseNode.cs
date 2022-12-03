using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor.Elements
{
    using Utilities;
    using Dialog.Data.Save;
    public class MultipleChoiseNode : DialogNode
    {
       internal override void Intialize(string nodeName, DialogGraphView dialogGraphView, Vector2 position)
       {
            base.Intialize(nodeName,dialogGraphView, position);
            _typeDialog = DialogueType.MultipleChoise;
            DialogChoiseSaveData choiceData = new DialogChoiseSaveData()
            {
                Text = "New Choice"
            };
            Choices.Add(choiceData);
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
                DialogChoiseSaveData choiceData = new DialogChoiseSaveData()
                {
                    Text = "New Choice"
                };

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
                DialogChoiseSaveData choiceData = (DialogChoiseSaveData) userData;
                
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

