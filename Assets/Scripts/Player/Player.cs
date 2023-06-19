using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Initial")]
    //Attatched Objects
    public Camera PlayerCamScript;
    public TMP_Text playerText;
    public TMP_Text agitText;
    public FAde fadeObj;
    public Image keyboardLayout;

    //Position, Movement, Buttons
    [Tooltip("Initial Camera X position")]
    public float camXRotation;
    [Tooltip("Initial Camera Y position")]
    public float camYRotation;

    //Speeds and Base attributes
    [Header("VR")]
    public bool isVR = false;
    public GameObject XRRig;

    [Header("Speed")]
    public float baseSpeed = 2f;
    public float crouchSpeed = 1f;
    public float sprintSpeed = 2.5f;

    [Header("Jump")]
    public bool enableJump;
    public float gravity = 12f;
    public float jumpSpeed = 9f;
    public float airControl = 1;
    public float airTurnSpeed = 1;

    [Header("Crouch")]
    public bool enableCrouch;
    public float camInitialHeight = 0.657f;
    public float camCrouchHeight = 0f;
    public GameObject feet;
    public GameObject unCrouch;

    [Header("WorldSpace UI")]
    public bool enableUIClick;
    public GameObject cursor;
    public TMP_Text cursorText;
    public LayerMask uiLayerMask;
    public LayerMask uiCueLayerMask;
    public LayerMask playerLayerMask;

    [Header("Flashlight")]
    public bool enableFlashlight;
    public GameObject flashlight;
    public GameObject VRflashlight;
    public int flashState;

    [Header("CameraZoom")]
    public bool enableCamZoom;
    public float maxFov = 170;
    public float minFov = 1;

    [Header("CameraSmooth")]
    public bool enableCamSmooth;
    public float smoothSpeed;
    public float maxVeclocity;
    Vector2 camAcceleration;

    [Header("Footstep")]
    Vector3 oldPosition;
    public AudioSource footstepSpeaker;
    public FootstepType[] meshTypes;

    [Header("Anims")]
    Animator _animator;
    public GameObject headBone;
    public GameObject neckBone;
    public GameObject spineBone;

    [Header("PlayerState")]
    public PlayerState playerState;

    public enum PlayerState
    {
        normal,
        frozenBody,
        frozenCam,
        frozenAll,
        frozenCamUnlock,
        frozenAllUnlock,
    }

    [Header("Player Items")]
    public CharacterItems[] itemList;
    public string currentItem;
    public float itemWheelCooldown = 0;
    public PlayerInteractions playerInter;
    public ScreenShotItem screenShotItem;
    public PrizeDeleteItem prizeDeleteItem;
    public GameObject pauseMenu;
    public bool canPause = true;

    //Other
    CharacterController CharCont;
    Vector3 moveDirection = Vector3.zero;
    Vector2 CStick;
    Vector2 JoyStick;
    public float JumpBool;
    int JumpFrames;
    float smoothScroll;
    float timedelta;
    float flashsmoothScroll = 63;
    bool crouchBool;
    float camHeight;
    [HideInInspector]
    public Vector2 holdingRotation;

    //New Input
    [SerializeField]
    private Controller gamepad;
    [Header("Gamepad")]
    public ControllerType controlType;
    private bool clickGamepad = false;
    private bool jumpGamepad = false;
    private bool crouchGamepad = false;
    private bool flashGamepad = false;
    private bool runGamepad = false;
    private bool holdGamepad = false;
    public Vector2 GPJoy;
    public Vector2 GPCam;
    public Vector2 GPZoom;
    float groundedTimeAnim = 0;
    bool fixedcamera = false;
    bool newvr = false;
    bool fixedUpdatelowerFPS;
    public enum ControllerType
    {
        keyboard,
        gamepad,
    }
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gamepad.Gamepad.Enable();
    }

    void OnDisable()
    {
        gamepad.Gamepad.Disable();
    }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        camHeight = camInitialHeight;
        smoothScroll = PlayerCamScript.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        //Initialize Variables
        _animator = this.transform.Find("PlayerModel").GetComponent<Animator>();
        CharCont = GetComponent<CharacterController>();
        oldPosition = this.transform.position;
        gamepad = new Controller();
        gamepad.Gamepad.Click.canceled += ctx => clickGamepad = false;
        gamepad.Gamepad.Click.performed += ctx => clickGamepad = true;
        gamepad.Gamepad.Jump.canceled += ctx => jumpGamepad = false;
        gamepad.Gamepad.Jump.performed += ctx => jumpGamepad = true;
        gamepad.Gamepad.Flashlight.canceled += ctx => flashGamepad = false;
        gamepad.Gamepad.Flashlight.started += ctx => flashGamepad = true;
        gamepad.Gamepad.Run.canceled += ctx => runGamepad = false;
        gamepad.Gamepad.Run.performed += ctx => runGamepad = true;
        gamepad.Gamepad.Crouch.canceled += ctx => crouchGamepad = false;
        gamepad.Gamepad.Crouch.performed += ctx => crouchGamepad = true;
        gamepad.Gamepad.Hold.canceled += ctx => holdGamepad = false;
        gamepad.Gamepad.Hold.performed += ctx => holdGamepad = true;
        gamepad.Gamepad.Horizontal.performed += ctx => GPJoy.x = ctx.ReadValue<float>();
        gamepad.Gamepad.Vertical.performed += ctx => GPJoy.y = ctx.ReadValue<float>();
        gamepad.Gamepad.Horizontal.canceled += ctx => GPJoy.x = 0;
        gamepad.Gamepad.Vertical.canceled += ctx => GPJoy.y = 0;
        gamepad.Gamepad.CamHorizontal.performed += ctx => GPCam.x = ctx.ReadValue<float>();
        gamepad.Gamepad.CamVertical.performed += ctx => GPCam.y = ctx.ReadValue<float>();
        gamepad.Gamepad.Zoom.performed += ctx => GPZoom.x = ctx.ReadValue<float>();
        gamepad.Gamepad.FlashZoom.performed += ctx => GPZoom.y = ctx.ReadValue<float>();
        gamepad.Gamepad.Zoom.canceled += ctx => GPZoom.x = 0;
        gamepad.Gamepad.FlashZoom.canceled += ctx => GPZoom.y = 0;
        gamepad.Gamepad.CamHorizontal.canceled += ctx => GPCam.x = 0;
        gamepad.Gamepad.CamVertical.canceled += ctx => GPCam.y = 0;
    }

    private void FixedUpdate()
    {
        fixedUpdatelowerFPS = !fixedUpdatelowerFPS;

        if (fixedUpdatelowerFPS)
        {
            //UI Click
            if (enableUIClick)
            {
                RayCastClick();
            }
        }
    }

    void Update()
    {
        if (isVR && !fixedcamera)
        {
            if (PlayerCamScript == null)
            {
                fixedcamera = true;
                PlayerCamScript = Camera.main;
            }
        }
        //VR Mess
        if (isVR)
        {
            Vector3 temp = XRRig.transform.position;
            Vector3 trans = PlayerCamScript.transform.position - this.transform.position;
            trans.y = 0;
            this.transform.position += trans;
            XRRig.transform.position = temp;
            if (!newvr)
            {
                newvr = true;
                flashlight = VRflashlight;
                jumpSpeed /= 3;
                gravity /= 9;
            }
        }

        //Joystick
        JoyStickCheck();

        //Cam Crouch Code
        PlayerCamScript.transform.localPosition = new Vector3(PlayerCamScript.transform.localPosition.x, Mathf.Lerp(PlayerCamScript.transform.localPosition.y, camHeight, Time.deltaTime * 5), PlayerCamScript.transform.localPosition.z);

        //Camera Code
        if (playerState != PlayerState.frozenCam && playerState != PlayerState.frozenAll && playerState != PlayerState.frozenAllUnlock && playerState != PlayerState.frozenCamUnlock && !isVR)
        {
            //Cam Zoom
            if (enableCamZoom)
            {
                CamZoomCheck();
            }

            bool camMove = true;
            //Items
            switch (currentItem)
            {
                case "Item Pickup":
                    playerInter.enabled = true;
                    screenShotItem.enabled = false;
                    prizeDeleteItem.enabled = false;
                    camMove = playerInter.PickupCheck(false);
                    break;
                case "Item Freeze":
                    playerInter.enabled = true;
                    screenShotItem.enabled = false;
                    prizeDeleteItem.enabled = false;
                    camMove = playerInter.PickupCheck(true);
                    break;
                case "Screenshot":
                    playerInter.enabled = false;
                    screenShotItem.enabled = true;
                    prizeDeleteItem.enabled = false;
                    screenShotItem.ScreenshotCheck();
                    break;
                case "Item Eraser":
                    playerInter.enabled = false;
                    screenShotItem.enabled = false;
                    prizeDeleteItem.enabled = true;
                    prizeDeleteItem.DeleterCheck();
                    break;
                default:
                    break;
            }

            //Camera
            if (camMove)
            {
                CameraMove(CStick);
            }
        }

        //Flashlight
        if (enableFlashlight)
        {
            FlashlightCheck();
        }

        //Body Code
        if (playerState != PlayerState.frozenBody && playerState != PlayerState.frozenAll && playerState != PlayerState.frozenAllUnlock)
        {
            //Item Wheel
            if (Input.GetKeyDown(KeyCode.Z) && controlType == ControllerType.keyboard)
            {
                OpenItemWheel();
            }
            if (itemWheelCooldown > 0)
            {
                itemWheelCooldown -= Time.deltaTime;
            }

            //Pause
            if (Input.GetKeyDown(KeyCode.Escape) && controlType == ControllerType.keyboard && canPause)
            {
                pauseMenu.SetActive(true);
                playerState = PlayerState.frozenAllUnlock;
            }

            //Jump
            if (!isVR)
            {
                if (enableJump && ((Input.GetKey(KeyCode.Space) && controlType == ControllerType.keyboard) || (jumpGamepad && controlType == ControllerType.gamepad)))
                {
                    JumpBool++;
                    UncrouchCheck();
                }
                else
                {
                    JumpBool = 0;
                }
            }
            //Crouch
            if (enableCrouch)
            {
                CrouchCheck();
            }
            //Footstep
            if (Vector3.Distance(this.transform.position, oldPosition) > 1.3f && CharCont.isGrounded)
            {
                FootstepSoundCheck();
            }
            //Move
            MovePlayer(JoyStick, false);
        }
        else
        {
            //UnPause
            if (Input.GetKeyDown(KeyCode.Escape) && controlType == ControllerType.keyboard)
            {
                if (pauseMenu.activeSelf)
                {
                    pauseMenu.SetActive(false);
                    playerState = PlayerState.normal;
                }
            }
        }

        switch (Cursor.lockState)
        {
            case CursorLockMode.None:
                if (playerState != PlayerState.frozenAllUnlock && playerState != PlayerState.frozenCamUnlock)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                break;
            case CursorLockMode.Locked:
                if (playerState == PlayerState.frozenAllUnlock || playerState == PlayerState.frozenCamUnlock)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                break;
            default:
                break;
        }

        //Anim
        AnimCheck();
    }


    private void LateUpdate()
    {
        headBone.transform.rotation = Quaternion.Lerp(headBone.transform.rotation, Quaternion.LookRotation(PlayerCamScript.transform.forward, headBone.transform.up), 0.4f);
        neckBone.transform.rotation = Quaternion.Lerp(neckBone.transform.rotation, Quaternion.LookRotation(PlayerCamScript.transform.forward, neckBone.transform.up), 0.4f);
        spineBone.transform.rotation = Quaternion.Lerp(spineBone.transform.rotation, Quaternion.LookRotation(PlayerCamScript.transform.forward, spineBone.transform.up), 0.2f);
    }

    void FootstepSoundCheck()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, ~playerLayerMask))
        {
            for (int i = 0; i < meshTypes.Length; i++)
            {
                if (hit.collider.gameObject.name == meshTypes[i].meshName)
                {
                    footstepSpeaker.clip = meshTypes[i].clips[UnityEngine.Random.Range(0, meshTypes[i].clips.Length)];
                    footstepSpeaker.volume = meshTypes[i].volume;

                    //Volume
                    float nowSpeed = 0.3f;
                    if ((Input.GetKey(KeyCode.LeftShift) && controlType == ControllerType.keyboard) || (runGamepad && controlType == ControllerType.gamepad))
                    {
                        nowSpeed = .7f;
                    }
                    if ((Input.GetKey(KeyCode.LeftControl) && enableCrouch && controlType == ControllerType.keyboard) || (crouchGamepad && enableCrouch && controlType == ControllerType.gamepad))
                    {
                        nowSpeed = .2f;
                    }
                    footstepSpeaker.volume *= nowSpeed;

                    footstepSpeaker.Play();
                }
            }
        }
        oldPosition = this.transform.position;
    }

    float Remap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    float realModulo(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    void UncrouchCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(unCrouch.transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
        {
            if (hit.point.y > PlayerCamScript.transform.position.y + camInitialHeight)
            {
                crouchBool = false;
            }
        }
    }

    void CameraMove(Vector2 axis)
    {

        if (enableCamSmooth)
        {
            camAcceleration += axis;
            if (camAcceleration.x > 0)
            {
                camAcceleration.x -= smoothSpeed;
            }
            else if (camAcceleration.x < 0)
            {
                camAcceleration.x += smoothSpeed;
            }
            if (camAcceleration.x < .1f && camAcceleration.x > -.1f)
            {
                camAcceleration.x = 0;
            }
            if (camAcceleration.y > 0)
            {
                camAcceleration.y -= smoothSpeed;
            }
            else if (camAcceleration.y < 0)
            {
                camAcceleration.y += smoothSpeed;
            }
            if (camAcceleration.y < .5f && camAcceleration.y > -.5f)
            {
                camAcceleration.y = 0;
            }
            camAcceleration.x = Mathf.Max(Mathf.Min(camAcceleration.x, maxVeclocity), -maxVeclocity);
            camAcceleration.y = Mathf.Max(Mathf.Min(camAcceleration.y, maxVeclocity), -maxVeclocity);
            camXRotation += camAcceleration.x / 100f;
            camYRotation += camAcceleration.y / 100f;
        }
        else
        {
            camXRotation += axis.x;
            camYRotation += axis.y;
        }


        camYRotation = Mathf.Clamp(camYRotation, -85, 85);


        PlayerCamScript.transform.eulerAngles = new Vector3(camYRotation, PlayerCamScript.transform.eulerAngles.y, PlayerCamScript.transform.eulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, camXRotation, transform.eulerAngles.z);
    }

    public void MovePlayer(Vector2 axis, bool sprint)
    {
        float newy = PlayerCamScript.transform.position.y - this.transform.position.y;
        CharCont.height = (((newy * (1 / camInitialHeight)) / 2f) + .5f) * 1.3f;
        CharCont.center = new Vector3(0f, Remap(newy, feet.transform.localPosition.y, camInitialHeight, feet.transform.localPosition.y, 0), 0);

        //Void Bounce
        if (this.transform.position.y < -20)
        {
            this.transform.position = new Vector3(this.transform.position.x, 100f, this.transform.position.z);
            moveDirection = this.transform.position;
        }
        float nowSpeed = baseSpeed;
        if (((Input.GetKey(KeyCode.LeftShift) && controlType == ControllerType.keyboard) || (runGamepad && controlType == ControllerType.gamepad)) || sprint && !crouchBool)
        {
            nowSpeed = sprintSpeed;
            _animator.SetBool("Sprint", true);
        }
        else
        {
            _animator.SetBool("Sprint", false);
        }
        if (crouchBool)
        {
            nowSpeed = crouchSpeed;
        }

        Vector3 transForward;
        Vector3 transRight;

        if (isVR)
        {
            transForward = PlayerCamScript.transform.forward;
            transRight = PlayerCamScript.transform.right;
            transForward.y = 0;
            transRight.y = 0;
            transForward.Normalize();
            transRight.Normalize();
        }
        else
        {
            transForward = transform.forward;
            transRight = transform.right;
        }

        if (CharCont.isGrounded)
        {

            moveDirection = ((transForward * axis.y * nowSpeed) + (transRight * (axis.x * nowSpeed)));

            //Jumping
            if (JumpBool == 1)
            {
                JumpFrames++;
            }
            else
            {
                JumpFrames = 0;
            }
            if (JumpFrames == 1)
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else
        {
            moveDirection = (((transForward * axis.y * nowSpeed) + (transRight * (axis.x * nowSpeed * airTurnSpeed))) * airControl) + new Vector3(0, moveDirection.y, 0);
        }
        moveDirection.y -= gravity * Time.deltaTime;
        CharCont.Move(moveDirection * Time.deltaTime);
    }

    void RayCastClick()
    {
        cursor.SetActive(false);
        RaycastHit hit;
        if (Physics.Raycast(PlayerCamScript.transform.position, PlayerCamScript.transform.forward, out hit, 10f, uiLayerMask))
        {
            Button3D hitcol = hit.collider.GetComponent<Button3D>();
            if (hitcol != null)
            {
                cursor.SetActive(true);
                cursorText.text = hitcol.buttonText;
                hitcol.Highlight(gameObject.name);

                switch (mouseCheck())
                {
                    case true:
                        hitcol.StartClick(gameObject.name);
                        break;
                    case false:
                        hitcol.EndClick(gameObject.name);
                        break;
                }

            }
        }
    }

    bool mouseCheck()
    {
        switch (controlType)
        {
            case ControllerType.keyboard:
                if (Input.GetMouseButton(0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case ControllerType.gamepad:
                if (clickGamepad)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                break;
        }
        return false;
    }

    void FlashlightCheck()
    {
        if ((Input.GetKeyDown(KeyCode.E) && controlType == ControllerType.keyboard) || (flashGamepad && controlType == ControllerType.gamepad) || flashState == 1)
        {
            flashGamepad = false;
            flashlight.SetActive(!flashlight.activeSelf);
            if (flashlight.activeSelf)
            {
                AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>(); Resources.Load("ting");
                sc.clip = (AudioClip)Resources.Load("Flashlight On");
                sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                sc.Play();
            }
            else
            {
                AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>(); Resources.Load("ting");
                sc.clip = (AudioClip)Resources.Load("Flashlight Off");
                sc.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
                sc.Play();
            }
        }
        if (controlType == ControllerType.keyboard)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                flashsmoothScroll += Input.GetAxis("Mouse ScrollWheel") * 25f;
                flashsmoothScroll = Mathf.Clamp(flashsmoothScroll, 5, 160);
                if (Input.GetAxis("Mouse ScrollWheel") != 0 && (flashsmoothScroll > 5 && flashsmoothScroll < 160))
                {
                    AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                    sc.clip = (AudioClip)Resources.Load("Flashlight Click");
                    sc.pitch = .5F + (flashsmoothScroll / 320);
                    sc.Play();
                }
            }
        }
        else
        {
            flashsmoothScroll += GPZoom.y;
            flashsmoothScroll = Mathf.Clamp(flashsmoothScroll, minFov, maxFov);
        }
        flashlight.GetComponent<Light>().spotAngle = Mathf.Lerp(flashlight.GetComponent<Light>().spotAngle, flashsmoothScroll, Time.deltaTime * 5);
    }

    void CrouchCheck()
    {
        if (CharCont.isGrounded)
        {
            if ((Input.GetKey(KeyCode.LeftControl) != crouchBool && controlType == ControllerType.keyboard) || (crouchGamepad != crouchBool && controlType == ControllerType.gamepad))
            {
                crouchBool = !crouchBool;
                if (!crouchBool)
                {
                    crouchBool = true;
                    UncrouchCheck();
                }
                timedelta = 0;

            }
        }
        else
        {
            timedelta += Time.deltaTime;
        }
        _animator.SetBool("Crouch", crouchBool);
        //Crouch height
        if (!crouchBool)
        {
            camHeight = camInitialHeight;
        }
        else
        {
            camHeight = camCrouchHeight;
        }

    }

    void JoyStickCheck()
    {
        CStick = Vector2.zero;
        JoyStick = Vector2.zero;
        switch (controlType)
        {
            case ControllerType.keyboard:
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    JoyStick.y += 1.0f;
                }
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    JoyStick.x -= 1.0f;
                }
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    JoyStick.y -= 1.0f;
                }
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    JoyStick.x += 1.0f;
                }
                CStick.x = Input.GetAxis("Mouse X") * 1.5f;
                CStick.y = Input.GetAxis("Mouse Y") * -1.5f;
                break;
            case ControllerType.gamepad:
                JoyStick = GPJoy;
                CStick = GPCam * 2;
                break;
            default:
                break;
        }
        //Anims
        _animator.SetFloat("Velocity Z", Mathf.Min(JoyStick.y * 10, 1), 0.1f, Time.deltaTime);
        _animator.SetFloat("Velocity X", Mathf.Min(JoyStick.x * 10, 1), 0.1f, Time.deltaTime);
        JoyStick = JoyStick.normalized;
    }

    void AnimCheck()
    {
        if (!CharCont.isGrounded)
        {
            groundedTimeAnim = 0;
        }
        else
        {
            groundedTimeAnim += Time.deltaTime;
        }
        if (groundedTimeAnim > 0.1f)
        {
            _animator.SetBool("Grounded", true);
        }
        else
        {
            _animator.SetBool("Grounded", false);
        }
    }

    void CamZoomCheck()
    {
        if (controlType == ControllerType.keyboard)
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    timedelta = 0;
                }
                else
                {
                    timedelta += Time.deltaTime;
                }
                smoothScroll += Input.GetAxis("Mouse ScrollWheel") * 25f;
                smoothScroll = Mathf.Clamp(smoothScroll, minFov, maxFov);
            }
        }
        else
        {
            smoothScroll += GPZoom.x;
            smoothScroll = Mathf.Clamp(smoothScroll, minFov, maxFov);
        }
        PlayerCamScript.fieldOfView = Mathf.Lerp(PlayerCamScript.fieldOfView, smoothScroll, Time.deltaTime * 5);
    }

    public void DisplayMessage(Texture2D image, byte todarkness, byte tospeed)
    {
        SetFade(todarkness, tospeed);
        GameObject sm = transform.Find("Player UI").Find("System Message").gameObject;
        sm.SetActive(true);
        sm.transform.Find("Message").GetComponent<RawImage>().texture = image;
        sm.GetComponent<Animator>().Play("SystemMessage");
    }

    public void OpenItemWheel()
    {
        GameObject iw = transform.Find("Player UI").Find("Item Wheel").gameObject;
        if (iw.activeSelf)
        {
            AdvanceItemWheel();
        }
        else
        {
            List<string> theItemList;
            int currItem = LoadItemWheelItems(out theItemList);
            if (theItemList.Count > 1)
            {
                iw.SetActive(true);
                iw.GetComponent<Animator>().Play("New State", 0, 0);
                iw.transform.Find("Middle").GetComponent<Image>().sprite = itemNameToIcon(theItemList[currItem]);
                iw.transform.Find("Bottom").GetComponent<Image>().sprite = itemNameToIcon(theItemList[(int)realModulo(currItem + 1, theItemList.Count)]);
                iw.transform.Find("Top").GetComponent<Image>().sprite = itemNameToIcon(theItemList[(int)realModulo(currItem - 1, theItemList.Count)]);
                iw.transform.Find("Name").GetComponent<Text>().text = theItemList[currItem];
            }
        }
    }

    void AdvanceItemWheel()
    {
        GameObject iw = transform.Find("Player UI").Find("Item Wheel").gameObject;
        if (itemWheelCooldown <= 0)
        {
            itemWheelCooldown = 0.5f;
            List<string> theItemList;
            int currItem = LoadItemWheelItems(out theItemList);
            if (theItemList.Count > 1)
            {
                AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                sc.clip = (AudioClip)Resources.Load("ItemWheel");
                sc.Play();
                iw.SetActive(true);
                currentItem = theItemList[(int)realModulo(currItem + 1, theItemList.Count)];
                iw.GetComponent<Animator>().Play("ItemWheel", 0, 0);
            }
        }
    }

    public void GatherItemWheelIcons()
    {
        List<string> theItemList;
        int currItem = LoadItemWheelItems(out theItemList);
        if (theItemList.Count > 1)
        {
            GameObject iw = transform.Find("Player UI").Find("Item Wheel").gameObject;
            iw.transform.Find("Middle").GetComponent<Image>().sprite = itemNameToIcon(theItemList[currItem]);
            iw.transform.Find("Bottom").GetComponent<Image>().sprite = itemNameToIcon(theItemList[(int)realModulo(currItem + 1, theItemList.Count)]);
            iw.transform.Find("Top").GetComponent<Image>().sprite = itemNameToIcon(theItemList[(int)realModulo(currItem - 1, theItemList.Count)]);
            iw.transform.Find("Name").GetComponent<Text>().text = theItemList[currItem];
        }
    }

    Sprite itemNameToIcon(string name)
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i].itemName == name)
            {
                return itemList[i].icon;
            }
        }
        return null;
    }

    int LoadItemWheelItems(out List<string> list)
    {
        list = new List<string>();
        int currItem = 0;
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i].unlockString == "")
            {
                list.Add(itemList[i].itemName);
            }
            else if (PlayerPrefs.GetInt(itemList[i].unlockString) == 1)
            {
                list.Add(itemList[i].itemName);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == currentItem)
            {
                currItem = i;
            }
        }
        return currItem;
    }

    public void FromUiToNormal(int tospeed)
    {
        playerState = Player.PlayerState.normal;
        SetFade(0, (byte)tospeed);
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        sc.clip = (AudioClip)Resources.Load("SystemClose");
        sc.Play();
    }

    public void SetFade(byte todarkness, byte tospeed)
    {
        fadeObj.fadeSpeed = tospeed;
        fadeObj.fadeTo = todarkness;
    }
}


[System.Serializable]
public class FootstepType
{
    public string meshName;
    public AudioClip[] clips;
    [Range(0.0f, 1.0f)]
    public float volume;
}

[System.Serializable]
public class CharacterItems
{
    public string itemName;
    public Sprite icon;
    public string unlockString;
}