using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [ExportCategory("Required Nodes")]
    [Export] private Node3D cameraPivotNode = null;
    [Export] private Camera3D playerCameraNode = null;

    [ExportCategory("Player Movement")]
    [Export] private float movementSpeed = 3.0f;
    [Export] private float acceleration = 10.0f;
    [Export] private float jumpHeight = 1.0f;
    [Export] private float fallMultiplier = 2.5f;
    [Export] private float mouseSensitivity = 0.003f;


    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    // Global signals
    private Options options;
    private GlobalSignals globalSignals;

    // Mouse motion vector
    private Vector2 mouseMotion = Vector2.Zero;

    // Station-specific variables
    private E_StationType activeStationCollider = E_StationType.NONE;

    public override void _Ready()
    {
        base._Ready();

        // Get Options autoload
        options = GetNode<Options>("/root/Options");

        // Get GlobalSignals autoload and assign signals
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnPlayerEnterStationCollider += HandlePlayerEnterStationCollider;
        globalSignals.OnPlayerExitStationCollider += HandlePlayerExitStationCollider;

        // Capture mouse cursor on start
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _ExitTree()
    {
        globalSignals.OnPlayerEnterStationCollider -= HandlePlayerEnterStationCollider;
        globalSignals.OnPlayerExitStationCollider -= HandlePlayerExitStationCollider;
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleCameraRotation();

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

        // Handle Jump.
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_JUMP) && IsOnFloor())
            velocity.Y = (float)Mathf.Sqrt(jumpHeight * 2.0 * gravity);

        // Handle Movement
        Vector2 inputDir = Input.GetVector(GlobalConstants.INPUT_STRAFE_LEFT, GlobalConstants.INPUT_STRAFE_RIGHT, GlobalConstants.INPUT_WALK_FORWARDS, GlobalConstants.INPUT_WALK_BACKWARDS);
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        // Define the target velocity based on input
        Vector3 targetVelocity = direction * movementSpeed;

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

        // INTERACT WITH STATION
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_INTERACT))
        {
            if (activeStationCollider == E_StationType.NONE) { return; }

            globalSignals.RaisePlayerInteractWithStation(activeStationCollider);
        }

        // SHOW CURSOR
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
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

    private void HandlePlayerEnterStationCollider(E_StationType stationType)
    {
        if (activeStationCollider != stationType)
        {
            activeStationCollider = stationType;
            GD.Print($"{Name} active station collider: {activeStationCollider.ToString()}");
        }        
    }

    private void HandlePlayerExitStationCollider(E_StationType stationType)
    {
        if (activeStationCollider == stationType)
        {
            activeStationCollider = E_StationType.NONE;
            GD.Print($"{Name} active station collider: {activeStationCollider.ToString()}");
        }
    }
}
