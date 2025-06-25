using System.Collections.Generic;
using System.Linq;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Character;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.RayData
{
    public class LaserCombo : IProjectileData
    {
        public Color CombinedColor { get; private set; }
        public float CombinedRefractiveIndex {get; private set; }
        //Va pouvoir être utilisé pour quand le laser hit un diffract shield
        private List<LaserData> _datas;
    
        public LaserCombo(List<LaserData> datas)
        {
            _datas = datas;
            if (datas.Count == 1)
            {
                CombinedColor = datas[0].Color;
                CombinedRefractiveIndex = datas[0].LightRefractiveIndex;
            }
            else
            {
                CombinedColor = CombineColors(datas.Select(l => l.Color).ToList());
                CombinedRefractiveIndex = CombineRefractiveIndices(datas.Select(l => l.LightRefractiveIndex).ToList());
            }
        }

        Color CombineColors(List<Color> colorList)
        {
            float r = 0f, g = 0f, b = 0f;
        
            foreach (Color color in colorList)
            {
                r += color.r;
                g += color.g;
                b += color.b;
            }

            return new Color(r / colorList.Count, g / colorList.Count, b / colorList.Count);
        }


        private float CombineRefractiveIndices(List<float> refractiveIndices)
        {
            float total = refractiveIndices.Sum();

            return total / refractiveIndices.Count;
        }

        public void CombinedEffect(Vector2 hitPosition,Health hitCharacterHealth)
        {
            foreach (LaserData data in _datas)
            {
                data.Logic.OnHit(hitPosition,hitCharacterHealth);
            }
        }
    }
}
