using UnityEngine;
using UnityEngine.EventSystems;

public class TimelineScreen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TimelineEditor tle;
    private bool mouse_over = false;
    public bool otherScroll;
    public bool rightClickBitBar;
    public bool deleteClickBar;
    public int rCBitBarNumber;

    void Update()
    {
        if (mouse_over)
        {
            if (otherScroll)
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    tle.ScrollBitGroups(-Input.mouseScrollDelta.y);
                }
            }
            else
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    tle.ZoomTimeline(-Input.mouseScrollDelta.y);
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                tle.ClickedTimeline();
            }
            if (Input.GetMouseButtonDown(1) && rightClickBitBar)
            {
                tle.BitLineRightClick(rCBitBarNumber);
            }
            if (Input.GetMouseButtonDown(1) && deleteClickBar)
            {
                float section = Screen.height / 2.2f;
                float goodGuess = Mathf.Floor(Mathf.Abs((section - Input.mousePosition.y) / (Screen.height - section) * 14f))-1;
                Debug.Log(goodGuess);
                tle.EraseBitBox((int)goodGuess);
            }
            if (Input.GetMouseButtonDown(2))
            {
                tle.DraggedTimeline(true);
            }
        }
        if (Input.GetMouseButtonUp(2))
        {
            tle.DraggedTimeline(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }
}