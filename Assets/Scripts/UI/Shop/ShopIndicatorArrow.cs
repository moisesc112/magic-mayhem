using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopIndicatorArrow : MonoBehaviour
{   // Arrow that pops up when the shop is off the screen
    // Used https://www.youtube.com/watch?v=dHzeHh-3bp4 as a reference

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
    }

    // Update is called once per frame
    void Update()
    {
        // Only turn on arrow if the shopkeeper is active and the instance is found
        if(shopkeeper == null || !shopkeeper.gameObject.activeInHierarchy)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        else 
        {
            arrow.gameObject.SetActive(true);
        }

        // Get the shopkeeper point based on the position
        Vector3 shopkeeperScreenPoint = playerCam.WorldToScreenPoint(shopkeeper.position);
        
        // To check if the shopkeeper is offscreen  
        bool isOffScreen = shopkeeperScreenPoint.x < 0 || shopkeeperScreenPoint.x > Screen.width || shopkeeperScreenPoint.y < 0 || shopkeeperScreenPoint.y > Screen.height;

        // Display if offscreen
        arrow.gameObject.SetActive(isOffScreen);

        if (isOffScreen)
        {   // Update position of arrow and clamp it so it has a certain margin from the edge of the screen 
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