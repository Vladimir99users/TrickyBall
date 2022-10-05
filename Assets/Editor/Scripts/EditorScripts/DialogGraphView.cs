using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
namespace DialogEditor
{
    using Elements;
    public class DialogGraphView : GraphView
    {
        private string _path = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogStyle.uss";
        public DialogGraphView()
        {
            AddGridBackground();
            AddManipulators();
            AddStyles();
        }



        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale,ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateNodeContextMenu());
          
        }

        private IManipulator CreateNodeContextMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Add node", actionEvent => AddElement(CreateNode(actionEvent.eventInfo.localMousePosition)))
            );
            return contextualMenuManipulator;
        }

        private DialogNode CreateNode(Vector2 position)
        {
            DialogNode node = new DialogNode();
            node.Intialize(position);
            node.Draw();
            return node;
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load(_path);
            styleSheets.Add(styleSheet);
        }
        private void AddGridBackground()
        {
            GridBackground gridBackGround = new GridBackground();
            gridBackGround.StretchToParentSize();
            Insert(0,gridBackGround);
        }
    }
}
    