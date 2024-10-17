using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuUIInputHandler : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputAction navigateAction;
    private InputAction submitAction;

    private GameObject currentSelected;
    private EventSystem eventSystem;

    void Awake()
    {
        // Get the InputActionAsset from the PlayerInput component
        var playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;

        // Find the "Navigate" and "Submit" actions in the "UI" action map
        var uiActionMap = inputActions.FindActionMap("UI");
        navigateAction = uiActionMap.FindAction("Navigate");
        submitAction = uiActionMap.FindAction("Submit");

        // Subscribe to action events
        navigateAction.performed += OnNavigate;
        submitAction.performed += OnSubmit;

        eventSystem = EventSystem.current;
    }

    void OnDestroy()
    {
        // Unsubscribe from action events
        navigateAction.performed -= OnNavigate;
        submitAction.performed -= OnSubmit;
    }

    void Start()
    {
        // Set the first selected UI element
        if (eventSystem.firstSelectedGameObject != null)
        {
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }
    }

    void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigation = context.ReadValue<Vector2>();

        if (navigation.magnitude > 0)
        {
            MoveSelection(navigation);
        }
    }

    void MoveSelection(Vector2 navigation)
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            // If nothing is selected, select the first selectable
            Selectable firstSelectable = FindObjectOfType<Selectable>();
            if (firstSelectable != null)
            {
                eventSystem.SetSelectedGameObject(firstSelectable.gameObject);
            }
            return;
        }

        Selectable current = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
        Selectable next = null;

        if (navigation.y > 0)
        {
            next = current.FindSelectableOnUp();
        }
        else if (navigation.y < 0)
        {
            next = current.FindSelectableOnDown();
        }
        else if (navigation.x > 0)
        {
            next = current.FindSelectableOnRight();
        }
        else if (navigation.x < 0)
        {
            next = current.FindSelectableOnLeft();
        }

        if (next != null)
        {
            eventSystem.SetSelectedGameObject(next.gameObject);
        }
    }

    void OnSubmit(InputAction.CallbackContext context)
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            // Invoke the button's onClick event
            Button button = eventSystem.currentSelectedGameObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }
}
