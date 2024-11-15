using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellOption : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
	[SerializeField] Shop _shop;
	[SerializeField] TextMeshProUGUI _spellNameText;
	[SerializeField] TextMeshProUGUI _spellCostTest;
	[SerializeField] Image _spellIcon;
	[SerializeField] Button _purchaseButton;
	[SerializeField] Color _purchaseDisabledColor = Color.white;
	[SerializeField] Color _purchaseEnabledColor = Color.white;

	public AbilityInfo abilityInfo => _abilityInfo;
	public Button purchaseButton => _purchaseButton;

	private void Awake()
	{
		_purchaseButton = GetComponent<Button>();
		_purchaseBackground = GetComponent<Image>();
	}

	public void SetShop(Shop shop) => _shop = shop;

	public void SetSpellInfo(AbilityInfo spellInfo)
	{
		_abilityInfo = spellInfo;
		_spellNameText.text = _abilityInfo.abilityName.ToUpper();
		_spellCostTest.text = _abilityInfo.cost.ToString();
		_spellIcon.sprite = _abilityInfo.icon;
	}

	public void BuySpell()
	{
		if (_canBuy)
		{
			_shop.PurchaseAbility(this);
			DisablePurchase();
		}
	}

	public void EnablePurchase()
	{
		_canBuy = true;
		if (_purchaseBackground != null)
			_purchaseBackground.color = _purchaseEnabledColor;
	}

	public void DisablePurchase()
	{
		_canBuy = false;
		if (_purchaseBackground != null)
			_purchaseBackground.color = _purchaseDisabledColor;
	}

	public void OnSelect(BaseEventData eventData)
	{
		_shop.SetSelectedSpell(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_shop.SetSelectedSpell(this);
	}

	Image _purchaseBackground;
	AbilityInfo _abilityInfo;
	bool _canBuy;
}
