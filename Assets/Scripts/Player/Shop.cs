using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	[Header("Prefabs")]
	[SerializeField] List<AbilityInfo> _abilityRegistry;
	[SerializeField] SpellOption _spellOptionPrefab;
	[SerializeField] ConfirmSpellButton _confirmSpellButtonPrefab;

	[Header("Screens")]
	[SerializeField] GameObject _shopUI;
	[SerializeField] GameObject _spellListUI;
	[SerializeField] GameObject _upgradesUI;
	[SerializeField] SpellDescription _spellDescription;
	[SerializeField] GameObject _abilitySlotConfirmation;

	[Header("UIElements")]
	[SerializeField] VerticalLayoutGroup _spellList;
	[SerializeField] VerticalLayoutGroup _upgradeList;
	[SerializeField] HorizontalLayoutGroup _confirmSpellList;
	[SerializeField] TextMeshProUGUI _shufflePlayerGoldText;
	[SerializeField] Button _shuffleButton;

	[Header("Settings")]
	[SerializeField] InputSystemUIInputModule _inputModule;
	[SerializeField] MultiplayerEventSystem multiplayerEventSystem;
	[SerializeField] int _shuffleIncreaseAmount = 1;
	[SerializeField] int _shuffleCostCap = 30;

	public int numOfSpells = 3;
	public InputSystemUIInputModule inputModule => _inputModule;

	private void Awake()
	{
		_player = GetComponentInParent<Player>();
		_shopUI.SetActive(false);
		_abilitySlotConfirmation.SetActive(false);

		if (WaveManager.instance != null)
			WaveManager.instance.waveFinished += WaveManager_OnWaveFinished;
	}

	private void Start()
	{
		SetupSpellsUI();
		ShuffleShopAbilityOptions(didPlayerUseShuffle: false);
	}

	private void OnDestroy()
	{
		if (WaveManager.instance != null)
			WaveManager.instance.waveFinished -= WaveManager_OnWaveFinished;
	}

	public void PurchaseAbility(SpellOption spell)
	{
		if (spell.abilityInfo.cost > _player.PlayerStats.gold)
		{
			Debug.LogError("BROKE BEHAVIOR: Player attempting to purchase something they can't afford.");
			return;
		}

		var abilitySlotComponent = _player.GetComponentInChildren<AbilitySlotsComponent>();
		if (abilitySlotComponent != null && !abilitySlotComponent.GetAreAllAbilitySlotsFull())
		{
			_player.PlayerStats.gold -= spell.abilityInfo.cost;
			abilitySlotComponent.UpdateAbilitySlot(spell.abilityInfo, abilitySlotComponent.GetNextUnusedAbilitySlotNumber());
			RefreshAllSpellPurchasability();
		}
		else
		{
			_selectedSpell = spell;
			for (int i = 0; i < abilitySlotComponent.numberOfAbilities; i++)
			{
				var ability = abilitySlotComponent.GetAbility(i + 1);
				_confirmSpellButtons[i].UpdateAbilityInfo(ability, i + 1);
			}
			if (PlayerUsingMK() == false)
			{
				multiplayerEventSystem.SetSelectedGameObject(null);
				multiplayerEventSystem.SetSelectedGameObject(_confirmSpellButtons[0].gameObject);
			}
			_abilitySlotConfirmation.SetActive(true);
		}
	}

	public void PurchaseAbilityForSlot(int index)
	{
		_player.PlayerStats.gold -= _selectedSpell.abilityInfo.cost;
		_player.abilitySlotsComponent.UpdateAbilitySlot(_selectedSpell.abilityInfo, index);
		_selectedSpell = null;
		_abilitySlotConfirmation.SetActive(false);
		multiplayerEventSystem.SetSelectedGameObject(null);
		multiplayerEventSystem.SetSelectedGameObject(_spellOptions[0].gameObject);
	}

	public void PurchaseUpgradeForSlot(int index, AbilityInfo spell)
	{
		_player.PlayerStats.gold -= spell.cost;
		_player.abilitySlotsComponent.UpdateAbilitySlot(spell, index);
		RefreshAllSpellPurchasability();
		RefreshUpgradeOptions();
	}

	public void ToggleShopUI(bool isEnabled)
	{
		_abilitySlotConfirmation.SetActive(false);
		_shopUI.SetActive(isEnabled);
		if (isEnabled)
		{
			OpenShop();
		}
		else
		{
			CloseShop();
		}
	}

	public void SetSelectedSpell(SpellOption spell)
	{
		UpdateDescription(spell);
	}

	public void ShuffleSpells()
	{
		ShuffleShopAbilityOptions(didPlayerUseShuffle: true);
	}

	public void GoToUpgradeScreen()
	{
		_spellListUI.SetActive(false);
		_upgradesUI.SetActive(true);
	}

	public void GoToSpellListScreen()
	{
		_spellListUI.SetActive(true);
		_upgradesUI.SetActive(false);
	}

	public void ToggleShopPage()
	{
		_showingSpellList = !_showingSpellList;
		if (_showingSpellList)
			GoToSpellListScreen();
		else
			GoToUpgradeScreen();
	}

	private void ResetShuffleCost() => _currentShuffleCost = SHUFFLE_COST_START;
	private void UpdateShuffleText() => _shufflePlayerGoldText.text = _currentShuffleCost.ToString();

	private void WaveManager_OnWaveFinished(object sender, WaveEndedEventArgs e)
	{
		ShuffleShopAbilityOptions(didPlayerUseShuffle: false);
		ResetShuffleCost();
	}

	private void SetupSpellsUI()
	{
		_spellOptions = new List<SpellOption>();
		for (int i = 0; i < numOfSpells; i++)
		{
			var spellOption = Instantiate(_spellOptionPrefab, _spellList.transform);
			spellOption.SetShop(this);
			_spellOptions.Add(spellOption);
		}

		_confirmSpellButtons = new List<ConfirmSpellButton>();
		_upgradeOptions = new List<SpellOption>();
		for (int i = 0; i < _player.abilitySlotsComponent.numberOfAbilities; i++)
		{
			// Setup confirm buttons
			var confirmSpell = Instantiate(_confirmSpellButtonPrefab, _confirmSpellList.transform);
			confirmSpell.SetSelectAction((index) => PurchaseAbilityForSlot(index));
			_confirmSpellButtons.Add(confirmSpell);

			// Setup upgrades
			var upgrade = Instantiate(_spellOptionPrefab, _upgradeList.transform);
			upgrade.ConfigureAsUpgrade(i + 1);
			upgrade.SetShop(this);
			_upgradeOptions.Add(upgrade);
		}

		_shuffleDisabledColor = _shuffleButton.colors.disabledColor;

		if (_player.owningController.usingMK == false)
			ConfigureUINav();
	}

	private void ConfigureUINav()
	{
		for (int i = 0; i < _spellOptions.Count; i++)
		{
			ConfigureButtonNav(_spellOptions[i].purchaseButton, i);
		}

		for (int i = 0; i < _confirmSpellButtons.Count; i++)
		{
			var confirmSpell = _confirmSpellButtons[i];
			if (i != 0)
			{
				var previousSpell = _confirmSpellButtons[i - 1].confirmButton;
				var previousNav = previousSpell.navigation;
				previousNav.mode = Navigation.Mode.Explicit;
				previousNav.selectOnRight = confirmSpell.confirmButton;
				previousSpell.navigation = previousNav;

				var currentNav = confirmSpell.confirmButton.navigation;
				currentNav.mode = Navigation.Mode.Explicit;
				currentNav.selectOnLeft = previousSpell;
				confirmSpell.confirmButton.navigation = currentNav;
			}
		}

		var shuffleNav = _shuffleButton.navigation;
		shuffleNav.mode = Navigation.Mode.Explicit;
		shuffleNav.selectOnUp = _spellOptions[numOfSpells - 1].purchaseButton;
		_shuffleButton.navigation = shuffleNav;

		void ConfigureButtonNav(Selectable button, int index)
		{
			var nav = button.navigation;
			nav.mode = Navigation.Mode.Explicit;
			if (index >= 1)
			{
				// Set current button's up to previos button
				var previousButton = _spellOptions[index - 1].purchaseButton;
				nav.selectOnUp = previousButton;

				// Set previous button's down to current button
				var previousNav = previousButton.navigation;
				previousNav.selectOnDown = button;
				previousButton.navigation = previousNav;
			}

			if (index == _spellOptions.Count - 1)
				nav.selectOnDown = _shuffleButton;

			button.navigation = nav;
		}
	}

	private void UpdateDescription(SpellOption spell)
	{
		if (spell != null)
			_spellDescription.SetSelectedSpell(spell.abilityInfo);
	}

	private void RefreshAllSpellPurchasability()
	{
		foreach (var option in _spellOptions)
		{
			if (option.abilityInfo.cost <= _player.PlayerStats.gold 
				&& !_player.abilitySlotsComponent.OwnsSpell(option.abilityInfo))
				option.EnablePurchase();
			else
				option.DisablePurchase();
		}
		UpdateShuffleButtonStyle();
	}

	private void RefreshUpgradeOptions()
	{
		for (int i = 0; i < _player.abilitySlotsComponent.numberOfAbilities; i++)
		{
			var matchingUpgradeSlot = _upgradeOptions[i];
			var matchingOwnedSpell = _player.abilitySlotsComponent.GetAbility(i + 1);
			if (matchingOwnedSpell != null && matchingOwnedSpell.nextLevel != null)
			{
				matchingUpgradeSlot.SetSpellInfo(matchingOwnedSpell.nextLevel);
				if (matchingUpgradeSlot.abilityInfo.cost <= _player.PlayerStats.gold)
					matchingUpgradeSlot.EnablePurchase();
				else
					matchingUpgradeSlot.DisablePurchase();
			}
			else
			{
				matchingUpgradeSlot.gameObject.SetActive(false);
			}
		}
	}

	private void ShuffleShopAbilityOptions(bool didPlayerUseShuffle = true)
	{
		if (didPlayerUseShuffle)
		{
			if (_player.PlayerStats.gold < _currentShuffleCost)
			{
				return;
			}
			_player.PlayerStats.gold -= _currentShuffleCost;
			_currentShuffleCost += _shuffleIncreaseAmount;
			_currentShuffleCost = Mathf.Clamp(_currentShuffleCost, 0, _shuffleCostCap);

			UpdateShuffleText();
		}
		else
		{
			ResetShuffleCost();
		}

		var uniqueCheck = new HashSet<int>();
		var spellNum = 0;
		while (uniqueCheck.Count < _spellOptions.Count)
		{
			var randomIndex = Random.Range(0, _abilityRegistry.Count - 1);
			// Adding to a hashset will only return true if the entry isn't already in the list
			if (!_player.abilitySlotsComponent.OwnsSpell(_abilityRegistry[randomIndex]) && uniqueCheck.Add(randomIndex))
			{
				_spellOptions[spellNum++].SetSpellInfo(_abilityRegistry[randomIndex]);
			}
		}

		RefreshAllSpellPurchasability();
	}

	private void UpdateShuffleButtonStyle()
	{
		// Darken button to look disabled but don't actually disable to allow nav.
		var canShuffle = _currentShuffleCost <= _player.PlayerStats.gold;
		_shuffleButton.GetComponent<Image>().color = canShuffle ? Color.white : _shuffleDisabledColor;
	}

	private void OpenShop()
	{
		RefreshAllSpellPurchasability();
		RefreshUpgradeOptions();
		if (PlayerUsingMK() == false)
			multiplayerEventSystem.SetSelectedGameObject(_spellOptions[0].purchaseButton.gameObject);
		UpdateShuffleText();
		UpdateDescription(_spellOptions[0]);
		GoToSpellListScreen();
	}

	private void CloseShop()
	{
		multiplayerEventSystem.SetSelectedGameObject(null);
	}

	private bool PlayerUsingMK() => _player.owningController.usingMK;

	const int SHUFFLE_COST_START = 1;
	
	List<SpellOption> _spellOptions;
	List<SpellOption> _upgradeOptions;
	List<ConfirmSpellButton> _confirmSpellButtons;
	Player _player;
	SpellOption _selectedSpell;

	Color _shuffleDisabledColor;

	int _currentShuffleCost = SHUFFLE_COST_START;
	bool _showingSpellList = true;
}
