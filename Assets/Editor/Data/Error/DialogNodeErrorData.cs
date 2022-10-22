using System.Collections.Generic;
using DialogEditor.Elements;

namespace DialogEditor.Data.Error
{
    public class DialogNodeErrorData
    {
        public DialogErrorData ErrorData { get; set; } 

        public List<DialogNode> Nodes { get; set; }
        public DialogNodeErrorData()
        {
            ErrorData = new DialogErrorData();
            Nodes = new List<DialogNode>();
        }
    }
}
