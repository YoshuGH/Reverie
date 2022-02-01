using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScrollRectAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scrollSpeed = 10f;
    private bool finded = false, mouseOver = false;

    private List<Selectable> selectables = new List<Selectable>();
    private ScrollRect scrollRect;

    private Vector2 nextScrollPosition = Vector2.up;
    private TMP_Dropdown dropdown;

    [SerializeField] private InputActionReference uiMovement;

    void OnEnable()
    {
        if (scrollRect)
        {
            scrollRect.content.GetComponentsInChildren(selectables);
        }

        uiMovement.action.Enable();
    }

    private void OnDestroy()
    {
        uiMovement.action.Disable();
    }

    void Awake()
    {
        dropdown = transform.GetComponent<TMP_Dropdown>();
    }
    void Start()
    {
        if (scrollRect)
        {
            scrollRect.content.GetComponentsInChildren(selectables);
        }
    }
    void Update()
    {
        if(dropdown.IsExpanded && !finded)
        {
            scrollRect = transform.Find("Dropdown List").GetComponent<ScrollRect>();
            if (scrollRect)
            {
                scrollRect.content.GetComponentsInChildren(selectables);
                ScrollToSelected(true);
                finded = true;
            }
        }
        
        if(!dropdown.IsExpanded)
        {
            finded = false;
        }
        // Scroll via input.
        if(finded)
        {
            InputScroll();
            if(!mouseOver)
            {
                scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, nextScrollPosition, scrollSpeed * Time.unscaledDeltaTime);
            }
            else
            {
                nextScrollPosition = scrollRect.normalizedPosition;
            }
            
        }
    }
    void InputScroll()
    {

        if (selectables.Count > 0)
        {
            Vector2 inputVec = uiMovement.action.ReadValue<Vector2>();

            if ( inputVec.x != 0 || inputVec.y != 0)
            {
                ScrollToSelected(false);
            }
        }
    }
    void ScrollToSelected(bool quickScroll)
    {
        int selectedIndex = -1;
        Selectable selectedElement = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;

        if (selectedElement)
        {
            selectedIndex = selectables.IndexOf(selectedElement);
        }
        if (selectedIndex > -1)
        {
            if (quickScroll)
            {
                scrollRect.normalizedPosition = new Vector2(0, 1 - (selectedIndex / ((float)selectables.Count - 1)));
                nextScrollPosition = scrollRect.normalizedPosition;
            }
            else
            {
                nextScrollPosition = new Vector2(0, 1 - (selectedIndex / ((float)selectables.Count - 1)));
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ScrollToSelected(false);
    }
}