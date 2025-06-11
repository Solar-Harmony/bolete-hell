using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

//Permet de jumeller plusieurs patterns ensemble et de les sauvegarder
[CreateAssetMenu(fileName = "BulletPatternMaster", menuName = "Scriptable Objects/BulletPatternMaster")]
public class BulletPatternMaster : ScriptableObject
{
    //[SerializeField] public bool simultaneousShots;
    
    [SerializeField] public List<BulletPatternData> patterns;
}
