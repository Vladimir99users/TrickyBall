using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using UnityEngine;


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
                Text = "Next Dialogue"
            };
            Choices.Add(choiceData);
       }

        internal override void Draw()
        {
            base.Draw();

            foreach (var choice in Choices)
            {
                Port choisePort = this.CreatPort(choice.Text);

                choisePort.userData = choice; 
                outputContainer.Add(choisePort);
            }

            RefreshExpandedState();
        }
    }
}

