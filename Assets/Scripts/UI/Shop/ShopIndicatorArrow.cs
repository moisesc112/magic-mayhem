using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopIndicatorArrow : MonoBehaviour
{
    private Transform shopkeeper;
    public RectTransform arrow;
    public Camera playerCam;
    public float arrowMargin = 250;

    private void Start()
    {
        if (ShopKeeper.instance != null)
        {
            shopkeeper = ShopKeeper.instance.shopkeeperPosition;
        }
        else
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(shopkeeper == null || !shopkeeper.gameObject.activeInHierarchy)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        else 
        {
            arrow.gameObject.SetActive(true);
        }

        Vector3 shopkeeperScreenPoint = playerCam.WorldToScreenPoint(shopkeeper.position);

        bool isOffScreen = shopkeeperScreenPoint.x < 0 || shopkeeperScreenPoint.x > Screen.width || shopkeeperScreenPoint.y < 0 || shopkeeperScreenPoint.y > Screen.height;

        arrow.gameObject.SetActive(isOffScreen);

        if (isOffScreen)
        {
            Vector3 playerScreenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 shopDirection = (shopkeeperScreenPoint - playerScreenCenter).normalized;
            
            float angle = Mathf.Atan2(shopDirection.y, shopDirection.x) * Mathf.Rad2Deg;

            arrow.localEulerAngles = new Vector3(0, 0, angle);

            Vector3 clampedScreenPosition = shopkeeperScreenPoint;
            clampedScreenPosition.x = Mathf.Clamp(clampedScreenPosition.x, arrowMargin, Screen.width - arrowMargin);
            clampedScreenPosition.y = Mathf.Clamp(clampedScreenPosition.y, arrowMargin, Screen.height - arrowMargin);
            arrow.position = clampedScreenPosition;
        }


    }
}