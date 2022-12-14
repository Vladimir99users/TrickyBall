using UnityEditor;
using UnityEngine.UIElements;

namespace DialogEditor.Utilities
{

   
    public static class DialogStyleUtility
    {
        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (var styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load(styleSheetName);
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }

        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (var className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }
    }
}

