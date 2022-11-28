using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using Dialog.Data.Save;


namespace DialogEditor.Elements
{
    using Utilities;
    using Enumerations;

    public class ConditionNode : DialogNode
    {   
        public string ConditionNameItem {get;set;}
        public string ConditionCount {get;set;}

        internal override void Intialize(string name, DialogGraphView dialogGraphView, Vector2 position)
        {
            base.Intialize(name,dialogGraphView,position);
          
            ConditionNameItem = "Name item";
            ConditionCount = "Count item";
            _typeDialog = DialogueType.Condition;
            defaultBackgroundColor = new Color(76f/255, 137f/255, 133/255f);
            DialogChoiseSaveData choiceDataTrue = new DialogChoiseSaveData()
            {
                Text = "True"
            };
            DialogChoiseSaveData choiceDataFalse = new DialogChoiseSaveData()
            {
                Text = "False"
            };
            Choices.Add(choiceDataTrue);
            Choices.Add(choiceDataFalse);

            mainContainer.AddClasses("dialog-node_condition-container");
        }

        internal override void Draw()
        {

            DrawConnectors();
            DrawTitel();
              // connectors
            VisualElement customDataContainer = new VisualElement();

            Foldout textFoldout = DialogElementUtility.CreateFoldout( "Condition settings" );

            TextField nameField = DialogElementUtility.CreatTextField(
                ConditionNameItem,
                null,
                callback =>
                {
                    TextField target = (TextField) callback.target;
                    target.value = callback.newValue;

                    if(Group == null)
                    {
                        _graphView.RemoveUngroupeNode(this);
                        DialogName = target.value;
                        _graphView.AddUngroupeNodes(this);
                        return;
                    }

                    GroupElements currentGroup = Group;
                    _graphView.RemoveGroupedNode(this,Group);
                    ConditionNameItem = target.value;
                    DialogName = target.value;
            
                    _graphView.AddGroupedNode(this,currentGroup);
                    
                }
            );

            TextField countField = DialogElementUtility.CreatTextField(
                ConditionCount,
                null,
                callback =>
                {
                    TextField target = (TextField) callback.target;
                    target.value = callback.newValue.DeleteAllCharactersExceptNumbers();
                    
                    if(Group == null)
                    {
                        _graphView.RemoveUngroupeNode(this);
                        DialogName = target.value;
                        _graphView.AddUngroupeNodes(this);
                        return;
                    }

                    GroupElements currentGroup = Group;
                    _graphView.RemoveGroupedNode(this,Group);
                    ConditionCount = target.value;
            
                    _graphView.AddGroupedNode(this,currentGroup);
                    
                }
            );


            textFoldout.Insert(0,nameField);
            textFoldout.Insert(1,countField);

            
            Port portTrue = this.CreatPort("True");
            portTrue.userData = Choices[0];
            Port portFalse = this.CreatPort("False");
            portFalse.userData = Choices[1];
          

            outputContainer.Add(portTrue);
            outputContainer.Add(portFalse);
            

            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

        private void DrawTitel()
        {
            TextField dialogNameTextField = DialogElementUtility.CreatTextField(DialogName, null,callback => 
            {
                TextField target = (TextField) callback.target;
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

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

    }
}

