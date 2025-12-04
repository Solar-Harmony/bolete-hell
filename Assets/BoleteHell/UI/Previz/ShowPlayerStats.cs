using BoleteHell.Gameplay.Characters;
using UnityEngine;

namespace BoleteHell.Code.UI.Previz
{
    [RequireComponent(typeof(HealthComponent), typeof(EnergyComponent))]
    public class ShowPlayerStats : MonoBehaviour
    {
        private HealthComponent _health;
        private EnergyComponent _energy;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            _energy = GetComponent<EnergyComponent>();
        }

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 32;
            GUI.skin.label.alignment = TextAnchor.UpperCenter;
            float labelWidth = 300;
            float centerX = (Screen.width - labelWidth) / 2;
            GUI.Label(new Rect(centerX, 10, labelWidth, 80), "Health: " + _health.CurrentHealth);
            GUI.Label(new Rect(centerX, 50, labelWidth, 80),
                $"Energy: {_energy?.CurrentEnergy:F0} / {_energy?.MaxEnergy}");
        }
    }
}
