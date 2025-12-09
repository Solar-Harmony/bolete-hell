using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.Characters.Registry;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace BoleteHell.Code.UI
{
    public class HUD : MonoBehaviour
    {
        [Inject]
        private IEntityRegistry _entityRegistry;

        private ProgressBar _healthBar;
        private ProgressBar _energyBar;

        private HealthComponent _healthComponent;
        private EnergyComponent _energyComponent;
        private Arsenal.Arsenal _arsenal;
        
        private GameObject _player;
        
        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _healthBar = root.Q<ProgressBar>("health");
            _energyBar = root.Q<ProgressBar>("energy");
        }

        private void OnEnable()
        {
            if (_player) Init();
        }

        private void Start()
        {
            _player = _entityRegistry.GetPlayer();
            Init();
        }

        private void Init()
        {
            _healthComponent = _player.GetComponent<HealthComponent>();
            _energyComponent = _player.GetComponent<EnergyComponent>();
            _arsenal = _player.GetComponent<Arsenal.Arsenal>();
            
            HealthComponent.OnDamaged += OnHealthChanged;
            HealthComponent.OnHealed += OnHealthChanged;
            _energyComponent.OnEnergyChanged += OnEnergyChanged;
            
            OnHealthChanged(_healthComponent.gameObject, 0);
            OnEnergyChanged(_energyComponent.Percent);
        }

        private void OnDisable()
        {
            HealthComponent.OnDamaged -= OnHealthChanged;
            HealthComponent.OnHealed -= OnHealthChanged;
            _energyComponent.OnEnergyChanged -= OnEnergyChanged;
        }

        private void OnHealthChanged(GameObject _, int __)
        {
            _healthBar.value = _healthComponent.Percent * 100;
            _healthBar.title = $"{_healthComponent.CurrentHealth} / {_healthComponent.MaxHealth}";
        }

        private void OnEnergyChanged(float percent)
        {
            _energyBar.value = percent * 100;
            _energyBar.title = $"{_energyComponent.CurrentEnergy} / {_energyComponent.MaxEnergy}";
        }
    }
}