using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void keyBindElementEvent(KeyBindElement element);

public class KeyBindElement : MonoBehaviour
{
	#region variables
	public keyBindElementEvent onBind1ButtonPressed, onBind2ButtonPressed;

	public string visualName, keybindName;

	public Text nameText, bind1Text, bind2Text;
	public Button bind1Button, bind2Button;
	#endregion
	#region initialization
	private void Start()
	{
		nameText.text = visualName;

		Reset();

		bind1Button.onClick.AddListener(OnButton1Pressed);
		bind2Button.onClick.AddListener(OnButton2Pressed);
	}
	#endregion
	#region logic
	#endregion
	#region public interface

	public void SetBind1(KeyCode keyCode)
	{
		bind1Text.text = keyCode.ToString();
		InputManager.SetKeyBind1(keybindName, keyCode);

		if (keyCode == KeyCode.None)
			InputManager.SetKeyBind1(keybindName, "", 0);
	}

	public void SetBind2(KeyCode keyCode)
	{
		bind2Text.text = keyCode.ToString();
		InputManager.SetKeyBind2(keybindName, keyCode);

		if (keyCode == KeyCode.None)
			InputManager.SetKeyBind2(keybindName, "", 0);
	}

	public void SetBind1(string axisName, int sign)
	{
		bind1Text.text = GetAxisFormat(axisName, sign);
		InputManager.SetKeyBind1(keybindName, axisName, sign);
	}

    public void SetBind2(string axisName, int sign)
	{
		bind2Text.text = GetAxisFormat(axisName, sign);
		InputManager.SetKeyBind2(keybindName, axisName, sign);
	}

	public void Reset()
	{
		var keyBind = InputManager.GetKeyBind(keybindName);
		bind1Text.text = keyBind.keyCode1.ToString();
		if (keyBind.keyCode1 == KeyCode.None)
			bind1Text.text = GetAxisFormat(keyBind.axisName1, keyBind.axisSign1);

		bind2Text.text = keyBind.keyCode2.ToString();
		if (keyBind.keyCode2 == KeyCode.None && !string.IsNullOrEmpty(keyBind.axisName2))
			bind2Text.text = GetAxisFormat(keyBind.axisName2, keyBind.axisSign2);
	}

	#endregion
	#region private interface

    private string GetAxisFormat(string axisName, int sign)
    {
        return axisName + " " + (sign > 0 ? "+" : "-");
    }
	
	#endregion
	#region events
	void OnButton1Pressed()
	{
		onBind1ButtonPressed(this);
	}

	void OnButton2Pressed()
	{
		onBind2ButtonPressed(this);
	}

    public bool HasNoBind()
    {
		var bind = InputManager.GetKeyBind(keybindName);
        return bind.IsEmpty();
    }

    #endregion
}
