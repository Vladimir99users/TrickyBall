using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using UnityEngine;


namespace DialogEditor.Elements
{
    using Utilities;
    public class SingleChoiseNode : DialogNode
    {
       internal override void Intialize(Vector2 position)
       {
            base.Intialize(position);
            _typeDialog = DialogueType.SingleChoise;

            Choices.Add("Next dialogue");
       }

        internal override void Draw()
        {
            base.Draw();

            foreach (var choice in Choices)
            {
                Port choisePort = this.CreatPort(choice);
                choisePort.portName = choice;

                outputContainer.Add(choisePort);
            }

            RefreshExpandedState();
        }
    }
}

