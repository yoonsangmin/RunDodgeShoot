using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerControl playerControl;
    public enum Direction
    {
        Left,
        Right,
    }
    public Direction direction;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (direction == Direction.Left)
            playerControl.OnLeftButtonDown();
        if (direction == Direction.Right)
            playerControl.OnRightButtonDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (direction == Direction.Left)
            playerControl.OnLeftButtonUp();
        if (direction == Direction.Right)
            playerControl.OnRightButtonUp();
    }
}
