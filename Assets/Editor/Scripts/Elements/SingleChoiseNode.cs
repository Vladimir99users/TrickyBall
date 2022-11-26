using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor.Elements
{
    using Utilities;
    using Dialog.Data.Save;
  

    public class SingleChoiseNode : DialogNode
    {
       internal override void Intialize(string nodeName,DialogGraphView dialogGraphView, Vector2 position)
       {
            base.Intialize(nodeName,dialogGraphView,position);
            _typeDialog = DialogueType.SingleChoise;
            DialogChoiseSaveData choiceData = new DialogChoiseSaveData()
            {
                Text = "Next Dialogue",
                Condition = new DialogConditionSaveData()
            };
            Choices.Add(choiceData);
       }

        internal override void Draw()
        {
            base.Draw();

            foreach (var choice in Choices)
            {
                Port choisePort = this.CreatPort(choice.Text);
                TextField NameTextFiled = DialogElementUtility.CreatTextField(choice.Condition.Name,null, callback =>{
                    choice.Condition.Name = callback.newValue;
                });
                TextField CountTextFiled = DialogElementUtility.CreatTextField(choice.Condition.Count,null, callback =>{
                    choice.Condition.Count = callback.newValue;
                });

                choisePort.Add(CountTextFiled);
                choisePort.Add(NameTextFiled);
                choisePort.userData = choice; 
                
                outputContainer.Add(choisePort);
            }

            RefreshExpandedState();
        }
    }
}

