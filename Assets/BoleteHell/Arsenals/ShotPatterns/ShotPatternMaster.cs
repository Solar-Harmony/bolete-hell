using System.Collections.Generic;
using UnityEngine;

//Permet de jumeller plusieurs patterns ensemble et de les sauvegarder
namespace BoleteHell.Arsenals.ShotPatterns
{
    [CreateAssetMenu(fileName = "BulletPatternMaster", menuName = "BoleteHell/Arsenal/Shot Pattern Master")]
    public class ShotPatternMaster : ScriptableObject
    {
        [SerializeField] public List<ShotPatternData> patterns;
    }
}
