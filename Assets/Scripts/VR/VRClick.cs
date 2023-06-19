using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRClick : MonoBehaviour
{
    public LayerMask uiLayerMask;
    public Player player;
    public bool isLeftHand;
    XRIDefaultInputActions controls;
    void Awake()
    {
        if (controls == null)
        {
            controls = new XRIDefaultInputActions();
        }
        controls.XRILeftHand.Enable();
        controls.XRIRightHand.Enable();
    }

    void OnDisable()
    {
        controls.XRILeftHand.Disable();
        controls.XRIRightHand.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, 10f, uiLayerMask))
        {
            if (hit.collider.GetComponent<Button3D>())
            {
                Button3D hitcol = hit.collider.GetComponent<Button3D>();
                hitcol.Highlight(gameObject.name);
                switch (mouseCheck())
                {
                    case true:
                        hitcol.StartClick("Player");
                        break;
                    case false:
                        hitcol.EndClick("Player");
                        break;
                }

            }
        }
        if(!isLeftHand)
        {
            if(controls.XRIRightHand.B.IsPressed())
            {
                player.MovePlayer(controls.XRILeftHand.Move.ReadValue<Vector2>(), true);
            }
            else
            {
                player.MovePlayer(controls.XRILeftHand.Move.ReadValue<Vector2>(), false);
            }
            if (controls.XRIRightHand.A.IsPressed())
            {
                player.JumpBool = 1;
            }
            else
            {
                player.JumpBool = 0;
            }
        }
        if (isLeftHand)
        {
            if (controls.XRILeftHand.A.IsPressed())
            {
                if(player.flashState == 0)
                {
                    player.flashState = 1;
                }
                else if(player.flashState == 1)
                {
                    player.flashState = 2;
                }
            }
            else
            {
                if (player.flashState == 2)
                {
                    player.flashState = 3;
                }
                else if (player.flashState == 3)
                {
                    player.flashState = 0;
                }
            }
        }
}

    bool mouseCheck()
    {
        if(isLeftHand)
        {
            if (controls.XRILeftHand.UIPress.IsPressed())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (controls.XRIRightHand.UIPress.IsPressed())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
