using UnityEngine;

namespace Dialog.Data.Save
{
    [System.Serializable]
    public class DialogConditionSaveData
    {
        [field: SerializeField] public string Name {get;set;}
        [field: SerializeField] public string Count {get;set;}

        public DialogConditionSaveData():this("Name", "0")
        {
            
        }
        public DialogConditionSaveData(string name, string count)
        {
            
            Name = name;
            Count = count;
        }
    }

}