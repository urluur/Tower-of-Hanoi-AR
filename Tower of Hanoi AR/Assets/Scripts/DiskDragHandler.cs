using UnityEngine;

public class DiskDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Transform nearestStick;
    private Vector3 originalPosition;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Raycast hit: " + hit.transform.name);
                    if (hit.transform == transform)
                    {
                        isDragging = true;
                        offset = transform.position - hit.point;
                        originalPosition = transform.position;
                        Debug.Log("Started dragging " + transform.name);
                    }
                }
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector3 newPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.WorldToScreenPoint(transform.position).z));
                transform.position = newPos + offset;
                Debug.Log("Dragging " + transform.name + " to position: " + transform.position);
            }
            else if (touch.phase == TouchPhase.Ended && isDragging)
            {
                isDragging = false;
                FindNearestStick();
                if (nearestStick != null)
                {
                    transform.position = nearestStick.position;
                    Debug.Log("Dropped " + transform.name + " on " + nearestStick.name);
                }
                else
                {
                    transform.position = originalPosition;
                    Debug.Log("Returned " + transform.name + " to original position");
                }
            }
        }
    }

    private void FindNearestStick()
    {
        float minDistance = float.MaxValue;
        nearestStick = null;

        foreach (Transform stick in GameObject.Find("sticks").transform)
        {
            float distance = Vector3.Distance(transform.position, stick.position);
            if (distance < minDistance && IsValidMove(stick))
            {
                minDistance = distance;
                nearestStick = stick;
            }
        }
    }

    private bool IsValidMove(Transform stick)
    {
        Transform topDisk = null;
        foreach (Transform child in stick)
        {
            if (child.CompareTag("Disk"))
            {
                if (topDisk == null || child.position.y > topDisk.position.y)
                    topDisk = child;
            }
        }

        if (topDisk == null)
        {
            return true;
        }
        else
        {
            float currentDiskSize = transform.localScale.x;
            float topDiskSize = topDisk.localScale.x;

            return currentDiskSize < topDiskSize;
        }
    }
}
