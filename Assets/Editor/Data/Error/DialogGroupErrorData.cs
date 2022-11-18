using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using DialogEditor.Elements;

namespace DialogEditor.Data.Error
{
    public class DialogGroupErrorData
    {
        public DialogErrorData ErrorData {get;set;}
        public List<GroupElements> Groups {get;set;}
        public DialogGroupErrorData()
        {
            ErrorData = new DialogErrorData();
            Groups = new List<GroupElements>();
        }
    }
}

