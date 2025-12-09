using BoleteHell.Code.Arsenal.Cannons;
using BoleteHell.Gameplay.Characters;
using BoleteHell.Gameplay.Characters.Registry;
using BoleteHell.Gameplay.InputControllers;
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
        private Label _cannonLabel;
        private Label _shieldLabel;
        private VisualElement _shieldContainer;

        private HealthComponent _healthComponent;
        private EnergyComponent _energyComponent;
        private Arsenal.Arsenal _arsenal;
        private ShieldInput _shieldInput;
        
        private GameObject _player;
        
        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            _healthBar = root.Q<ProgressBar>("health");
            _energyBar = root.Q<ProgressBar>("energy");
            _cannonLabel = root.Q<Label>("cannon");
            _shieldLabel = root.Q<Label>("shield");
            _shieldContainer = root.Q<VisualElement>("shieldBadge");
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
            _shieldInput = _player.GetComponent<ShieldInput>();
            _arsenal = _player.GetComponent<Arsenal.Arsenal>();
            
            HealthComponent.OnDamaged += OnHealthChanged;
            HealthComponent.OnHealed += OnHealthChanged;
            _energyComponent.OnEnergyChanged += OnEnergyChanged;
            _arsenal.OnWeaponChanged += OnWeaponChanged;
            
            OnHealthChanged(_healthComponent.gameObject, 0);
        }

        private void Update()
        {
            // TODO: Add event for shield cycling
            var shield = _shieldInput.GetSelectedShield();
            _shieldLabel.text = shield.Name;
            
            Color c = shield.color;
            c.a = 0.5f;
                
            _shieldContainer.style.backgroundColor = new StyleColor(c);
        }

        private void OnDisable()
        {
            HealthComponent.OnDamaged -= OnHealthChanged;
            HealthComponent.OnHealed -= OnHealthChanged;
            _energyComponent.OnEnergyChanged -= OnEnergyChanged;
            _arsenal.OnWeaponChanged -= OnWeaponChanged;
        }

        private void OnHealthChanged(GameObject _, int __)
        {
            _healthBar.value = _healthComponent.Percent * 100;
            _healthBar.title = $"{_healthComponent.CurrentHealth} / {_healthComponent.MaxHealth}";
        }

        private void OnEnergyChanged(float percent)
        {
            _energyBar.value = percent * 100;
            _energyBar.title = $"{Mathf.CeilToInt(_energyComponent.CurrentEnergy)} / {_energyComponent.MaxEnergy}";
        }
        
        private void OnWeaponChanged(Cannon cannon)
        {
            _cannonLabel.text = cannon.Name;
        }
    }
}