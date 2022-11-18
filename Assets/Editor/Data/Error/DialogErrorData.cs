using UnityEngine;

namespace DialogEditor.Data.Error
{
    public class DialogErrorData
    {
        public Color Color {get;set;}
        
        private Color32 ErrorColor = new Color32(240,27,19,200);
        public DialogErrorData()
        {
            SetErrorColor();
        }
        private void SetErrorColor()
        {
            Color = ErrorColor;
        }

        /* рандомная генерация цветов, не совсм корректно
         Color = new Color32(
                (byte)Random.Range(65,256),
                (byte)Random.Range(50,176),
                (byte)Random.Range(50,176),
                255
            );
        
        
        */
    }


}
