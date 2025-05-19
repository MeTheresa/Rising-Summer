using UnityEngine;
using UnityEngine.InputSystem;

namespace SupanthaPaul
{
    public class PlayerInput
    {
        
        public KeyCode P1MoveLeftKey = KeyCode.A;
        public KeyCode P1MoveRightKey = KeyCode.D;
        public KeyCode P1JumpKey = KeyCode.W;
        public KeyCode P1PunchKey = KeyCode.F;


        public KeyCode P2MoveLeftKey = KeyCode.LeftArrow;
        public KeyCode P2MoveRightKey = KeyCode.RightArrow;
        public KeyCode P2JumpKey = KeyCode.UpArrow;
        public KeyCode P2PunchKey = KeyCode.RightControl;

        private Gamepad gamepadP1 => Gamepad.all.Count > 0 ? Gamepad.all[0] : null;
        private Gamepad gamepadP2 => Gamepad.all.Count > 1 ? Gamepad.all[1] : null;

        bool P1left;
        bool P1right;
        bool P2left;
        bool P2right;

        public float HorizontalRawP1()
        {


            if (Input.GetKey(P1MoveLeftKey))
            {
                Debug.Log("Left key pressed");
                P1right = false;
                P1left = true;
            }
            if (Input.GetKey(P1MoveRightKey))
            {
                Debug.Log("Right key pressed");
                P1right = true;
                P1left = false;
            }
            if (gamepadP1 != null)
            {
                if (gamepadP1.dpad.left.isPressed)
                {
                    Debug.Log("Left controller pressed");
                    P1left = true;
                    P1right = false;
                }
                if (gamepadP1.dpad.right.isPressed)
                {
                    Debug.Log("Right controller pressed");
                    P1right = true;
                    P1left = false;
                }
            }

            if (P1left && !P1right)
            {
                P1left = false; P1right = false;
                return -1f;
            }
            if (P1right && !P1left)
            {
                P1left = false; P1right = false;
                return 1f;
            }
            return 0f;
        }

        // Check if the player is pressing the jump key
        public bool JumpP1()
        {
            return Input.GetKeyDown(P1JumpKey) || (gamepadP1 != null && gamepadP1.buttonSouth.wasPressedThisFrame);
        }

        // Check if the player is pressing the punch key
        public bool PunchP1()
        {
            return Input.GetKeyDown(P1PunchKey) || (gamepadP1 != null && gamepadP1.buttonEast.wasPressedThisFrame);  // Return true if punch key is pressed
        }

        public float HorizontalRawP2()
        {
            if (Input.GetKey(P2MoveLeftKey))
            {
                Debug.Log("Left key pressed");
                P2right = false;
                P2left = true;
            }
            if (Input.GetKey(P2MoveRightKey))
            {
                Debug.Log("Right key pressed");
                P2right = true;
                P2left = false;
            }
            if (gamepadP2 != null)
            {
                if (gamepadP2.dpad.left.isPressed)
                {
                    Debug.Log("Left controller pressed");
                    P2left = true;
                    P2right = false;
                }
                if (gamepadP2.dpad.right.isPressed)
                {
                    Debug.Log("Right controller pressed");
                    P2right = true;
                    P2left = false;
                }
            }

            if (P2left && !P2right)
            {
                P2left = false; P2right = false;
                return -1f;
            }
            if (P2right && !P2left)
            {
                P2left = false; P2right = false;
                return 1f;
            }
            return 0f;
        }

        // Check if the player is pressing the jump key
        public bool JumpP2()
        {
            return Input.GetKeyDown(P2JumpKey) || (gamepadP2 != null && gamepadP2.buttonSouth.wasPressedThisFrame);
        }

        // Check if the player is pressing the punch key
        public bool PunchP2()
        {
            return Input.GetKeyDown(P2PunchKey) || (gamepadP2 != null && gamepadP2.buttonEast.wasPressedThisFrame);  // Return true if punch key is pressed
        }
    }
}
