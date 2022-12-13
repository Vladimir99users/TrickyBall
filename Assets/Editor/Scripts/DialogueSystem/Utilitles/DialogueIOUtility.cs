using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace DialogEditor.Utilities
{
    
    using Dialog.Data;
    using Dialog.Data.Save;
    using Dialog.ScriptableObjects;
    using DialogEditor.Enumerations;
    using Elements;

    public static class DialogueIOUtility
    { 
        private static DialogGraphView _graphView;
        private static string _graphFileName;
        private static string _containerFolderPath;
        private static List<GroupElements> _groups;
       // private static List<DialogNode> _nodes;
        private static List<BranchNode> _branchNode;
        private static List<MultipleChoiseNode> _dialogueMultipleNode;

        private static Dictionary<string, DialogGroupSO> _createdDialogueGroups;
        private static Dictionary<string, DialogSO> _createdDialogues;
        private static Dictionary<string, DialogBranchSO> _createdBranch;


        private static Dictionary<string,GroupElements> _loadedGroups;
        private static Dictionary<string,DialogNode> _loadedNodes;

        #region Start Methods
        public static void Initialize(DialogGraphView currentGraph,string graphName)
        {
            _graphView = currentGraph;
            _graphFileName = graphName;
            _containerFolderPath = $"Assets/DialogueSystem/Dialogues/{_graphFileName}";
        
            _groups = new List<GroupElements>();
           // _nodes = new List<DialogNode>();
            _branchNode = new List<BranchNode>();
            _dialogueMultipleNode = new List<MultipleChoiseNode>();

            _createdDialogueGroups = new Dictionary<string, DialogGroupSO>();
            _createdDialogues = new Dictionary<string, DialogSO>();
            _createdBranch = new Dictionary<string, DialogBranchSO>();

            _loadedGroups = new Dictionary<string, GroupElements>();
            _loadedNodes = new Dictionary<string, DialogNode>();
        }
        
        private static void CreateStaticFolders()
        {
            CreateFolder("Assets/Editor/Scripts/DialogueSystem", "Graphs");
            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/DialogueSystem", "Dialogues");
            CreateFolder("Assets/DialogueSystem/Dialogues", _graphFileName);
            CreateFolder(_containerFolderPath, "Global");
            CreateFolder(_containerFolderPath, "Groups");
            CreateFolder($"{_containerFolderPath}/Global", "Dialogues");
        }

        private static void GetElementsFromGraphView()
        {
            Type grouptype = typeof(GroupElements);
            _graphView.graphElements.ForEach(graphElement =>
            {
                if(graphElement is MultipleChoiseNode node)
                {
                    _dialogueMultipleNode.Add(node);
                    return;
                }
                if(graphElement is BranchNode branch)
                {
                    _branchNode.Add(branch);
                    return;
                }

                if(graphElement.GetType() == grouptype)
                {
                    GroupElements group = (GroupElements)graphElement;
                    _groups.Add(group);
                    return;
                }
            });
        }

        #endregion


        #region Load_Methods
        public static void Load()
        {
            DialogGraphSaveDataSO graphData = LoadAsset<DialogGraphSaveDataSO>("Assets/Editor/Scripts/DialogueSystem/Graphs", _graphFileName);
        
            if(graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Couldn't load is not found!",
                    "The file at the following path could not be found: \n\n"+
                    $"Assets/Editor/Scripts/DialogueSystem/Graphs/{_graphFileName} \n\n"+
                    "Make sure you choice the right file and it's placed at the folder mentioned above.",
                    "Thanks!"
                );
                return;
            }

            WindowEditor.UpdateFileName(graphData.FileName);
            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodeConnections();
        }

        private static void LoadGroups(List<DialogGroupSaveData> groups)
        {
           foreach (DialogGroupSaveData groupData in groups)
           {
                GroupElements group = (GroupElements)_graphView.CreateGroup(groupData.Name, groupData.position);
          
                group.ID = groupData.ID;


                _loadedGroups.Add(group.ID,group);
           }
        }

        private static void LoadNodes(List<BaseNodeSaveData> nodes)
        {
           foreach (BaseNodeSaveData nodeData in nodes)
           {

                DialogNode node = (DialogNode)_graphView.CreateNode(nodeData.Titel,nodeData.Type, nodeData.position,false);

                node.IDcurrentNode = nodeData.IDGuidBaseNode;
                node.Draw();

                _graphView.AddElement(node);

                _loadedNodes.Add(node.IDcurrentNode,node);
                if(string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                GroupElements group = _loadedGroups[nodeData.GroupID];
                node.Group = group;

                group.AddElement(node);

           }
        }
        private static void LoadNodeConnections()
        {
            foreach (KeyValuePair<string, DialogNode> loadNode in _loadedNodes)
            {
                foreach (Port choicePort in loadNode.Value.outputContainer.Children())
                {
                    DialogChoiseSaveData choiceData = (DialogChoiseSaveData)choicePort.userData;

                    if(string.IsNullOrEmpty(choiceData.NextNodeID))
                    {
                        continue;
                    }

                    DialogNode nextNode = _loadedNodes[choiceData.NextNodeID];

                    Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);


                    _graphView.AddElement(edge);

                    loadNode.Value.RefreshPorts();
                }
            }
        }
        #endregion


        #region Save_Methods
        public static void Save()
        {
            CreateStaticFolders();
            GetElementsFromGraphView();

            DialogGraphSaveDataSO graphData = CreateAsset<DialogGraphSaveDataSO>($"Assets/Editor/Scripts/DialogueSystem/Graphs", $"{_graphFileName}Graph");
        
            graphData.Initialize(_graphFileName);

            DialogContainerSO dialogContainerSO = CreateAsset<DialogContainerSO>(_containerFolderPath,_graphFileName);
            dialogContainerSO.Initialize(_graphFileName);

            SaveGroups(graphData, dialogContainerSO);
            SaveNodes(graphData, dialogContainerSO);

            SaveAssets(graphData);
            SaveAssets(dialogContainerSO);
        }
        #endregion

        #region Nodes

        private static void SaveNodes(DialogGraphSaveDataSO graphData, DialogContainerSO dialogContainerSO)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();

            foreach (var node in _dialogueMultipleNode)
            {
                SaveNodeToGraph((MultipleChoiseNode)node,graphData);
                SaveNodeToScriptableObject((MultipleChoiseNode)node,dialogContainerSO);
                if(node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
                    continue;
                }

                ungroupedNodeNames.Add(node.DialogueName);
                break;
            }

            foreach (var node in _branchNode)
            {
                SaveNodeBrachToGraph((BranchNode)node,graphData);
                SaveNodeBrachToScriptableObject((BranchNode)node,dialogContainerSO);
                if(node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);

                    continue;
                }

                ungroupedNodeNames.Add(node.DialogueName);
                break;

            }


            UpdateDialoguesChoicesConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);

            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(MultipleChoiseNode node, DialogGraphSaveDataSO graphData)
        {
            List<DialogBranchData> choices = CloneNodeChoices(node.Choices);

            DialogueNodeSaveData nodeData = new DialogueNodeSaveData()
            {
                IDGuidBaseNode = node.IDcurrentNode,
                Titel = node.DialogueName,
                Choices = choices,
                Text = node.Text,
                GroupID = node.Group?.ID,
                Type = node._typeDialog,
                position = node.GetPosition().position
            };

            graphData.Nodes.Add(nodeData);
        }
        private static void SaveNodeToScriptableObject(MultipleChoiseNode node, DialogContainerSO dialogContainerSO)
        {
            DialogSO dialogue;

            if(node.Group != null)
            {
                dialogue = CreateAsset<DialogSO>($"{_containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
                dialogContainerSO.DialogueGroups.AddItem(_createdDialogueGroups[node.Group.ID], dialogue);
            } else 
            {
                dialogue = CreateAsset<DialogSO>($"{_containerFolderPath}/Global/Dialogues", node.DialogueName);
                
                dialogContainerSO.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(
                node.DialogueName,
                node.Text,
                ConvertNodeChoicesToDialogChoices(node.Choices),
                node._typeDialog,
                node.IsStartingNode()
            );
            _createdDialogues.Add(node.IDcurrentNode, dialogue);
            SaveAssets(dialogue);
        }
        private static void SaveNodeBrachToGraph(BranchNode node, DialogGraphSaveDataSO graphData)
        {
            //TODO CLODE ALL Items
            List<DialogueItemDataSO> items = new List<DialogueItemDataSO>();

            BranchNodeSaveData branchData = new BranchNodeSaveData()
            {
                IDGuidBaseNode = node.IDcurrentNode,
                Titel = node.DialogueName,
                NextIDNode = node.TargetNodeID,
                ItemData = items,
                GroupID = node.Group?.ID,
                Type = node._typeDialog,
                position = node.GetPosition().position
            };

            graphData.Nodes.Add(branchData);
        }

        private static void SaveNodeBrachToScriptableObject(BranchNode node, DialogContainerSO dialogContainerSO)
        {
            DialogBranchSO branch;
             if(node.Group != null)
            {
                branch = CreateAsset<DialogBranchSO>($"{_containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);
                dialogContainerSO.DialogueBrancGroups.AddItem(_createdDialogueGroups[node.Group.ID], branch);
            } else 
            {
                branch = CreateAsset<DialogBranchSO>($"{_containerFolderPath}/Global/Dialogues", node.DialogueName);
                
                dialogContainerSO.UngroupedBranchDialogues.Add(branch);
            }

            branch.Initialize(
                node.DialogueName,
                node.TargetNodeID,
                node.Data,
                node._typeDialog,
                false
            );

            _createdBranch.Add(node.IDcurrentNode,branch);
            SaveAssets(branch);
        }

        private static List<DialogBranchData> CloneNodeChoices(List<DialogBranchData> nodeChoices)
        {
            List<DialogBranchData> choices = new List<DialogBranchData>();

            foreach (DialogBranchData choice in nodeChoices)
            {
                DialogBranchData choiceData  = new DialogBranchData()
                {
                    NextNodeID = choice.NextNodeID,
                };
                
                
                choices.Add(choiceData);
            }

            return choices;
        }

        private static List<DialogBranchData> ConvertNodeChoicesToDialogChoices(List<DialogBranchData> nodeChoices)
        {
            List<DialogBranchData> dialogChoices = new List<DialogBranchData>();

            foreach (DialogBranchData nodeChoice in nodeChoices)
            {
                DialogBranchData choiceData = new DialogBranchData();

                dialogChoices.Add(choiceData);
            }
            return dialogChoices;
        }
        // myMethod node item choice
        private static List<DialogueItemDataSO> CloneNodeItemChoices(List<DialogueItemDataSO> nodeItem)
        {
            List<DialogueItemDataSO> items = new List<DialogueItemDataSO>();
            
            if(nodeItem is null) return null;


            foreach (DialogueItemDataSO item in nodeItem)
            {
                DialogueItemDataSO choiceData = new DialogueItemDataSO()
                {
                    NameItem = item.NameItem,
                    CountItem = item.CountItem,
                    CountRequare = item.CountRequare,
                    Type = item.Type
                };
                items.Add(choiceData);
            }

            return items;
        }

        
        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, DialogGraphSaveDataSO graphData)
        {
            if(graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGrouped in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if(currentGroupedNodeNames.ContainsKey(oldGrouped.Key))
                    {
                        nodesToRemove = oldGrouped.Value.Except(currentGroupedNodeNames[oldGrouped.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{_containerFolderPath}/Groups/{oldGrouped.Key}/Dialogues", nodeToRemove);
                    }
                }

                graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
                
            }
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNode, DialogGraphSaveDataSO data)
        {
            if(data.OldUngroupedNodeNames != null && data.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = data.OldUngroupedNodeNames.Except(currentUngroupedNode).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{_containerFolderPath}/Global/Dialogues", nodeToRemove);
                }
            }

            data.OldUngroupedNodeNames = new List<string>(currentUngroupedNode);
        }

        #endregion

        #region Groups

        private static void SaveGroups(DialogGraphSaveDataSO data, DialogContainerSO container)
        {
            List<string> groupName = new List<string>();
            foreach (GroupElements group in _groups)
            {
                SaveGroupToGraph(group,data);
                SaveGroupToScriptableObject(group, container);

                groupName.Add(group.title);
            }

            UpdateOldGroups(groupName,data);
        }

        private static void SaveGroupToGraph(GroupElements group,DialogGraphSaveDataSO data)
        {
            DialogGroupSaveData groupData = new DialogGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                position = group.GetPosition().position
            };

            data.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(GroupElements group, DialogContainerSO container)
        {
            string groupName = group.title;

            CreateFolder($"{_containerFolderPath}/Groups", groupName);
            CreateFolder($"{_containerFolderPath}/Groups/{groupName}", "Dialogues");

            DialogGroupSO dialogueGroup = CreateAsset<DialogGroupSO>($"{_containerFolderPath}/Groups/{groupName}", groupName);
        
            dialogueGroup.Initialize(groupName);

            _createdDialogueGroups.Add(group.ID, dialogueGroup);

            container.DialogueGroups.Add(dialogueGroup,new List<DialogSO>());

            SaveAssets(dialogueGroup);
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, DialogGraphSaveDataSO data)
        {
            if(data.OldGroupNames != null && data.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = data.OldGroupNames.Except(currentGroupNames).ToList();
                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{_containerFolderPath}/groupsToRemove/{groupsToRemove}");
                }
            }

            data.OldGroupNames = new List<string>(currentGroupNames);
        }

        #endregion

        #region Utility Method

        private static void SaveAssets(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static T CreateAsset<T>(string path,string assetName) where T: ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";
            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, fullPath);
            }



            return asset;
        }

        private static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        private static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, folderName);
        }

        
        private static void UpdateDialoguesChoicesConnections()
        {
           
            foreach(var node in _dialogueMultipleNode)
            {
                if(node is MultipleChoiseNode)
                {
                    DialogSO dialogue = _createdDialogues[node.IDcurrentNode];

                    MultipleChoiseNode MultiNode = (MultipleChoiseNode) node;
                    for(int choiceIndex = 0; choiceIndex < MultiNode.Choices.Count;++choiceIndex)
                    {
                        DialogBranchData nodeChoice = MultiNode.Choices[choiceIndex];

                        if(string.IsNullOrEmpty(nodeChoice.NextNodeID))
                        {
                            continue;
                        }
                        // ERRORS не может найти ключ в дикшеонари
                        dialogue.Choices[choiceIndex].NextDialog =_createdBranch[nodeChoice.NextNodeID];
                    
                        SaveAssets(dialogue);
                    }
                }
            }

            foreach(var node in _branchNode)
            {

                DialogBranchSO dialogue = _createdBranch[node.IDcurrentNode];
                BranchNode branchNode = (BranchNode) node;
                BranchNodeSaveData nodeChoice = new BranchNodeSaveData()
                {
                    NextIDNode = branchNode.NextNode.NextIDNode
                };
                if(string.IsNullOrEmpty(nodeChoice.NextIDNode))
                {
                    continue;
                }
                dialogue.Next.NextDialog =  _createdDialogues[nodeChoice.NextIDNode];
                SaveAssets(dialogue);
            }
        }

        private static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta"); 
            FileUtil.DeleteFileOrDirectory($"{fullPath}/"); 
        }

        
        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        



        #endregion

    }
}


