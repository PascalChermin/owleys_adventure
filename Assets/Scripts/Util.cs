using System;
using UnityEngine;

namespace AssemblyCSharp.Assets.Scripts
{
    public struct Axis
    {
        public static readonly string Horizontal = "Horizontal";
        public static readonly string Vertical = "Vertical";
    }

    public enum MovementDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    [Serializable]
    public enum SceneNumbers
    {
        Quit = -1,
        Menu_Main = 0,
        Menu_GameOver = 1,
        Level_Secret = 2,
        Level_1 = 3,
        Level_2 = 4,
        Level_3 = 5,
        Level_4 = 6,
        Level_5 = 7,
        Level_6 = 8,
        Level_7 = 9,
        Level_8 = 10,
        Level_9 = 11,
        Level_10 = 12
    }

    public struct Layers
    {
        public static readonly string Ground = "Ground";
        public static readonly string Ladder = "Ladder";
        public static readonly string Player = "Player";
        public static readonly string Enemy = "Enemy";
        public static readonly string Doors = "Doors";
    }

    public sealed class ButtonMapping
    {
        // Keyboard Defaults
        private static KeyCode defaultKeyboardLeft = KeyCode.LeftArrow;
        private static KeyCode defaultKeyboardRight = KeyCode.RightArrow;
        private static KeyCode defaultKeyboardUp = KeyCode.UpArrow;
        private static KeyCode defaultKeyboardDown = KeyCode.DownArrow;
        private static KeyCode defaultKeyboardJump = KeyCode.Space;
        private static KeyCode defaultKeyboardAttack = KeyCode.LeftControl;

        // Controller Defaults
        private static string defaultControllerJump = "Jump";
        private static string defaultControllerAttack = "Fire1";

        // Singleton instance
        private static ButtonMapping _instance = new ButtonMapping();

        public KeyCode KeyboardLeft { get; set; }
        public KeyCode KeyboardRight { get; set; }
        public KeyCode KeyboardUp { get; set; }
        public KeyCode KeyboardDown { get; set; }
        public KeyCode KeyboardJump { get; set; }
        public KeyCode KeyboardAttack { get; set; }
        public string ControllerJump { get; set; }
        public string ControllerAttack { get; set; }

        private ButtonMapping()
        {
            // Set Keyboard defaults
            KeyboardLeft = defaultKeyboardLeft;
            KeyboardRight = defaultKeyboardRight;
            KeyboardUp = defaultKeyboardUp;
            KeyboardDown = defaultKeyboardDown;
            KeyboardJump = defaultKeyboardJump;
            KeyboardAttack = defaultKeyboardAttack;

            // Set Controller defaults
            ControllerJump = defaultControllerJump;
            ControllerAttack = defaultControllerAttack;
        }

        public static ButtonMapping GetInstance() => _instance;
    }

    public struct Animations
    {
        public static readonly string Idle = "Idle";
        public static readonly string Running = "Running";
        public static readonly string Jumping = "Jumping";
        public static readonly string Gliding = "Gliding";
        public static readonly string Climbing = "Climbing";
        public static readonly string Death = "Death";
    }

    public class UtilityHelper
    {
        public static SceneNumbers previousScene = SceneNumbers.Menu_Main;
    }
}
