using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;

namespace DialogEditor
{
    
    using Utilities;
    public class WindowEditor : EditorWindow
    {
        private string defaultFileName = "New Dialog name";
        private TextField _fileNameTextField;
        private Button _saveButton;
        private string _path = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogVariables.uss";

        //DialogVariables
        private string _pathToolbar = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogToolbarStyles.uss";
        [MenuItem("Dialog/UIWindows/DialogWindows")]
        public static void Open()
        {
           WindowEditor editorWindow = GetWindow<WindowEditor>();
           editorWindow.titleContent = new GUIContent("Dialog Graph");
        }   

        private void OnEnable()
        {
            AddGraphView();
            AddToolBar();
            AddStyles();
        }

        private void AddGraphView()
        {
            DialogGraphView graphView = new DialogGraphView(this);
            graphView.StretchToParentSize();
            
            rootVisualElement.Add(graphView);
        }
        private void AddToolBar()
        {
            Toolbar toolBar = new Toolbar();

            TextField fileNameTextField = DialogElementUtility.CreatTextField(defaultFileName, "File Name:", callback => 
            {
                _fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            _saveButton = DialogElementUtility.CreateButton("Save");

            toolBar.Add(fileNameTextField);
            toolBar.Add(_saveButton);

            toolBar.AddStyleSheets(_pathToolbar);

            rootVisualElement.Add(toolBar);
        }

        private void AddStyles()
        {
           rootVisualElement.AddStyleSheets(_path);
        }

        public void EnableSaving()
        {
            _saveButton.SetEnabled(true);
        }
        public void DisableSaving()
        {
            _saveButton.SetEnabled(false);
        }

    }
}
