using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputSystem : MonoSingleton<InputSystem>
{
    public KeyCode[] moveKeyArray = new KeyCode[4] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
}
