using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace DialogEditor
{
    using Utilities;
    public class WindowEditor : EditorWindow
    {
        private string _path = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogVariables.uss";
        [MenuItem("Dialog/UIWindows/DialogWindows")]
        public static void Open()
        {
           WindowEditor editorWindow = GetWindow<WindowEditor>();
           editorWindow.titleContent = new GUIContent("Dialog Graph");
        }   

        private void OnEnable()
        {
            AddGraphView();
            AddStyles();
        }
        
        private void AddStyles()
        {
           rootVisualElement.AddStyleSheets(_path);
        }

        private void AddGraphView()
        {
            DialogGraphView graphView = new DialogGraphView(this);
            graphView.StretchToParentSize();
            
            rootVisualElement.Add(graphView);
        }

    }
}
