using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

namespace DialogEditor.Elements
{

    using Utilities;
    using DialogEditor;
    using System;
    using Dialog.Data.Save;
    using Dialog.ScriptableObjects;

    public class DialogNode : Node
    {
     public string IDcurrentNode {get;set;}
     public string DialogueName {get;set;}
     public DialogueType _typeDialog {get;set;}
     public GroupElements Group {get;set;}

     protected DialogGraphView _graphView;

     internal virtual void Intialize(string name, DialogGraphView dialogGraphView, Vector2 position)
     {
          IDcurrentNode = Guid.NewGuid().ToString();
          DialogueName = name;


          _graphView = dialogGraphView;

          _typeDialog  = DialogueType.None;
          SetPosition(new Rect(position,Vector2.zero));
          mainContainer.AddToClassList("dialog-node_main-container");
          extensionContainer.AddToClassList("dialog-node_extension-container");  
     }

     public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
     {
          evt.menu.AppendAction("Disconect Input Ports", actionEvent => DisconnectInputPorts());
          evt.menu.AppendAction("Disconect Output Ports", actionEvent => DisconnectOutputPorts());
          base.BuildContextualMenu(evt);
     }
     internal virtual void Draw()
     {
        DrawTextFieldDialogName();
        DrawConnectors();

        RefreshExpandedState();
     }
     private void DrawTextFieldDialogName()
     {
        TextField dialogNameTextField = DialogElementUtility.CreatTextField(DialogueName, null,callback => 
        {
          TextField target = (TextField) callback.target;
          target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

          if(string.IsNullOrEmpty(target.value))
          {
               if(!string.IsNullOrEmpty(DialogueName))
               {
                    ++_graphView.NameErrorsAmount;
               }
          } else 
          {
               if(string.IsNullOrEmpty(DialogueName))
               {
                    --_graphView.NameErrorsAmount;
               }
          }

          if(Group == null)
          {
               _graphView.RemoveUngroupeNode(this);
               DialogueName = target.value;
               _graphView.AddUngroupeNodes(this);
               return;
          }
          GroupElements currentGroup = Group;
          _graphView.RemoveGroupedNode(this,Group);
          DialogueName = target.value;
          _graphView.AddGroupedNode(this,currentGroup);
         
        });
       
        // text field
        dialogNameTextField.AddClasses(
             "dialog-node_textfield",
             "dialog-node_filename-textfield",
             "dialog-node_textfield_hidden"
        );
          titleContainer.Insert(0,dialogNameTextField);
     }
     private void DrawConnectors()
     {
        Port inputPort = this.CreatPort("DialogConnection", Orientation.Horizontal,Direction.Input,Port.Capacity.Multi);
        inputContainer.Add(inputPort);
     }   


     #region Utility Methods

     public void DisconnectAllPorts()
     {
          DisconnectInputPorts();
          DisconnectOutputPorts();
     }

     private void DisconnectInputPorts()
     {
          DisconnectPorts(inputContainer);
     }

     private void DisconnectOutputPorts()
     {
          DisconnectPorts(outputContainer);
     }
     private void DisconnectPorts(VisualElement container)
     {
          foreach (Port port in container.Children())
          {
               if(port.connected == false)
               {
                    continue;
               }
            _graphView.DeleteElements(port.connections);
          }
     }

     public bool IsStartingNode()
     {
          Port inputPort = (Port) inputContainer.Children().First();
          return !inputPort.connected;
     }
     public void SetErrorStyle(Color color)
     {
           mainContainer.style.backgroundColor = color;
     }

     //TODO - доделать смену стиля.
     public void ResetStyle()
     {
           Debug.Log("ResetStyle");
     }
     #endregion
    }
}

 