using System.Collections.Generic;
using System.Linq;
using BoleteHell.Code.Arsenal.HitHandler;
using BoleteHell.Code.Arsenal.Rays;
using BoleteHell.Code.Gameplay.Character;
using BoleteHell.Code.Gameplay.Damage;
using UnityEngine;

namespace BoleteHell.Code.Arsenal.RayData
{
    public class LaserCombo : IProjectileData
    {
        public Color CombinedColor { get; private set; }
        public float CombinedRefractiveIndex {get; private set; }
        //Va pouvoir être utilisé pour quand le laser hit un diffract shield
        public List<LaserData> Data { get; private set; }

        //Détermine qui le laser peut toucher
        public AffectedSide HitSide;
        
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
            
            SetAffectedSide();
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

        public void CombinedEffect(Vector2 hitPosition, IDamageable hitCharacterHealth, LaserInstance laserInstance)
        {
            foreach (LaserData data in Data)
            {
                //Not gonna lie je comprend pas ce qui ce passe avec le IDamageable et pourquoi c'est castable en IFaction
                
                //Peut-être faire le check de si l'éffet devrait affecter la personne touché selon sa faction
                //Donc un laser qui a un effet de soin des alliés et un effet de dégat des ennemis
                //Le laser pourrait soigné les alliés sans les endommagé ou endommagé les ennemis sans les soigné
                //Ce qui devrait être modifier quand un laser touche un shield a quel moment tout les éffet du laser devrait devenir neutre
                data.Logic.OnHit(hitPosition, hitCharacterHealth, laserInstance, data);
                
            }
        }

        public float GetLaserSpeed()
        {
            return Data.Average(data => data.MovementSpeed);
        }

        //Le affected side va être déterminé selon si les éffet sur le laser affecte les alliés ou les ennemis
        private void SetAffectedSide()
        {
            HitSide = Data[0].affectedSide;

            if (!Data.Any(laserData =>
                    laserData.affectedSide == AffectedSide.All || laserData.affectedSide != HitSide)) return;
            
            HitSide = AffectedSide.All;
        }

        public void MakeLaserNeutral()
        {
            HitSide = AffectedSide.All;
        }
    }
}
