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
        private DialogGraphView _graphView;
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
            _graphView = new DialogGraphView(this);
            _graphView.StretchToParentSize();
            
            rootVisualElement.Add(_graphView);
        }
        private void AddToolBar()
        {
            Toolbar toolBar = new Toolbar();

            _fileNameTextField = DialogElementUtility.CreatTextField(defaultFileName, "File Name:", callback => 
            {
                _fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            _saveButton = DialogElementUtility.CreateButton("Save", () => SaveToolbarAction());

            toolBar.Add(_fileNameTextField);
            toolBar.Add(_saveButton);

            toolBar.AddStyleSheets(_pathToolbar);

            rootVisualElement.Add(toolBar);
        }

        private void SaveToolbarAction()
        {

            if(string.IsNullOrEmpty(_fileNameTextField.value))
            {
                EditorUtility.DisplayDialog(
                    "Invalid file name",
                    "Please ensure the file name you've typed in is valid",
                    "Roger!"
                );
                return;
            }

            DialogueIOUtility.Initialize(_graphView, _fileNameTextField.value);
            DialogueIOUtility.Save();
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
