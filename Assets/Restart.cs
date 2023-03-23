using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Restart : MonoBehaviour
{
    private MyInputActions _myActions;
    public Grid grid;

    private void Awake()
    {
        _myActions = new MyInputActions();
    }
    void OnEnable()
    {
        _myActions.PersonAMap.Restart.started +=
        ctx => HandleStarted();
        _myActions.PersonAMap.Restart.performed += HandleFire;
        _myActions.PersonAMap.Enable();
    }

    void OnDisable()
    {
        _myActions.PersonAMap.Restart.performed -= HandleFire;
        _myActions.PersonAMap.Restart.started -= ctx => HandleStarted();
        _myActions.PersonAMap.Disable();
    }
    private void HandleFire(InputAction.CallbackContext context)
    {
        Debug.Log("Fired Action");
        grid.NewBoard();
    }
    private void HandleStarted()
    {
        Debug.Log("Fired Started");
    }

}
