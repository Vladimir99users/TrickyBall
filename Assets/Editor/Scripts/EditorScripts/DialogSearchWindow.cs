using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogEditor.Windows
{
    using Enumerations;
    using Elements;
    public class DialogSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogGraphView _grahpView;
        private Texture2D _identationIcon;
        public void Initialize(DialogGraphView dgGrahpView)
        {
            _grahpView = dgGrahpView;
            _identationIcon = new Texture2D(2,2);
            _identationIcon.SetPixel(0,0,Color.clear);
            _identationIcon.Apply();
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"),1),
                new SearchTreeEntry(new GUIContent("Multiple Choice",_identationIcon))
                {
                    level = 2,
                    userData = DialogueType.MultipleChoise
                },
                new SearchTreeGroupEntry(new GUIContent("Group Choice"),1),
                new SearchTreeEntry(new GUIContent("Single Group",_identationIcon))
                {
                    level = 2,
                    userData = new Group()
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition  = _grahpView.GetLocalMousePosition(context.screenMousePosition, true);
            switch (SearchTreeEntry.userData)
            {

                case DialogueType.MultipleChoise:
                {
                    MultipleChoiseNode  multipleNode = (MultipleChoiseNode) _grahpView.CreateNode("New Multi Node",DialogueType.MultipleChoise, localMousePosition);
                    _grahpView.AddElement(multipleNode);
                    return true;
                }
                case Group _:
                {
                    _grahpView.CreateGroup("New group",localMousePosition);
                    return true;
                }
                default:
                    return false;
            }
        }
    }
}

