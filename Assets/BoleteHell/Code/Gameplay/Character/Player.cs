using UnityEngine;

namespace BoleteHell.Code.Gameplay.Character
{
    public class Player : Character
    {
        private void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.Label(new Rect(10, 10, 300, 80), "Health: " + health.CurrentHealth);
        }
    }
}