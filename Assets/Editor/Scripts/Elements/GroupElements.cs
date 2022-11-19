using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogEditor.Elements
{
    public class GroupElements : Group
    {
        public string ID {get;set;}
        public string OldTitel {get;set;}
        private Color _defaultBoarderColor;
        private float _defaultBorderWidth;

        public GroupElements(string groupTitel,Vector2 position)
        {
            ID = System.Guid.NewGuid().ToString();
            title = groupTitel;
            OldTitel = groupTitel;
            SetPosition(new Rect(position, Vector2.zero));
            _defaultBoarderColor = contentContainer.style.borderBottomColor.value;
            _defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }

        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = _defaultBoarderColor;
            contentContainer.style.borderBottomWidth = _defaultBorderWidth;
        }
    }
}

