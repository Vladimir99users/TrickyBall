using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DialogEditor
{
    using Elements;
    using Enumerations;
    using Utilities;
    using Windows;
    using Data.Error;
    public class DialogGraphView : GraphView
    {
        private string _pathDialogGraphSteel = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogStyle.uss";
        private string _pathNodeSteel = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogNodeSteel.uss";
        
        private DialogSearchWindow _searchWindows;
        private WindowEditor _editorWindow;
        private SerializableDictionary<string,DialogNodeErrorData> _ungroupeNodes;
        private SerializableDictionary<Group, SerializableDictionary<string,DialogNodeErrorData>> _groupedNodes;

        public DialogGraphView(WindowEditor editorWindow)
        {
            _editorWindow = editorWindow;
            _ungroupeNodes = new SerializableDictionary<string, DialogNodeErrorData>();
            _groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DialogNodeErrorData>>();

            AddGridBackground();
            AddSearchWindows();
            AddManipulators();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();

            AddStyles();
        }
 
        private void AddSearchWindows()
        {
            if(_searchWindows == null)
            {
                _searchWindows = ScriptableObject.CreateInstance<DialogSearchWindow>();

                _searchWindows.Initialize(this);
            }

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindows);
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePort = new List<Port>();

            ports.ForEach(port => 
            {
                if(startPort == port)
                {
                    return;
                }
                if(startPort.node == port.node)
                {
                    return;
                }

                if(startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePort.Add(port);
            });

            return compatiblePort;
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale,ContentZoomer.DefaultMaxScale);
            ManipulationOnGraph();
            ManipulationOnAddNewNodeInContextMenu();          
        }

        private void ManipulationOnGraph()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
        }

        private void ManipulationOnAddNewNodeInContextMenu()
        {
            this.AddManipulator(CreateNodeContextMenu("Single Node", DialogueType.SingleChoise));
            this.AddManipulator(CreateNodeContextMenu("Multi Node", DialogueType.MultipleChoise));
            this.AddManipulator(CreateGroupContextMenu());
            this.AddManipulator(CreateConditionContextMenu());
        }

        private IManipulator CreateGroupContextMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );
            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextMenu(string actionTitle, DialogueType typeNode)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(typeNode,GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );
            return contextualMenuManipulator;
        }

        private IManipulator CreateConditionContextMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Condtion", actionEvent => AddElement(CreateCondition(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );
            return contextualMenuManipulator;
        }

        internal DialogNode CreateNode(DialogueType typeNode,Vector2 position)
        {
            Type nodeType = Type.GetType($"DialogEditor.Elements.{typeNode}Node");

            DialogNode node = (DialogNode) Activator.CreateInstance(nodeType);
            node.Intialize(this,position);
            node.Draw();

            AddUngroupeNodes(node);
            return node;
        }

        public void AddUngroupeNodes(DialogNode node)
        {
            string nodeName = node.DialogName;
            if(!_ungroupeNodes.ContainsKey(nodeName))
            {
                DialogNodeErrorData nodeErrorData = new DialogNodeErrorData();
                nodeErrorData.Nodes.Add(node);

                _ungroupeNodes.Add(nodeName,nodeErrorData);
                return;
            }
            List<DialogNode> ungroupNodesList = _ungroupeNodes[nodeName].Nodes;
            ungroupNodesList.Add(node);
            Color errorColor = _ungroupeNodes[nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);

            if(ungroupNodesList.Count == 2)
            {
                ungroupNodesList[0].SetErrorStyle(errorColor);
            }
        }


        public void RemoveUngroupeNode(DialogNode node)
        {
            string nodeName = node.DialogName;
            List<DialogNode> ungroupNodesList = _ungroupeNodes[nodeName].Nodes;

            ungroupNodesList.Remove(node);
            node.ResetStyle();

            if(_ungroupeNodes[nodeName].Nodes.Count == 1)
            {
                ungroupNodesList[0].ResetStyle();
                return;
            }
            if(_ungroupeNodes[nodeName].Nodes.Count == 0)
            {
                _ungroupeNodes.Remove(nodeName);
            }
        }
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName,AskUser) =>
            {
                List<DialogNode> nodeToDelete = new List<DialogNode>();
                foreach (GraphElement elemtn in selection)
                {
                    if(elemtn is DialogNode)
                    {
                        nodeToDelete.Add((DialogNode) elemtn);
                        continue;
                    }
                }

                foreach (var node in nodeToDelete)
                {
                    if(node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }
                    RemoveUngroupeNode(node);
                    RemoveElement(node);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group,elements) =>
            {
                foreach (var element in elements)
                {
                    if(!(element is DialogNode))
                    {
                        continue;
                    }

                    DialogNode node = (DialogNode) element;
                    RemoveUngroupeNode(node);
                    AddGroupedNode(node,group);
                }
            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group,elements) =>
            {
                foreach (var element in elements)
                {
                    if(!(element is DialogNode))
                    {
                        continue;
                    }

                    DialogNode node = (DialogNode) element;
                    RemoveGroupedNode(node,group);
                    AddUngroupeNodes(node);
                }
            };
        }

        public void RemoveGroupedNode(DialogNode node, Group group)
        {
            string nodeName = node.DialogName;
            node.Group = null;
            List<DialogNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);
            node.ResetStyle();

            if(groupedNodesList.Count == 1)
            {
                groupedNodesList[0].ResetStyle();
                return;
            }
            if(groupedNodesList.Count == 0)
            {
                _groupedNodes[group].Remove(nodeName);
                if(_groupedNodes[group].Count == 0)
                {
                    _groupedNodes.Remove(group);
                }
            }
        }

        public void AddGroupedNode(DialogNode node, Group group)
        {
            string nodeName = node.DialogName;
            node.Group = group;
            if(_groupedNodes.ContainsKey(group) == false)
            {
                _groupedNodes.Add(group,new SerializableDictionary<string,DialogNodeErrorData>());
            }

            if(_groupedNodes[group].ContainsKey(nodeName) == false)
            {
                DialogNodeErrorData nodeErrorData = new DialogNodeErrorData();
                nodeErrorData.Nodes.Add(node);
                _groupedNodes[group].Add(nodeName,nodeErrorData);
                return;
            }

            List<DialogNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;
            groupedNodesList.Add(node);
            Color errorColor = _groupedNodes[group][nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);

            if(groupedNodesList.Count == 2)
            {
                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        internal Group CreateGroup(Vector2 position)
        {
            GroupElements _group = new GroupElements("Empty group", position);

            return _group;
        }
        internal DialogEditor.Condition.ConditionNode CreateCondition(Vector2 position)
        {
            DialogEditor.Condition.ConditionNode condition = new DialogEditor.Condition.ConditionNode()
            {
                title = "Single if"
            };
            condition.Intialize(position);
            condition.Draw();
            return condition;
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                _pathDialogGraphSteel,
                _pathNodeSteel
            );
        }
        
        private void AddGridBackground()
        {
            GridBackground gridBackGround = new GridBackground();
            gridBackGround.StretchToParentSize();
            Insert(0,gridBackGround);
        }

        // Метод пересчета позиции из-за зума в Graph View
        
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindows = false)
        {
            Vector2 worldMousePosition = mousePosition;
            if(isSearchWindows)
            {
                worldMousePosition -= _editorWindow.position.position; // Rect -> Vecotr2
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }
    }
}
    