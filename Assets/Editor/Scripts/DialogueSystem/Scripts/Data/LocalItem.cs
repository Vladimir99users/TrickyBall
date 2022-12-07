using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="item",menuName ="Item/local")]
public class LocalItem : ScriptableObject
{
    [SerializeField] public string Name{get;set;}

}
