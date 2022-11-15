using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogEditor.Elements
{
    public class GroupElements : Group
    {
        private Color defaultBoarderColor;
        private float defaultBorderWidth;

        public GroupElements(string groupTitel,Vector2 position)
        {
            title = groupTitel;
            SetPosition(new Rect(position, Vector2.zero));
            defaultBoarderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }

        public void ResetSyle()
        {
            contentContainer.style.borderBottomColor = defaultBoarderColor;
            contentContainer.style.borderBottomWidth = defaultBorderWidth;
        }
    }
}

