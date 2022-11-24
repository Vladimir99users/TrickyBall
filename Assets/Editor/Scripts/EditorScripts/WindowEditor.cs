using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.IO;

namespace DialogEditor
{
    using Utilities;
    public class WindowEditor : EditorWindow
    {
        private DialogGraphView _graphView;
        private string _defaultFileName = "New Dialog name";
        private static TextField _fileNameTextField;
        private Button _saveButton;
        private Button _clearButton;
        private Button _resetButton;
        private Button _loadButton;
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

            _fileNameTextField = DialogElementUtility.CreatTextField(_defaultFileName, "File Name:", callback => 
            {
                _fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            _saveButton =  DialogElementUtility.CreateButton("Save", () => SaveToolbarAction());
            _clearButton = DialogElementUtility.CreateButton("Clear", ()=> Clear());
            _resetButton = DialogElementUtility.CreateButton("Reset", ()=> ResetGraph());
            _loadButton = DialogElementUtility.CreateButton("Load", ()=> LoadGraph());

            Label labels = new Label();
            labels.text = "|                                                                                   |";
            

            toolBar.Add(_fileNameTextField);
            toolBar.Add(_saveButton);
            toolBar.Add(_loadButton);
            toolBar.Add(labels);
            toolBar.contentContainer.Add(_resetButton);
            toolBar.contentContainer.Add(_clearButton);

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

        private void Clear()
        {
            _graphView.ClearGraph();
        }

        private void ResetGraph()
        {
           Clear();
           UpdateFileName(_defaultFileName);
        }

        public static void UpdateFileName(string newFileName)
        {
            _fileNameTextField.value = newFileName;
        }

        private void LoadGraph()
        {
            string path = EditorUtility.OpenFilePanel("Dialogue Graphs", "Assets/Editor/Scripts/DialogueSystem/Graphs", "asset");
            if(string.IsNullOrEmpty(path))
            {
                return;
            }
            Clear();
            DialogueIOUtility.Initialize(_graphView,Path.GetFileNameWithoutExtension(path));
            DialogueIOUtility.Load();
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
