using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace DialogEditor.Data.Error
{
    public class DialogGroupErrorData
    {
        public DialogErrorData ErrorData {get;set;}
        public List<Group> Groups {get;set;}
        public DialogGroupErrorData()
        {
            ErrorData = new DialogErrorData();
            Groups = new List<Group>();
        }
    }
}

