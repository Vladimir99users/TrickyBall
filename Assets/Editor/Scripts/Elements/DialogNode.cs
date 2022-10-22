using UnityEditor.Experimental.GraphView;
using DialogEditor.Enumerations;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace DialogEditor.Elements
{

    using Utilities;
    using DialogEditor;
    public class DialogNode : Node
    {
     public string DialogName {get;set;}
     public List<string> Choices {get;set;}
     public string Text {get;set;}

     private Color defaultBackgroundColor;
     private DialogGraphView _graphView;
     public DialogueType _typeDialog {get;set;}

          internal virtual void Intialize(DialogGraphView dialogGraphView, Vector2 position)
          {
               DialogName = "Name Node";
               Choices = new List<string>();
               Text = "Dialog text";
               _graphView = dialogGraphView;
               defaultBackgroundColor = new Color(29f / 255f,29f / 255f,30f / 255f);
               _typeDialog  = DialogueType.None;
               SetPosition(new Rect(position,Vector2.zero));
               mainContainer.AddToClassList(".dialog-node_main-container");
               extensionContainer.AddToClassList(".dialog-node_extension-container");  

          }

          internal virtual void Draw()
          {

             DrawTextFieldDialogName();
             DrawConnectors();
              // connectors
             VisualElement customDataContainer = new VisualElement();
             customDataContainer.AddClasses(".dialog-node_custom-data-container");


             Foldout textFoldout = DialogElementUtility.CreateFoldout( "Dialog text" );
             TextField textTextField = DialogElementUtility.CreatTextArea(Text);

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
             TextField dialogNameTextField = DialogElementUtility.CreatTextField(DialogName,callback => 
             {
               _graphView.RemoveUngroupNode(this);
               DialogName = callback.newValue;
               _graphView.AddUngroupeNodes(this);
             });
             // text field
             dialogNameTextField.AddClasses(
                  ".dialog-node_textfield",
                  ".dialog-node_filename-textfield",
                  ".dialog-node_textfield_hidden"
             );

               titleContainer.Insert(0,dialogNameTextField);

          }

          private void DrawConnectors()
          {
             Port inputPort = this.CreatPort("DialogConnection", Orientation.Horizontal,Direction.Input,Port.Capacity.Multi);
             inputContainer.Add(inputPort);
          }   

          public void SetErrorStyle(Color color)
          {
               mainContainer.style.backgroundColor = color;
          }

          public void ResetStyle()
          {
                mainContainer.style.backgroundColor = defaultBackgroundColor;
          }
     }
}

