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
        private List<DialogueItemDataSO> Data;

        internal override void Intialize(string name, DialogGraphView dialogGraphView, Vector2 position)
        {
            base.Intialize(name,dialogGraphView,position);

            _typeDialog = DialogueType.Condition;
            Data = new List<DialogueItemDataSO>();
            DialogChoiseSaveData choiceData = new DialogChoiseSaveData()
            {
                ChoiceText = "out",
                ItemData = Data
            };

            Choices.Add(choiceData);
            defaultBackgroundColor = new Color(0,0,0,255);
            mainContainer.AddClasses("Branch_Node-size");
        }

        internal override void Draw()
        {
            base.Draw();

            DrawContextMenu();
           

            Port portTrue = this.CreatPort("out");
            portTrue.userData = Choices[0];

            outputContainer.Add(portTrue);
            
            RefreshExpandedState();
        }
      
        private void DrawContextMenu()
        {
            ToolbarMenu menu = new ToolbarMenu();

            menu.text = "Condition";
            menu.text = "Set item";

           // menu.menu.AppendAction("Add new condition", new Action<DropdownMenuAction>(x=> AddCondition(ConditionOperation.Equals)));
            menu.menu.AppendAction("Set a new value", new Action<DropdownMenuAction>(x=> SetNewValue(MathOperation.Addition)));

            titleButtonContainer.Add(menu);
        }
        /*
        private void AddCondition(ConditionOperation operation){
            Box boxContainer = new Box();
            DialogueItemDataSO item = new DialogueItemDataSO();
            boxContainer.AddToClassList("Branch_box-container");

            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(DialogueItemSaveData),
                allowSceneObjects = false,
                value = item.Item
            };
            
            objectField.RegisterValueChangedCallback(callback =>
            {
                 item.Item = objectField.value as DialogueItemSaveData;
            });

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
                item.CountRequare = textNumber.value.ToString();
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
*/
        private void SetNewValue(MathOperation operation)
        {
            Box boxContainer = new Box();
            boxContainer.AddToClassList("Branch_box-container");
            DialogueItemDataSO item = new DialogueItemDataSO();
            DialogueItemSaveData dataSO;
            ObjectField objectField = new ObjectField()
            {
                objectType = typeof(DialogueItemSaveData),
                allowSceneObjects = false,

            };
           
           objectField.RegisterValueChangedCallback(callback =>
           {
                dataSO = objectField.value as DialogueItemSaveData;
                item.NameItem = dataSO.Name;
                item.CountItem = dataSO.Count;
           });
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
                item.CountRequare = textNumber.value.ToString();
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

