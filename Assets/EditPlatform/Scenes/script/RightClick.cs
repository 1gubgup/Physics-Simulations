using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerClickHandler
{
    
    public UnityEvent rightClick;
    public GameObject Menu;
    private int legacy;

    void Start()
    {
        rightClick.AddListener(new UnityAction(ButtonRightClick));
    }

    void Update()
    {
        
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            rightClick.Invoke();
        }
    }

    private void ButtonRightClick()
    {
        Menu.SetActive(true);
        Menu.transform.position = gameObject.transform.position + new Vector3(0, -75, 0);
        Menu.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, Menu.GetComponent<RectTransform>().sizeDelta.y);
    }
}
