using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{

    public GameObject joystick;
    public GameObject joystickBG;
    public Vector2 joystickVec;
    public bool shoot;
    public RectTransform area;
    private Vector2 joystickTouchPos;
    private Vector2 joystickOriginalPos;
    private float joystickRadius;
    public bool leftTouchPad;
    private Touch touch;
    private Touch touchLeft;
    private Touch touchRight;
    // Start is called before the first frame update
    void Start()
    {
        joystickOriginalPos = joystickBG.transform.position;
        joystickRadius = joystickBG.GetComponent<RectTransform>().sizeDelta.y * 3;
    }

    public void PointerDown()
    {
        foreach(Touch touch in Input.touches)
        {
            if(touch.phase == TouchPhase.Began && touch.position.x <= Screen.width / 2)
            {
                touchLeft = touch;
            }
            if(touch.phase == TouchPhase.Began && touch.position.x > Screen.width / 2)
            {
                touchRight = touch;
            }
        }
        if (leftTouchPad)
        {
            joystick.transform.position = touchLeft.position;
            joystickBG.transform.position = touchLeft.position;
            joystickTouchPos = touchLeft.position;
        }
        else
        {
            joystick.transform.position = touchRight.position;
            joystickBG.transform.position = touchRight.position;
            joystickTouchPos = touchRight.position;
        }
        
        /*joystick.transform.position = Input.mousePosition;
        joystickBG.transform.position = Input.mousePosition;
        joystickTouchPos = Input.mousePosition;*/
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joystickVec = (dragPos - joystickTouchPos).normalized;

        float joystickDist = Vector2.Distance(dragPos, joystickTouchPos);

        if (joystickDist < joystickRadius)
        {
            joystick.transform.position = joystickTouchPos + joystickVec * joystickDist;
        }

        else
        {
            joystick.transform.position = joystickTouchPos + joystickVec * joystickRadius;
        }
        if (joystickDist >= joystickRadius / 1.3f)
        {
            shoot = true;
        }
        else
        {
            shoot = false;
        }
        /*PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joystickVec = (dragPos - joystickTouchPos).normalized;

        float joystickDist = Vector2.Distance(dragPos, joystickTouchPos);

        if (joystickDist < joystickRadius)
        {
            joystick.transform.position = joystickTouchPos + joystickVec * joystickDist;
        }

        else
        {
            joystick.transform.position = joystickTouchPos + joystickVec * joystickRadius;
        }
        if(joystickDist >= joystickRadius / 1.3f)
        {
            shoot = true;
        }
        else
        {
            shoot = false;
        }*/
    }

    public void PointerUp()
    {
        joystickVec = Vector2.zero;
        joystick.transform.position = joystickOriginalPos;
        joystickBG.transform.position = joystickOriginalPos;
    }

}
