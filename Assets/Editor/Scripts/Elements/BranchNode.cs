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
    public class BranchNode : DialogNode
    {   
        public List<DialogueItemDataSO> Data;
        public string TargetNodeID;
        public BranchNodeSaveData NextNode;

        internal override void Intialize(string name, DialogGraphView dialogGraphView, Vector2 position)
        {
            base.Intialize(name,dialogGraphView,position);

            TargetNodeID = "";

            _typeDialog = DialogueType.Branch;
            Data = new List<DialogueItemDataSO>();
            mainContainer.AddClasses("Branch_Node-size");
        }

        internal override void Draw()
        {
            base.Draw();

            DrawContextMenu();
           
            Port portTrue = this.CreatPort("out");
            TargetNodeID = (string) portTrue.userData; 
            outputContainer.Add(portTrue);
            
            RefreshExpandedState();
        }
      
        private void DrawContextMenu()
        {
            ToolbarMenu menu = new ToolbarMenu();

            menu.text = "Condition";
            menu.text = "Set item";

            menu.menu.AppendAction("Set a new value", new Action<DropdownMenuAction>(x=> SetNewAction(TypeAction.None)));

            titleButtonContainer.Add(menu);
        }

        private void SetNewAction(TypeAction operation)
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
                item.Type = (TypeAction)enumField.value;
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

