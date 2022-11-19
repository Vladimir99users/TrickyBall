using UnityEngine;

namespace Dialog.Data.Save
{
    [System.Serializable]
    public class DialogGroupSaveData
    {
        [field: SerializeField] public string ID {get;set;}
        [field: SerializeField] public string Name {get;set;}
        [field: SerializeField] public Vector2 position {get;set;}

    }
}
