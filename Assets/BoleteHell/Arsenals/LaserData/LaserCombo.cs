using System.Collections.Generic;
using System.Linq;
using BoleteHell.Arsenals.HitHandler;
using BoleteHell.Arsenals.Rays;
using BoleteHell.Code.Audio.BoleteHell.Models;
using UnityEngine;

namespace BoleteHell.Arsenals.LaserData
{
    public class LaserCombo : IProjectileData
    {
        public Color CombinedColor { get; private set; }
        public float CombinedRefractiveIndex {get; private set; }
        //Va pouvoir être utilisé pour quand le laser hit un diffract shield
        public List<LaserData> Data { get; private set; }
        
        public LaserCombo(List<LaserData> data)
        {
            Data = data;
            if (data.Count == 1)
            {
                CombinedColor = data[0].Color;
                CombinedRefractiveIndex = data[0].LightRefractiveIndex;
            }
            else
            {
                CombinedColor = CombineColors(data.Select(l => l.Color).ToList());
                CombinedRefractiveIndex = data.Select(l => l.LightRefractiveIndex).Average();
            }
        }

        private static Color CombineColors(List<Color> colorList)
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

        public void CombinedEffect(Vector2 hitPosition, IDamageable payload, LaserInstance laserInstance)
        {
            foreach (LaserData data in Data)
            {
                data.Logic.OnHit(hitPosition, payload, laserInstance, data);
            }
        }

        public float GetLaserSpeed()
        {
            return Data.Average(data => data.MovementSpeed);
        }
    }
}
