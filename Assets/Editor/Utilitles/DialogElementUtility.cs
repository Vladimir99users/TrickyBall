using UnityEngine;
using UnityEngine.UIElements;
using System;

using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;


namespace DialogEditor.Utilities
{
    using Elements;
    public static class DialogElementUtility
    {

        public static Button CreateButton(string text, Action onClick = null)
        {   
            Button button = new Button(onClick)
            {
                text = text
            };
            return button;
        }
        public static Foldout CreateFoldout(string title, bool collapse = false)
        {
            Foldout foldout =  new Foldout()
            {
                text = title,
                value = !collapse
            };

            return foldout;
        }
        public static TextField CreatTextField(string value = null,string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if(onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }
            return textField;
        }

        public static TextField CreatTextArea(string value = null,string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreatTextField(value,label,onValueChanged);
            textArea.multiline = true;

            return textArea;
        }
    
        public static Port CreatPort(this DialogNode node, string portName = "", 
                                        Orientation orientation = Orientation.Horizontal,
                                            Direction direction = Direction.Output,
                                                Port.Capacity capasity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation,direction,capasity,typeof(bool));
            port.portName = portName;
            return port;
        }
    }
}

