using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[ExportCategory("Required Nodes")]
    [Export] public Camera3D PlayerCameraNode { get; private set; } = null;
	[Export] private Flashlight flashlightNode = null;
    [Export] private Node3D cameraPivotNode = null;
	[Export] private Node3D canisterCarrierNode = null;
	[Export] private RayCast3D interactRaycastNode = null;
	[Export] private Timer interactRaycastTimerNode = null;

	[ExportCategory("UI Nodes")]
	[Export] private Control newEmailNotificationNode = null;

	[ExportCategory("Player Movement")]
	[Export] private float movementSpeed = 3.0f;
	[Export] private float acceleration = 10.0f;
	[Export] private float runMultiplier = 1.8f;
	[Export] private float jumpHeight = 1.0f;
	[Export] private float fallMultiplier = 2.5f;
	[Export] private float mouseSensitivity = 0.003f;


	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	// Global signals
	private Options options;
	private GlobalSignals globalSignals;

	// Running
	private bool isRunning = false;

	// Flashlight
	private bool flashlightIsActive = false;

	// Mouse motion vector
	private Vector2 mouseMotion = Vector2.Zero;

	// Station-specific variables
	private E_StationType activeStationCollider = E_StationType.NONE;
	private bool isClockedIn = false;
	private bool isRelinquishingControl = false;
	private bool isCarryingSlimeCanister = false;

	// Interact raycast result
	private Interactable currentInteractable = null;

	public override void _Ready()
	{
		base._Ready();

		// Get autoloads
		options = GetNode<Options>("/root/Options");
		globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
		SubscribeToEvents();

		// Set intial visiblity variables
		canisterCarrierNode.Visible = false;

		// Make camera current (just in case Godot thinks Roving Camera should be current)
		PlayerCameraNode.MakeCurrent();

		// Set interact raycast node to disabled and start timer to activate
		interactRaycastTimerNode.Start();

		// Stops player moving until black screen has disappeared
		isRelinquishingControl = true;

		// Set flashlight to off
		flashlightNode.ToggleFlashlight(false);
	}

	public override void _ExitTree()
	{
		UnsubscribeFromEvents();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isRelinquishingControl) { return; }

		HandleCameraRotation();
        HandleFlashlightInterpolation(delta);

        Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			if (velocity.Y >= 0)
			{
				velocity.Y -= gravity * (float)delta * 2;
			}
			else
			{
				velocity.Y -= gravity * (float)delta * fallMultiplier;
			}
		}

		// Handle Movement
		Vector2 inputDir = Input.GetVector(GlobalConstants.INPUT_STRAFE_LEFT, GlobalConstants.INPUT_STRAFE_RIGHT, GlobalConstants.INPUT_WALK_FORWARDS, GlobalConstants.INPUT_WALK_BACKWARDS);
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		// Define the target velocity based on input
		float totalSpeed = 0.0f;
		if (isRunning)
		{
			totalSpeed = movementSpeed * runMultiplier;
		}
		else
		{
			totalSpeed = movementSpeed;
		}

		Vector3 targetVelocity = direction * totalSpeed;

		if (targetVelocity != Vector3.Zero)
		{
			velocity.X = Mathf.Lerp(velocity.X, targetVelocity.X, acceleration * (float)delta);
			velocity.Z = Mathf.Lerp(velocity.Z, targetVelocity.Z, acceleration * (float)delta);
		}
		else
		{
			// Forces faster stopping than accelerating
			velocity.X = Mathf.Lerp(velocity.X, targetVelocity.X, acceleration * 2 * (float)delta);
			velocity.Z = Mathf.Lerp(velocity.Z, targetVelocity.Z, acceleration * 2 * (float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    private void HandleFlashlightInterpolation(double delta)
    {
		flashlightNode.InterpLightWithCamera((float)delta, PlayerCameraNode);
    }

    public override void _UnhandledInput(InputEvent @event)
	{
		// CAMERA LOOK
		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			// Control camera with mouse motion
			if (@event is InputEventMouseMotion motion)
			{
				mouseMotion = -motion.Relative * mouseSensitivity;
			}
		}

		// RUN
		if (Input.IsActionJustPressed(GlobalConstants.INPUT_RUN))
		{
			isRunning = true;
		}
		else if (Input.IsActionJustReleased(GlobalConstants.INPUT_RUN))
		{
			isRunning = false;
		}

		// INTERACT WITH STATION/INTERACTABLE
		if (Input.IsActionJustPressed(GlobalConstants.INPUT_INTERACT))
        {
            InteractWithStation();
            InteractWithInteractable();
        }

		// FLASHLIGHT
		if (Input.IsActionJustPressed(GlobalConstants.INPUT_FLASHLIGHT))
		{
			if (!flashlightIsActive)
			{
				flashlightNode.ToggleFlashlight(true);
				flashlightIsActive = true;
            }
			else
			{
                flashlightNode.ToggleFlashlight(false);
                flashlightIsActive = false;
            }
        }

        // SHOW CURSOR
        if (Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public void MakeCameraCurrent()
	{
		PlayerCameraNode.MakeCurrent();
	}

    private void InteractWithStation()
    {
        // No active station
        if (activeStationCollider == E_StationType.NONE)
        {
            return;
        }

        // Check if the player is currently not relinquishing control
        if (isRelinquishingControl)
        {
            // Player is exiting the station interaction
            globalSignals.RaisePlayerExitStation(activeStationCollider);
            isRelinquishingControl = false;
            return;
        }

        // If player is not clocked in, allow only CLOCKOUT or COMPUTER interactions
        if (!isClockedIn)
        {
            if (activeStationCollider == E_StationType.CLOCKOUT || activeStationCollider == E_StationType.COMPUTER)
            {
                InteractWithCurrentStation();
            }
            else
            {
                GD.PrintErr("Need to clock in first!");
            }
            return;
        }

        // If carrying a slime canister, allow only SLIMECOLLECTION interactions
        if (isCarryingSlimeCanister)
        {
			if (activeStationCollider == E_StationType.SLIMECOLLECTION)
			{
				InteractWithCurrentStation();
			}
			else
			{
                GD.PrintErr("Cannot interact while carrying a slime canister!");
                return;
            }
        }

        // If clocked in and not carrying a slime canister, allow full station interaction
        InteractWithCurrentStation();
    }

    private void InteractWithCurrentStation()
    {
        // Stop movement and any related sound effects
        Velocity = Vector3.Zero;

        // Trigger the interaction signal and relinquish control
        globalSignals.RaisePlayerInteractWithStation(activeStationCollider);
        isRelinquishingControl = true;
    }


    private void InteractWithInteractable()
    {
        if (currentInteractable != null)
        {
            GD.Print("Calling Interact on Interactable");
            currentInteractable.Interact();
        }
    }

    private void SubscribeToEvents()
	{
		globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
		globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;
		globalSignals.OnPlayerEnterStationCollider += HandlePlayerEnterStationCollider;
		globalSignals.OnPlayerExitStationCollider += HandlePlayerExitStationCollider;
		globalSignals.OnPlayerCanMoveAgain += HandlePlayerCanMoveAgain;
		globalSignals.OnSlimeCanisterTakenFromStorage += HandleSlimeCanisterTakenFromStorage;
		globalSignals.OnSlimeCanisterAddedToStation += HandleSlimeCanisterAddedToStation;
		globalSignals.OnBlackScreenDisappeared += HandleBlackScreenDisappeared;
		interactRaycastTimerNode.Timeout += HandleInteractRaycastTimerTimeout;
	}

    private void UnsubscribeFromEvents()
	{
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;
        globalSignals.OnPlayerEnterStationCollider -= HandlePlayerEnterStationCollider;
		globalSignals.OnPlayerExitStationCollider -= HandlePlayerExitStationCollider;
		globalSignals.OnSlimeCanisterTakenFromStorage -= HandleSlimeCanisterTakenFromStorage;
		globalSignals.OnSlimeCanisterAddedToStation -= HandleSlimeCanisterAddedToStation;
		globalSignals.OnBlackScreenDisappeared -= HandleBlackScreenDisappeared;
		interactRaycastTimerNode.Timeout -= HandleInteractRaycastTimerTimeout;
	}

	private void HandleCameraRotation()
	{
		// Rotate around Y-axis using mouse X movement
		RotateY(mouseMotion.X);

		// Move camera via pivot but clamp to prevent it spinning all the way around on the X axis
		if (options.IsInverted)
		{
			cameraPivotNode.RotateX(-mouseMotion.Y);
		}
		else
		{
			cameraPivotNode.RotateX(mouseMotion.Y);
		}
		Vector3 rotationDegrees;
		rotationDegrees.X = Mathf.Clamp(cameraPivotNode.RotationDegrees.X, -85.0f, 85.0f);
		rotationDegrees.Y = cameraPivotNode.RotationDegrees.Y;
		rotationDegrees.Z = cameraPivotNode.RotationDegrees.Z;

		cameraPivotNode.RotationDegrees = rotationDegrees;

		// Reset mouseMotion to prevent infinite movement
		mouseMotion = Vector2.Zero;
	}

    private void HandlePlayerClockedIn()
    {
        isClockedIn = true;
    }

    private void HandlePlayerClockedOut()
    {
        isClockedIn = false;
    }

    private void HandlePlayerEnterStationCollider(E_StationType stationType)
	{
		if (activeStationCollider != stationType)
		{
			activeStationCollider = stationType;
		}
	}

	private void HandlePlayerExitStationCollider(E_StationType stationType)
	{
		if (activeStationCollider == stationType)
		{
			activeStationCollider = E_StationType.NONE;
		}
	}

	private void HandleSlimeCanisterTakenFromStorage()
	{
		canisterCarrierNode.Visible = true;
		isCarryingSlimeCanister = true;
	}

	private void HandleSlimeCanisterAddedToStation()
	{
		canisterCarrierNode.Visible = false;
		isCarryingSlimeCanister = false;
	}

	private void HandlePlayerCanMoveAgain()
	{
		PlayerCameraNode.MakeCurrent();
		isRelinquishingControl = false;
	}

	private void HandleInteractRaycastTimerTimeout()
	{
		if (interactRaycastNode.GetCollider() != null)
		{
			currentInteractable = (Interactable)interactRaycastNode.GetCollider();
		}
		else
		{
			currentInteractable = null;
		}
		interactRaycastTimerNode.Start();
	}

	private void HandleBlackScreenDisappeared()
	{
		isRelinquishingControl = false;
	}
}
