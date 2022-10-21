using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;


namespace DialogEditor.Condition
{
    using Utilities;
    public class ConditionNode : Node
    {   
        public string ConditionNameItem {get;set;}
        public string ConditionCount {get;set;}

        public List<string> Choices;

        internal virtual void Intialize(Vector2 postion)
        {
            ConditionNameItem = "Name item";
            ConditionCount = "count item";
            Choices = new List<string>();
            Choices.Add("Next dialogue");
            SetPosition(new Rect(postion,Vector2.zero));
        }

        internal virtual void Draw()
        {

            DrawConnectors();
              // connectors
            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = DialogElementUtility.CreateFoldout( "Condition" );
        

            textFoldout.Insert(0,new TextField("Name =>")
            {
                value = ConditionNameItem
            });

            textFoldout.Insert(1,new TextField("Count =>")
            {
                value = ConditionCount
            });


            foreach (var choice in Choices)
            {
                Port port = InstantiatePort(Orientation.Horizontal,Direction.Output,Port.Capacity.Single,typeof(bool));
                port.portName = "Yes";

                outputContainer.Add(port);
            }

            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }


        private void DrawConnectors()
        {
          Port port = InstantiatePort(Orientation.Horizontal,Direction.Input,Port.Capacity.Single,typeof(bool));
          port.portName = "In";
          inputContainer.Add(port);
        }   

    }
}

