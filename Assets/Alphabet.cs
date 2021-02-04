using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Alphabet : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public delegate void OnalphabetPressed(char alphabet);
    public static event OnalphabetPressed pressed;

    public void AlphabetPressed(char a) {
        pressed.Invoke(a);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed(GetComponent<Text>().text.ToCharArray()[0]);
        Destroy(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}