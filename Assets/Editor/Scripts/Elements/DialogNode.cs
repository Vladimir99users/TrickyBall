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

    public class DialogNode : Node
    {
     public string ID {get;set;}
     public string DialogName {get;set;}
     public List<DialogChoiseSaveData> Choices {get;set;}
     public string Text {get;set;}

     private Color defaultBackgroundColor;
     protected DialogGraphView _graphView;
     public DialogueType _typeDialog {get;set;}

     public GroupElements Group {get;set;}

     internal virtual void Intialize(DialogGraphView dialogGraphView, Vector2 position)
     {
          ID = Guid.NewGuid().ToString();
          DialogName = "Name Node";
          Choices = new List<DialogChoiseSaveData>();
          Text = "Dialog text";
          _graphView = dialogGraphView;
          defaultBackgroundColor = new Color(29f / 255f,29f / 255f,30f / 255f);
          _typeDialog  = DialogueType.None;
          SetPosition(new Rect(position,Vector2.zero));
          mainContainer.AddToClassList("dialog-node_main-container");
          extensionContainer.AddToClassList("dialog-node_extension-container");  
     }
     // Добавление в контекстное меню дополнительного функционала
     // Сначала название действия, потом кидаем делегат на функцию
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
         // connectors
        VisualElement customDataContainer = new VisualElement();
        customDataContainer.AddClasses("dialog-node_custom-data-container");
        Foldout textFoldout = DialogElementUtility.CreateFoldout( "Dialog text" );
        TextField textTextField = DialogElementUtility.CreatTextArea(Text,null, callback =>
        {
          Text = callback.newValue;
        });
        textTextField.AddClasses(
             ".dialog-node_textfield",
             ".dialog-node_quote-textfield"
        );
        textFoldout.Add(textTextField);
        customDataContainer.Add(textFoldout);
        extensionContainer.Add(customDataContainer);
        RefreshExpandedState();
     }
     private void DrawTextFieldDialogName()
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
     public void ResetStyle()
     {
           mainContainer.style.backgroundColor = defaultBackgroundColor;
     }
     #endregion
    }
}

 