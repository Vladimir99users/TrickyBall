using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DialogEditor.Elements
{
    using Utilities;
    using Dialog.Data.Save;
    

    public class SingleChoiseNode : DialogNode
    {

        private DialogChoiseSaveData choiceData;
       internal override void Intialize(string nodeName,DialogGraphView dialogGraphView, Vector2 position)
       {
            base.Intialize(nodeName,dialogGraphView,position);
            _typeDialog = DialogueType.SingleChoise;
            choiceData = new DialogChoiseSaveData()
            {
                Text = "Next Dialogue",
                Item = "name"
                
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




          /*  ObjectField objectField = new ObjectField()
            {
                objectType = typeof(LocalItem),
                allowSceneObjects = false,
                value = null

            };



            objectField.RegisterValueChangedCallback(callback =>
            {
                 choiceData.Item = objectField.value as LocalItem;
                 Debug.Log(choiceData.Item.Name);
            });*/

            
          
            //extensionContainer.Add(itemName);
            RefreshExpandedState();
        }
    }
}

