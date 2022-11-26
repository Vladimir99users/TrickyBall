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
                Text = "New Choice",
                Condition = new DialogConditionSaveData()
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
                    Text = "New Choice",
                    Condition = new DialogConditionSaveData()
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
            TextField choiceTextFiled = DialogElementUtility.CreatTextField(choiceData.Text, null, callback =>{
                choiceData.Text = callback.newValue;
            });
            TextField NameTextFiled = DialogElementUtility.CreatTextField(choiceData.Condition.Name,null, callback =>{
                choiceData.Condition.Name = callback.newValue;
            });
            TextField CountTextFiled = DialogElementUtility.CreatTextField(choiceData.Condition.Count,null, callback =>{
                choiceData.Condition.Count = callback.newValue;
            });
            choiceTextFiled.AddClasses(
                "dialog-node_textfield",
                "dialog-node_choice-textfield",
                "dialog-node_textfield_hidden"
            );
            choicePort.Add(choiceTextFiled);
            choicePort.Add(CountTextFiled);
            choicePort.Add(NameTextFiled);
            choicePort.Add(deleteChoiceButton);
            
            return choicePort;
        }
    } 
}

