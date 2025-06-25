using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

//Permet de jumeller plusieurs patterns ensemble et de les sauvegarder
[CreateAssetMenu(fileName = "BulletPatternMaster", menuName = "Scriptable Objects/BulletPatternMaster")]
public class ShotPatternMaster : ScriptableObject
{
    [SerializeField] public List<ShotPatternData> patterns;
}
