using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DialogEditor
{
    internal class DialogGroup : Group
    {
      
        public void Intialize(Vector2 position)
        {
            autoUpdateGeometry = true;
            SetPosition(new Rect(position,Vector2.zero));
        }
    }
}