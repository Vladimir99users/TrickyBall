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
        private List<DialogueItemDataSO> Data;
        private int _count;

        internal override void Intialize(string name, DialogGraphView dialogGraphView, Vector2 position)
        {
            base.Intialize(name,dialogGraphView,position);

            _typeDialog = DialogueType.Choice;
            Data = new List<DialogueItemDataSO>();
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
            DialogueItemDataSO item = new DialogueItemDataSO();
            boxContainer.AddToClassList("Branch_box-container");

            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(DialogueItemSaveData),
                allowSceneObjects = false,
                value = item.Data
            };
            
            objectField.RegisterValueChangedCallback(callback =>
            {
                 item.Data = objectField.value as DialogueItemSaveData;
            });
            objectField.SetValueWithoutNotify(item);
            objectField.FindAncestorUserData();
 
            EnumField enumField = new EnumField(operation);
            enumField.value = _conditionOperation;
            enumField.SetValueWithoutNotify(_conditionOperation);
 
            enumField.RegisterValueChangedCallback(callback =>
            {
                 item.Type = (ConditionOperation)enumField.value;
                 Debug.Log(item.Type);
            });

            IntegerField textNumber = new IntegerField();
            textNumber.RegisterValueChangedCallback(callback => 
            {
                item.Count = textNumber.value.ToString();
            });
           
           Button button = new Button()
           {
                text = "X"
           };
           button.clicked += () =>
           {
                DeleteBoxContainer(boxContainer,item);
           };


           button.AddToClassList("Branch_button");
           objectField.AddClasses("Branch_box-objectField");
           enumField.AddClasses("Branch_box-enumField");
           textNumber.AddClasses("Branch_box-integerField");

           boxContainer.Insert(0,button);
           boxContainer.Insert(1,objectField);
           boxContainer.Insert(2,enumField);
           boxContainer.Insert(3,textNumber);
           Data.Add(item);
           mainContainer.Add(boxContainer);
           RefreshExpandedState();

        }

        private void SetNewValue(MathOperation operation)
        {
            Box boxContainer = new Box();
            boxContainer.AddToClassList("Branch_box-container");
            DialogueItemDataSO item = new DialogueItemDataSO();

            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(DialogueItemSaveData),
                allowSceneObjects = false,
                value = item.Data
            };
           
           objectField.RegisterValueChangedCallback(callback =>
           {
                item.Data = objectField.value as DialogueItemSaveData;
           });

           objectField.SetValueWithoutNotify(item);
           objectField.FindAncestorUserData();

           EnumField enumField = new EnumField(operation);
           enumField.value = operation;
           enumField.SetValueWithoutNotify(operation);

           enumField.RegisterValueChangedCallback(callback =>
           {
                item.Type = (MathOperation)enumField.value;
                Debug.Log(item.Type);
           });

            IntegerField textNumber = new IntegerField();
            textNumber.RegisterValueChangedCallback(callback => 
            {
                item.Count = textNumber.value.ToString();
            });
           

           Button button = new Button()
           {
                text = "X"
           };
           button.clicked += () =>
           {
                DeleteBoxContainer(boxContainer,item);
           };


           button.AddToClassList("Branch_button");
           objectField.AddClasses("Branch_box-objectField");
           enumField.AddClasses("Branch_box-enumField");
           textNumber.AddClasses("Branch_box-integerField");

           boxContainer.Insert(0,button);
           boxContainer.Insert(1,objectField);
           boxContainer.Insert(2,enumField);
           boxContainer.Insert(3,textNumber);
           Data.Add(item);
           mainContainer.Add(boxContainer);
           RefreshExpandedState();
        }
        private void DeleteBoxContainer(Box container, DialogueItemDataSO data)
        {
            Data.Remove(data);
            mainContainer.Remove(container);
        }



    }
}

