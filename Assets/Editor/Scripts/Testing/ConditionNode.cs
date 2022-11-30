using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using Dialog.Data.Save;


namespace DialogEditor.Elements
{
    using Utilities;
    using Enumerations;
    using Dialog.ScriptableObjects;

    public class ConditionNode : DialogNode
    {   

        private ObjectField _objectField;
        private EnumField _enumField;
        private DialogueItemDataSO _itemData;
        private OperationName _operation;

        internal override void Intialize(string name, DialogGraphView dialogGraphView, Vector2 position)
        {
            base.Intialize(name,dialogGraphView,position);

            _typeDialog = DialogueType.Condition;
       
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

            mainContainer.AddClasses("Branch_Node-size");
        }

        internal override void Draw()
        {

            DrawConnectors();
            DrawTitel();
              // connectors

            _objectField = new ObjectField()
            {
                objectType = typeof(DialogueItemDataSO),
                allowSceneObjects = false,
                value = _itemData
            };
           
           _objectField.RegisterValueChangedCallback(callback =>
           {
                _itemData = _objectField.value as DialogueItemDataSO;
                Debug.Log(_itemData.Data.Text);
           });
           _objectField.SetValueWithoutNotify(_itemData);
           _objectField.FindAncestorUserData();
    
          _objectField.AddClasses("Branch_box-objectField");


           _enumField = new EnumField(OperationName.Equals);
           _enumField.value = _operation;
           _enumField.SetValueWithoutNotify(_operation);

           _enumField.RegisterValueChangedCallback(callback =>
           {
                _operation = (OperationName)_enumField.value;
                Debug.Log(_operation);
           });
           


           Box boxContainer = new Box(); 
           boxContainer.AddToClassList("Branch_box-container");
           boxContainer.Add(_objectField);
           boxContainer.Add(_enumField);

            Port portTrue = this.CreatPort("True");
            portTrue.userData = Choices[0];
            Port portFalse = this.CreatPort("False");
            portFalse.userData = Choices[1];
          

            outputContainer.Add(portTrue);
            outputContainer.Add(portFalse);
            
            extensionContainer.Add(boxContainer);
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
            Button button = new Button();
            button.text = "Add condition";
            titleButtonContainer.Add(button);
        }

        private void DrawConnectors()
        {
          Port port = this.CreatPort("In", Orientation.Horizontal,Direction.Input,Port.Capacity.Single);
          inputContainer.Add(port);
        }   

    }

    public enum OperationName
    {
        Equals,
        Addition,
        Subtraction,
        More,
        Less
    }
}

