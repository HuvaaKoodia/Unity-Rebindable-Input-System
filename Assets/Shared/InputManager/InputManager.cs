using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// An input manager which supports key rebinding.
/// </summary>
public class InputManager : MonoBehaviour
{
    #region variables
    [System.Serializable]
    public class InputClass
    {
        public List<KeyBind> keybinds = new List<KeyBind>();

        internal InputClass Clone()
        {
            var clone = new InputClass();

            foreach (var bind in keybinds)
            {
                var newBind = bind.Copy();
                clone.keybinds.Add(newBind);
            }

            return clone;
        }
    }

    [System.Serializable]
    public class KeyBind
    {
        public string name, visualName;
        public KeyCode keyCode1, keyCode2;
        public string axisName1, axisName2;
        public int axisSign1, axisSign2;
        [NonSerialized]
        public int oldAxisValue1, oldAxisValue2;
        [NonSerialized]
        public int group;

        public KeyBind(KeyCode keyCode1, string name, string visualName, int group = -1)
        {
            this.keyCode1 = keyCode1;

            this.name = name;
            this.visualName = visualName;
            this.group = group;
        }

        public KeyBind(KeyCode keyCode1, KeyCode keyCode2, string name, string visualName, int group = -1): this(keyCode1, name, visualName, group)
        {
            this.keyCode2 = keyCode2;
        }

        public KeyBind(KeyCode keyCode1, KeyCode keyCode2, string axisName2, int axisSign2, string name, string visualName, int group = -1): this(keyCode1, name, visualName, group)
        {
            this.axisName2 = axisName2;
            this.axisSign2 = axisSign2;
        }

        public KeyBind(string axisName1, int axisSign1, string name, string visualName, int group = -1)
        {
            this.axisName1 = axisName1;
            this.axisSign1 = axisSign1;

            this.name = name;
            this.visualName = visualName;
            this.group = group;
        }

        public KeyBind(string axisName1, int axisSign1, string axisName2, int axisSign2, string name, string visualName, int group = -1): this(axisName1, axisSign1, name, visualName, group)
        {
            this.axisName2 = axisName2;
            this.axisSign2 = axisSign2;
        }

        public KeyBind(string axisName1, int axisSign1, KeyCode keyCode2, string name, string visualName, int group = -1): this(axisName1, axisSign1, name, visualName, group)
        {
            this.keyCode2 = keyCode2;
        }

        public bool Same(KeyBind keyBind)
        {
            return name == keyBind.name;
        }

        public bool IsEmpty()
        {
            return keyCode1 == KeyCode.None && keyCode2 == KeyCode.None && string.IsNullOrEmpty(axisName1)&& string.IsNullOrEmpty(axisName2);
        }

        public void Reset(KeyBind keyBind)
        {
            keyCode1 = keyBind.keyCode1;
            keyCode2 = keyBind.keyCode2;
            axisName1 = keyBind.axisName1;
            axisSign1 = keyBind.axisSign1;
            axisName2 = keyBind.axisName2;
            axisSign2 = keyBind.axisSign2;
        }

        public KeyBind Copy()
        {
            return (KeyBind)MemberwiseClone();
        }

        public string GetInputName()
        {
            if (keyCode1 != KeyCode.None)
                return keyCode1.ToString();
            if (keyCode2 != KeyCode.None)
                return keyCode2.ToString();
            if (axisSign1 != 0)
                return axisName1;
            if (axisSign2 != 0)
                return axisName2;
            return "None";
        }
    }

    public class AxisData
    {
        public string name;
        public bool showAsButton;

        public AxisData(string name, bool showAsButton = false)
        {
            this.name = name;
            this.showAsButton = showAsButton;
        }
    }

    public static List<KeyBind> keybindsRead { get { return input.keybinds; } }
    public static List<AxisData> allowedAxes = new List<AxisData>();

    static InputClass input;
    static Dictionary<string, KeyBind> keybindsTable;
    const string fileName = "Keybinds.ini";
    static bool initDone = false;
    static InputClass defaultInput;

    #endregion
    #region initialization
    /// <summary>
    /// Call this before using any of the input functions.
    /// </summary>
    public static void Init()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return;
#endif
        if (initDone)return;
        initDone = true;

        //Add all keybinds here.
        defaultInput = new InputClass();

        defaultInput.keybinds.Add(new KeyBind(KeyCode.W, "MoveUp", "Walk up", 0));
        defaultInput.keybinds.Add(new KeyBind(KeyCode.S, "MoveDown", "Walk down", 0));
        defaultInput.keybinds.Add(new KeyBind(KeyCode.D, "MoveRight", "Walk right", 0));
        defaultInput.keybinds.Add(new KeyBind(KeyCode.A, "MoveLeft", "Walk left", 0));

        defaultInput.keybinds.Add(new KeyBind("Mouse wheel", 1, "ChangeColorUp", "Change color up", 2));
        defaultInput.keybinds.Add(new KeyBind("Mouse wheel", -1, "ChangeColorDown", "Change color down", 2));

        defaultInput.keybinds.Add(new KeyBind(KeyCode.Escape, "EscapeMenu", "Escape menu Toggle"));

        //Add all allowed input axes here. Other axes cannot be rebinded to.
        allowedAxes.Add(new AxisData("Mouse wheel"));
        allowedAxes.Add(new AxisData("Mouse X", true));
        allowedAxes.Add(new AxisData("Mouse Y", true));
        allowedAxes.Add(new AxisData("JoyAxis 1"));
        allowedAxes.Add(new AxisData("JoyAxis 2"));
        allowedAxes.Add(new AxisData("JoyAxis 3"));
        allowedAxes.Add(new AxisData("JoyAxis 4"));
        allowedAxes.Add(new AxisData("JoyAxis 5"));
        allowedAxes.Add(new AxisData("JoyAxis 6"));
        allowedAxes.Add(new AxisData("JoyAxis 7"));
        allowedAxes.Add(new AxisData("JoyAxis 8"));
        allowedAxes.Add(new AxisData("JoyAxis 9"));
        allowedAxes.Add(new AxisData("JoyAxis 10"));

        //Find and read saved input
        input = new InputClass();

#if UNITY_STANDALONE
        if (File.Exists(fileName))
        {
            var fileText = File.ReadAllText(fileName);
            input = JsonUtility.FromJson<InputClass>(fileText);
        }
#else
        if (PlayerPrefs.HasKey(fileName))
        {
            var fileText = PlayerPrefs.GetString(fileName);
            input = JsonUtility.FromJson<InputClass>(fileText);
        }
#endif

        bool createDefault = true;
        if (input.keybinds.Count == defaultInput.keybinds.Count)
        { //The saved settings are overwritten if default settings have changed
            for (int i = 0; i < input.keybinds.Count; i++)
            {
                if (input.keybinds[i].Same(defaultInput.keybinds[i]))
                {
                    input.keybinds[i].group = defaultInput.keybinds[i].group;
                    createDefault = false;
                }
                else
                {
                    createDefault = true;
                    break;
                }
            }
        }

        if (createDefault)
        {
            input = defaultInput.Clone();
            SaveSettings();
        }

        //Init runtime data
        keybindsTable = new Dictionary<string, KeyBind>();

        foreach (var item in input.keybinds)
        {
            keybindsTable.Add(item.name, item);
        }
    }

    #endregion
    #region logic
    #endregion
    #region public interface
    public static bool GetButtonDown(string button)
    {
        var keybind = GetKeyBind(button);

        if (keybind.axisSign1 != 0 && keybind.oldAxisValue1 != Input.GetAxisRaw(keybind.axisName1)&& Math.Abs(Input.GetAxisRaw(keybind.axisName1)- keybind.axisSign1)< 1)
            return true;
        if (keybind.axisSign2 != 0 && keybind.oldAxisValue2 != Input.GetAxisRaw(keybind.axisName2)&& Math.Abs(Input.GetAxisRaw(keybind.axisName2)- keybind.axisSign2)< 1)
            return true;

        return Input.GetKeyDown(keybind.keyCode1)|| Input.GetKeyDown(keybind.keyCode2);
    }

    public static bool GetButtonUp(string button)
    {
        var keybind = GetKeyBind(button);

        if (keybind.axisSign1 != 0 && keybind.oldAxisValue1 != Input.GetAxisRaw(keybind.axisName1)&& Math.Abs(Input.GetAxisRaw(keybind.axisName1)- keybind.axisSign1)>= 1)
            return true;
        if (keybind.axisSign2 != 0 && keybind.oldAxisValue2 != Input.GetAxisRaw(keybind.axisName2)&& Math.Abs(Input.GetAxisRaw(keybind.axisName2)- keybind.axisSign2)>= 1)
            return true;

        return Input.GetKeyUp(keybind.keyCode1)|| Input.GetKeyUp(keybind.keyCode2);
    }

    public static bool GetButton(string button)
    {
        var keybind = GetKeyBind(button);

        if (keybind.axisSign1 != 0 && Math.Abs(Input.GetAxisRaw(keybind.axisName1)- keybind.axisSign1)< 1)
            return true;
        if (keybind.axisSign2 != 0 && Math.Abs(Input.GetAxisRaw(keybind.axisName2)- keybind.axisSign2)< 1)
            return true;

        return Input.GetKey(keybind.keyCode1)|| Input.GetKey(keybind.keyCode2);
    }

    public static bool GetAxis(string axis, out float value)
    {
        var keybind = GetKeyBind(axis);

        if (keybind.axisSign1 != 0 && Math.Abs(Mathf.Clamp(Input.GetAxis(keybind.axisName1), -1, 1)+ keybind.axisSign1)> 1)
        {
            value = Mathf.Abs(Input.GetAxis(keybind.axisName1));
            return true;
        }

        if (keybind.axisSign2 != 0 && Math.Abs(Mathf.Clamp(Input.GetAxis(keybind.axisName2), -1, 1)+ keybind.axisSign2)> 1)
        {
            value = Mathf.Abs(Input.GetAxis(keybind.axisName2));
            return true;
        }

        if (Input.GetKey(keybind.keyCode1)|| Input.GetKey(keybind.keyCode2))
        {
            value = 1;
            return true;
        }

        value = 0;
        return false;
    }

    public static void SetKeyBind1(string name, KeyCode keyCode)
    {
        var keyBind = GetKeyBind(name);
        keyBind.keyCode1 = keyCode;

        SaveSettings();
    }

    public static void SetKeyBind2(string name, KeyCode keyCode)
    {
        var keyBind = GetKeyBind(name);
        keyBind.keyCode2 = keyCode;

        SaveSettings();
    }

    public static void SetKeyBind1(string name, string axisName, int sign)
    {
        var keyBind = GetKeyBind(name);
        keyBind.axisName1 = axisName;
        keyBind.axisSign1 = sign;

        SaveSettings();
    }

    public static void SetKeyBind2(string name, string axisName, int sign)
    {
        var keyBind = GetKeyBind(name);
        keyBind.axisName2 = axisName;
        keyBind.axisSign2 = sign;

        SaveSettings();
    }

    public static KeyBind GetKeyBind(string name)
    {
#if UNITY_EDITOR
        if (!keybindsTable.ContainsKey(name))
        {
            Debug.LogErrorFormat("{0} is not a valid keybind name.", name);
            return null;
        }
#endif
        return keybindsTable[name];
    }

    public static void ResetKeybinds()
    {
        for (int i = 0; i < defaultInput.keybinds.Count; i++)
        {
            input.keybinds[i].Reset(defaultInput.keybinds[i]);
        }
        SaveSettings();
    }
    #endregion
    #region private interface
    static void SaveSettings()
    {
#if ANDROID
        return;
#elif UNITY_STANDALONE
        var serialized = JsonUtility.ToJson(input, true);
        File.WriteAllText(fileName, serialized);
#else
        var serialized = JsonUtility.ToJson(input, false);
        PlayerPrefs.SetString(fileName, serialized);
#endif  
    }
    #endregion
    #region events
    #endregion
}