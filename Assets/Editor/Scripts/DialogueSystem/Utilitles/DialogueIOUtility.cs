using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;

namespace DialogEditor.Utilities
{
    
    using Dialog.Data;
    using Dialog.Data.Save;
    using Dialog.ScriptableObjects;
    using Elements;
    public static class DialogueIOUtility
    { 
        private static DialogGraphView _graphView;
        private static string _graphFileName;
        private static string _containerFolderPath;
        private static List<GroupElements> _groups;
        private static List<DialogNode> _nodes;

        private static Dictionary<string, DialogGroupSO> _createdDialogueGroups;
        private static Dictionary<string, DialogSO> _createdDialogues;

        public static void Initialize(DialogGraphView currentGraph,string graphName)
        {
            _graphView = currentGraph;
            _graphFileName = graphName;
            _containerFolderPath = $"Assets/DialogueSystem/Dialogues/{_graphFileName}";
        
            _groups = new List<GroupElements>();
            _nodes = new List<DialogNode>();

            _createdDialogueGroups = new Dictionary<string, DialogGroupSO>();
            _createdDialogues = new Dictionary<string, DialogSO>();
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
                if(graphElement is DialogNode node)
                {
                    _nodes.Add(node);
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
            foreach (var node in _nodes)
            {
                SaveNodeToGraph(node,graphData);
                SaveNodeToScriptableObject(node,dialogContainerSO);

                if(node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogName);

                    continue;
                }

                ungroupedNodeNames.Add(node.DialogName);
            }

            UpdateDialoguesChoicesConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);

            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(DialogNode node, DialogGraphSaveDataSO graphData)
        {  
            List<DialogChoiseSaveData> choices = new List<DialogChoiseSaveData>();

            foreach (DialogChoiseSaveData choice in node.Choices)
            {
                DialogChoiseSaveData choiceData = new DialogChoiseSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };
                choices.Add(choiceData);
            }

            DialogNodeSaveData nodeData = new DialogNodeSaveData()
            {
                ID = node.ID,
                Name = node.DialogName,
                Choices = choices,
                Text = node.Text,
                GroupID = node.Group?.ID,
                Type = node._typeDialog,
                position = node.GetPosition().position
            };

            graphData.Nodes.Add(nodeData);
        }

        private static void SaveNodeToScriptableObject(DialogNode node, DialogContainerSO dialogContainerSO)
        {
            DialogSO dialogue;

            if(node.Group != null)
            {
                dialogue = CreateAsset<DialogSO>($"{_containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogName);
                dialogContainerSO.DialogueGroups.AddItem(_createdDialogueGroups[node.Group.ID], dialogue);
            } else 
            {
                dialogue = CreateAsset<DialogSO>($"{_containerFolderPath}/Global/Dialogues", node.DialogName);
                
                dialogContainerSO.UngroupedDialogues.Add(dialogue);
            }

            dialogue.Initialize(
                node.DialogName,
                node.Text,
                ConvertNodeChoicesToDialogChoices(node.Choices),
                node._typeDialog,
                node.IsStartingNode()
            );
            _createdDialogues.Add(node.ID, dialogue);
            SaveAssets(dialogue);
        }

        private static List<DialogChoiceData> ConvertNodeChoicesToDialogChoices(List<DialogChoiseSaveData> nodeChoices)
        {
            List<DialogChoiceData> dialogChoices = new List<DialogChoiceData>();

            foreach (DialogChoiseSaveData nodeChoice in nodeChoices)
            {
                DialogChoiceData choiceData = new DialogChoiceData()
                {
                    Text = nodeChoice.Text
                };
                dialogChoices.Add(choiceData);
            }
            return dialogChoices;
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

            T asset  = AssetDatabase.LoadAssetAtPath<T>(fullPath);

            if(asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset,fullPath);
            }
            


            return asset;
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
            foreach(DialogNode node in _nodes)
            {
                DialogSO dialogue = _createdDialogues[node.ID];

                for(int choiceIndex = 0; choiceIndex < node.Choices.Count;++choiceIndex)
                {
                    DialogChoiseSaveData nodeChoice = node.Choices[choiceIndex];

                    if(string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextDialog = _createdDialogues[nodeChoice.NodeID];
                    
                    SaveAssets(dialogue);
                }
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


