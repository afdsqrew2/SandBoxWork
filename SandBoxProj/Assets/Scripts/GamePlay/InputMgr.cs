using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputMgr : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    private void Update()
    {
        if(Mouse.current!=null && Mouse.current.leftButton.isPressed)
            OnClicked?.Invoke();
        
        if(Touchscreen.current!= null && Touchscreen.current.touches.Count>0 && Touchscreen.current.touches[0].press.wasPressedThisFrame)
            OnClicked?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Vector3.zero;
        if (Mouse.current != null)
        {
             mousePos = Mouse.current.position.ReadValue();
        }
        
        if (Touchscreen.current!= null && Touchscreen.current.touches.Count>0)
        {
            mousePos = Touchscreen.current.touches[0].position.ReadValue();
        }

        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
