using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using Dialog.ScriptableObjects;

namespace DialogEditor.Elements
{
    using Dialog.Data.Save;
    using Enumerations;
    using Utilities;
    public class ChoiceNode : Node
    {
        public string ID {get;private set;}
        public string DialogName {get;set;}
        public DialogueType _typeDialog {get;set;}
        public GroupElements Group {get;set;}
        protected DialogGraphView _graphView;
        private DialogueItemDataSO _dataItem;
        private ConditionOperation _operation;
        private DialogueItemSaveData _saveData;

        public virtual void Intialize(string name, DialogGraphView dialogGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DialogName = name;
            _graphView = dialogGraphView;
            _dataItem = new DialogueItemDataSO();
            _saveData = new DialogueItemSaveData();
            _typeDialog  = DialogueType.Condition;
            SetPosition(new Rect(position,Vector2.zero));
            
            mainContainer.AddClasses("Branch_Node-size");
        }

        public virtual void Draw(){
            DrawTitelContainer();
            DrawContextMenu();
            DrawConnectors();
            
        }

        private void DrawTitelContainer()
        {
           Label titel = new Label();
           titel.text = "Choice";

            titleContainer.Insert(0,titel);
        }

        private void DrawContextMenu()
        {
            ToolbarMenu menu = new ToolbarMenu();

            menu.text = "Condition";

            menu.menu.AppendAction("Add new condition", new Action<DropdownMenuAction>(x=> AddCondition()));

            titleButtonContainer.Add(menu);
        }

        private void AddCondition(){
            Box boxContainer = new Box();
            boxContainer.AddToClassList("Branch_box-container");

            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(DialogueItemDataSO),
                allowSceneObjects = false,
                value = _dataItem
            };
           
           objectField.RegisterValueChangedCallback(callback =>
           {
                _dataItem = objectField.value as DialogueItemDataSO;
                Debug.Log(_dataItem.Data.Name);
           });
           objectField.SetValueWithoutNotify(_dataItem);
           objectField.FindAncestorUserData();

           EnumField enumField = new EnumField(ConditionOperation.Equals);
           enumField.value = _operation;
           enumField.SetValueWithoutNotify(_operation);

           enumField.RegisterValueChangedCallback(callback =>
           {
                _operation = (ConditionOperation)enumField.value;
                Debug.Log(_operation);
           });
    
           

           Button button = new Button()
           {
                text = "X"
           };
           button.clicked += () =>
           {
                DeleteBoxContainer(boxContainer);
           };


           button.AddToClassList("Branch_button");
           objectField.AddClasses("Branch_box-objectField");
           enumField.AddClasses("Branch_box-enumField");

           boxContainer.Add(button);
           boxContainer.Add(objectField);
           boxContainer.Add(enumField);

           mainContainer.Add(boxContainer);
           RefreshExpandedState();

        }

        private void DeleteBoxContainer(Box container)
        {
            mainContainer.Remove(container);
        }

        private void DrawConnectors()
        {
            Port port = InstantiatePort(Orientation.Horizontal,Direction.Input,Port.Capacity.Single,typeof(bool));
            port.portName = "IN";
            inputContainer.Add(port);
            return;
        }   
    }
}

