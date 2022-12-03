using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using Dialog.Data.Save;
using System.Collections.Generic;


namespace DialogEditor.Elements
{
    using Utilities;
    using Enumerations;
    using Dialog.ScriptableObjects;
    using System;
    public class ConditionNode : DialogNode
    {   
        private ConditionOperation _conditionOperation;
        private MathOperation _mathOperation;
        private DialogueItemDataSO _dataItem;
        private int _count;

        internal override void Intialize(string name, DialogGraphView dialogGraphView, Vector2 position)
        {
            base.Intialize(name,dialogGraphView,position);

            _typeDialog = DialogueType.Choice;
            _dataItem = new DialogueItemDataSO();
            DialogChoiseSaveData choiceData = new DialogChoiseSaveData()
            {
                Text = "out"
            };

            Choices.Add(choiceData);
            defaultBackgroundColor = new Color(0,0,0,255);
            mainContainer.AddClasses("Branch_Node-size");
        }

        internal override void Draw()
        {

            DrawTitel();
            DrawConnectors();
            DrawContextMenu();
           

            Port portTrue = this.CreatPort("out");
            portTrue.userData = Choices[0];

            outputContainer.Add(portTrue);
            
            RefreshExpandedState();
        }
        private void DrawTitel()
        {
            TextField dialogNameTextField = DialogElementUtility.CreatTextField(DialogName, null,callback => 
            {
                TextField target = (TextField) callback.target;
                target.value = callback.newValue.RemoveSpecialCharacters();

                if(string.IsNullOrEmpty(target.value))
                {
                     if(!string.IsNullOrEmpty(DialogName))
                     {
                          ++_graphView.NameErrorsAmount;
                     }
                } else 
                {
                     if(string.IsNullOrEmpty(DialogName))
                     {
                          --_graphView.NameErrorsAmount;
                     }
                }

                if(Group == null)
                {
                     _graphView.RemoveUngroupeNode(this);
                     DialogName = target.value;
                     _graphView.AddUngroupeNodes(this);
                     return;
                }
                GroupElements currentGroup = Group;
                _graphView.RemoveGroupedNode(this,Group);
                DialogName = target.value;
                _graphView.AddGroupedNode(this,currentGroup);

            });

            dialogNameTextField.AddClasses(
             "dialog-node_textfield",
             "dialog-node_filename-textfield",
             "dialog-node_textfield_hidden"
            );

            titleContainer.Insert(0,dialogNameTextField);

        }

        private void DrawConnectors()
        {
          Port port = this.CreatPort("In", Orientation.Horizontal,Direction.Input,Port.Capacity.Single);
          inputContainer.Add(port);
        }   
        
        private void DrawContextMenu()
        {
            ToolbarMenu menu = new ToolbarMenu();

            menu.text = "Condition";
            menu.text = "Set item";

            menu.menu.AppendAction("Add new condition", new Action<DropdownMenuAction>(x=> AddCondition(ConditionOperation.Equals)));
            menu.menu.AppendAction("Set a new value", new Action<DropdownMenuAction>(x=> SetNewValue(MathOperation.Addition)));

            titleButtonContainer.Add(menu);
        }
        private void AddCondition(ConditionOperation operation){
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
           });
           objectField.SetValueWithoutNotify(_dataItem);
           objectField.FindAncestorUserData();

           EnumField enumField = new EnumField(operation);
           enumField.value = _conditionOperation;
           enumField.SetValueWithoutNotify(_conditionOperation);

           enumField.RegisterValueChangedCallback(callback =>
           {
                _conditionOperation = (ConditionOperation)enumField.value;
                Debug.Log(_conditionOperation);
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

        private void SetNewValue(MathOperation operation)
        {
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
           });

           objectField.SetValueWithoutNotify(_dataItem);
           objectField.FindAncestorUserData();

           EnumField enumField = new EnumField(operation);
           enumField.value = operation;
           enumField.SetValueWithoutNotify(operation);

           enumField.RegisterValueChangedCallback(callback =>
           {
                _mathOperation = (MathOperation)enumField.value;
                Debug.Log(_mathOperation);
           });

           IntegerField textNumber = new IntegerField();
            textNumber.RegisterValueChangedCallback(callback => 
            {
                _count = textNumber.value;
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
           textNumber.AddClasses("Branch_box-integerField");

           boxContainer.Insert(0,button);
           boxContainer.Insert(1,objectField);
           boxContainer.Insert(2,enumField);
           boxContainer.Insert(3,textNumber);

           mainContainer.Add(boxContainer);
           RefreshExpandedState();
        }
        private void DeleteBoxContainer(Box container)
        {
            mainContainer.Remove(container);
        }



    }
}

