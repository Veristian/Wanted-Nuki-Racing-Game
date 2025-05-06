using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;

    public bool MenuOpenCloseInput {  get; private set; }

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _BoostAction;
    private InputAction _DriftAction;
    private InputAction _menuOpenCloseAction;

    public static bool hasPressedDriftButton;
    public static bool hasReleasedDriftButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _lookAction = PlayerInput.actions["Look"];
        _BoostAction = PlayerInput.actions["Boost"];
        _DriftAction = PlayerInput.actions["Drift"];
        _menuOpenCloseAction = PlayerInput.actions["MenuOpenClose"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        hasPressedDriftButton = _DriftAction.WasPressedThisFrame();
        hasReleasedDriftButton = _DriftAction.WasReleasedThisFrame();
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
    }
}
