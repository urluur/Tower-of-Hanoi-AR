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
                    else
                    {
                        Debug.Log("Raycast hit, but not this object: " + hit.transform.name);
                    }
                }
                else
                {
                    Debug.Log("Raycast did not hit any object.");
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
                    // Snap to the nearest stick, if rules are followed
                    transform.position = nearestStick.position;
                    Debug.Log("Dropped " + transform.name + " on " + nearestStick.name);
                }
                else
                {
                    // Return to original position if no valid stick is found
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

        if (nearestStick != null)
        {
            Debug.Log("Nearest stick found: " + nearestStick.name);
        }
        else
        {
            Debug.Log("No valid stick found.");
        }
    }

    private bool IsValidMove(Transform stick)
    {
        // Get the topmost disk on the stick
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
            return true; // The stick is empty, valid move
        }
        else
        {
            // Check if the current disk is smaller than the top disk
            float currentDiskSize = transform.localScale.x;
            float topDiskSize = topDisk.localScale.x;

            return currentDiskSize < topDiskSize;
        }
    }
}
