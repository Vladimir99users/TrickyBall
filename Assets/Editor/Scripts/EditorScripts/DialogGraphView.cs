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
    using Dialog.Data.Save;
    public class DialogGraphView : GraphView
    {
        private string _pathDialogGraphSteel = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogStyle.uss";
        private string _pathNodeSteel = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogNodeSteel.uss";
        private string _pathBranchStyle = $"Assets/Editor/EditorDefaultResources/DialogSystem/DialogueBranchNodeStyle.uss";
        

        private MiniMap _miniMap;
        private DialogSearchWindow _searchWindows;
        private WindowEditor _editorWindow;
        private SerializableDictionary<string,DialogNodeErrorData> _ungroupeNodes;
        private SerializableDictionary<Group, SerializableDictionary<string,DialogNodeErrorData>> _groupedNodes;
        private SerializableDictionary<string,DialogGroupErrorData> _groups;
       
       
        private int nameErrorsAmount;
        public int NameErrorsAmount
        {
            get
            {
                return nameErrorsAmount;
            }

            set
            {
                nameErrorsAmount =  value;
                if(nameErrorsAmount == 0)
                {
                    _editorWindow.EnableSaving();
                }

                if(nameErrorsAmount == 1)
                {
                    _editorWindow.DisableSaving();
                }
            }
        }
       
       
        public DialogGraphView(WindowEditor editorWindow)
        {
            _editorWindow = editorWindow;
            _ungroupeNodes = new SerializableDictionary<string, DialogNodeErrorData>();
            _groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DialogNodeErrorData>>();
            _groups = new SerializableDictionary<string, DialogGroupErrorData>();


            AddGridBackground();
            AddSearchWindows();
            AddMiniMapWindows();
            AddManipulators();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
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
            this.AddManipulator(CreateNodeContextMenu("Multi Node", DialogueType.MultipleChoise));
            this.AddManipulator(CreateGroupContextMenu());
            this.AddManipulator(CreateConditionContextMenu("Choice", DialogueType.Condition));
        }

        private IManipulator CreateGroupContextMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("New Group",GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );
            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextMenu(string actionTitle, DialogueType typeNode)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("New Node",typeNode,GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );
            return contextualMenuManipulator;
        }

        private IManipulator CreateConditionContextMenu(string actionTitle, DialogueType typeNode)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode("Choice text", typeNode,GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );
            return contextualMenuManipulator;
        }

        internal Node CreateNode(string nodeName, DialogueType typeNode,Vector2 position, bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"DialogEditor.Elements.{typeNode}Node");

            DialogNode node = (DialogNode) Activator.CreateInstance(nodeType);
            node.Intialize(nodeName,this,position);

            if(shouldDraw)
            {
                node.Draw();
            }

            AddUngroupeNodes(node);


            return node;
        }


        public void AddUngroupeNodes(DialogNode node)
        {
            string nodeName = node.DialogueName.ToLower();
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
                ++NameErrorsAmount;

                ungroupNodesList[0].SetErrorStyle(errorColor);
            }
        }


        public void RemoveUngroupeNode(DialogNode node)
        {
            string nodeName = node.DialogueName.ToLower();
            List<DialogNode> ungroupNodesList = _ungroupeNodes[nodeName].Nodes;

            ungroupNodesList.Remove(node);
            node.ResetStyle();

            if(_ungroupeNodes[nodeName].Nodes.Count == 1)
            {
                --NameErrorsAmount;

                ungroupNodesList[0].ResetStyle();
                return;
            }
            if(_ungroupeNodes[nodeName].Nodes.Count == 0)
            {
                _ungroupeNodes.Remove(nodeName);
            }
        }
        

        private void RemoveGroupe(GroupElements group)
        {
            string oldGroupName = group.OldTitel.ToLower();

            List<GroupElements> groupsList =  _groups[oldGroupName].Groups;
            groupsList.Remove(group);
            group.ResetStyle();

            if(groupsList.Count == 1)
            {
                --NameErrorsAmount;
                groupsList[0].ResetStyle();
                return;
            }
            if(groupsList.Count == 0)
            {
                _groupedNodes[group].Remove(oldGroupName);
                if(_groupedNodes[group].Count == 0)
                {
                    _groupedNodes.Remove(group);
                }
            }
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
                    GroupElements myGroup = (GroupElements)group;
                    DialogNode node = (DialogNode) element;
                    RemoveUngroupeNode(node);
                    AddGroupedNode(node,myGroup);
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
                    GroupElements myGroup = (GroupElements)group;
                    DialogNode node = (DialogNode) element;
                    RemoveGroupedNode(node,myGroup);
                    AddUngroupeNodes(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitel ) =>
            {
                GroupElements dialogGroup = (GroupElements) group;
                
                dialogGroup.title = newTitel.RemoveWhitespaces().RemoveSpecialCharacters();
                if(string.IsNullOrEmpty(dialogGroup.title))
                {
                    if(!string.IsNullOrEmpty(dialogGroup.OldTitel))
                    {
                       ++NameErrorsAmount;
                    }
                } else 
                {
                    if(string.IsNullOrEmpty(dialogGroup.OldTitel))
                    {
                        --NameErrorsAmount;
                    }
                }
                RemoveGroupe(dialogGroup);

                dialogGroup.OldTitel = dialogGroup.title;
                AddGroup(dialogGroup);
            };
        }

        private void OnGraphViewChanged()
        {
           graphViewChanged = (changes) =>
           {
                if(changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        // тут обращаемся к следующему ноду через инпут edge

                        DialogNode nextNode = (DialogNode)edge.input.node;
                        DialogChoiseSaveData choiceData = (DialogChoiseSaveData)edge.output.userData;
                        choiceData.NodeID = nextNode.ID;
                        choiceData.ChoiceText = nextNode.DialogueName;
                        //Debug.Log("id next node = " + nextNode.ID + " ID choice = " + choiceData.NodeID + " text choice = " + choiceData.ChoiceText + " nextNode text choice = " + nextNode.DialogName);
                    }
                }

                if(changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if(element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge) element;
                        DialogChoiseSaveData  choiceData = (DialogChoiseSaveData) edge.output.userData;
                        choiceData.NodeID = "";
                        choiceData.ChoiceText = "<|>";
                        choiceData.ItemData = null;
                    }
                }
                return changes;
           };
        }
        public void RemoveGroupedNode(DialogNode node, GroupElements group)
        {
            string nodeName = node.DialogueName.ToLower();
            node.Group = null;
            List<DialogNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);
            node.ResetStyle();

            if(groupedNodesList.Count == 1)
            {
                --NameErrorsAmount;
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

        public void AddGroupedNode(DialogNode node, GroupElements group)
        {
            string nodeName = node.DialogueName.ToLower();
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
                ++NameErrorsAmount;
                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        internal Group CreateGroup(string name, Vector2 position)
        {
            GroupElements group = new GroupElements(name, position);
            AddGroup(group);
            AddElement(group);

            foreach (var selectedElement in selection)
            {
                if((selectedElement is DialogNode) == false)
                {
                    continue;
                }

                DialogNode node = (DialogNode) selectedElement;
                group.AddElement(node);
            }

            return group;
        }

        private void AddGroup(GroupElements group)
        {
            string groupName = group.title.ToLower();

            if(_groups.ContainsKey(groupName) == false)
            {
                DialogGroupErrorData groupErrorData = new DialogGroupErrorData();
                groupErrorData.Groups.Add(group);
                _groups.Add(groupName,groupErrorData);
            }

            List<GroupElements> groupsList = _groups[groupName].Groups;
            groupsList.Add(group);

            Color errorColor = _groups[groupName].ErrorData.Color;
            group.SetErrorStyle(errorColor);

            if(groupsList.Count == 2)
            {
                groupsList[0].SetErrorStyle(errorColor);
            }
        }


        private void AddStyles()
        {
            this.AddStyleSheets(
                _pathDialogGraphSteel,
                _pathNodeSteel,
                _pathBranchStyle
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

        public void ClearGraph()
        {
            graphElements.ForEach(graphElements => RemoveElement(graphElements));
            _groups.Clear();
            _groupedNodes.Clear();
            _ungroupeNodes.Clear();

            NameErrorsAmount = 0;
        }

        private void AddMiniMapWindows()
        {
            _miniMap = new MiniMap()
            {
                anchored = true, // перенос миникарты, теперь она кликабельна для переноса

            };

            _miniMap.SetPosition(new Rect(15,50,200,180));

            Add(_miniMap);
        }

        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29,29,30,255));
            StyleColor borderColor = new StyleColor(new Color32(51,51,51,255));

            _miniMap.style.backgroundColor = backgroundColor;

            _miniMap.style.borderTopColor = borderColor;
            _miniMap.style.borderRightColor = borderColor;
            _miniMap.style.borderBottomColor = borderColor;
            _miniMap.style.borderLeftColor = borderColor;

             _miniMap.visible = false;
        }

        public void ToogleMiniMap()
        {
            _miniMap.visible = !_miniMap.visible;
        }
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName,AskUser) =>
            {
                Type groupType = typeof(GroupElements);
                Type edgeType = typeof(Edge);
                List<GroupElements> groupsToDelete = new List<GroupElements>();
                List<Edge> edgesToDelete = new List<Edge>();
                List<DialogNode> nodeToDelete = new List<DialogNode>();


                foreach (GraphElement element in selection)
                {
                    if(element is DialogNode)
                    {
                        nodeToDelete.Add((DialogNode) element);
                        continue;
                    }
                    if(element.GetType() == edgeType)
                    {
                        Edge edge = (Edge)element;
                        edgesToDelete.Add(edge);

                        continue;
                    }

                    if(element.GetType() != groupType)
                    {
                        continue;
                    }

                    GroupElements currentGroup = (GroupElements)element;
              
                    groupsToDelete.Add(currentGroup);
                }

                //callback на удаление группы
                foreach (var group in groupsToDelete)
                {
                    List<DialogNode> groupNodes = new List<DialogNode>();
                    foreach (var groupElement in group.containedElements)
                    {
                        if((groupElement is DialogNode) == false)
                        {
                            continue;
                        }

                        DialogNode groupNode = (DialogNode)groupElement;
                        groupNodes.Add(groupNode);
                    }
                    group.RemoveElements(groupNodes);

                    RemoveGroupe(group);
                    
                    RemoveElement(group);
                }

                DeleteElements(edgesToDelete);
                // callback на удаление нода
                foreach (var node in nodeToDelete)
                {
                    if(node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }
                    RemoveUngroupeNode(node);
                    node.DisconnectAllPorts();
                    RemoveElement(node);
                }

            };
        }
    }
}
    