using UnityEngine;

namespace SupanthaPaul
{
    public class PlayerInput
    {
        public KeyCode moveLeftKey = KeyCode.A;
        public KeyCode moveRightKey = KeyCode.D;
        public KeyCode jumpKey = KeyCode.W;
        public KeyCode punchKey = KeyCode.F;  // Key for punching

        // Get horizontal movement input
        public float HorizontalRaw()
        {
            bool left = Input.GetKey(moveLeftKey);
            bool right = Input.GetKey(moveRightKey);

            if (left && !right) return -1f;
            if (right && !left) return 1f;
            return 0f;
        }

        // Check if the player is pressing the jump key
        public bool Jump()
        {
            return Input.GetKeyDown(jumpKey);
        }

        // Check if the player is pressing the punch key
        public bool Punch()
        {
            return Input.GetKeyDown(punchKey);  // Return true if punch key is pressed
        }
    }
}
