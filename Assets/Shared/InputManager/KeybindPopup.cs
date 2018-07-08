using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeybindPopup : MonoBehaviour
{
	#region variables
	public GameObject panel;
	public Text text;
	public Transform axisButtonParent;
	public AxisButton axisButtonPrefab;
	bool checkForKeyUp, isBind1;
	KeyBindElement element;
	#endregion
	#region initialization

	public void Start()
	{
		panel.SetActive(false);

		for (int i = 0; i < InputManager.allowedAxes.Count; i++)
		{
			if (InputManager.allowedAxes[i].showAsButton)
			{
				{
					var button = Instantiate(axisButtonPrefab, axisButtonParent)as AxisButton;
					button.Init(InputManager.allowedAxes[i].name, i, 1);
					button.onSelectedEvent += OnAxisButtonSelected;
					button.gameObject.SetActive(true);
				}
				{
					var button = Instantiate(axisButtonPrefab, axisButtonParent)as AxisButton;
					button.Init(InputManager.allowedAxes[i].name, i, -1);
					button.onSelectedEvent += OnAxisButtonSelected;
					button.gameObject.SetActive(true);
				}
			}
		}

		axisButtonParent.gameObject.SetActive(axisButtonParent.childCount > 2);
	}

	#endregion
	#region logic
	void Update()
	{
		if (panel.activeSelf)
		{
			if (Input.anyKey)
			{
				checkForKeyUp = true;
			}
			else if (checkForKeyUp)
			{
				int maxKeyCode = (int)KeyCode.Joystick8Button19;
				for (int i = 0; i < maxKeyCode; i++)
				{
					var keyCode = (KeyCode)i;
					if (Input.GetKeyUp(keyCode))
					{
						Close(keyCode);
						return;
					}
				}
			}

			for (int i = 0; i < InputManager.allowedAxes.Count; i++)
			{
				if (!InputManager.allowedAxes[i].showAsButton)
				{
					var value = Input.GetAxisRaw(InputManager.allowedAxes[i].name);
					if (value != 0)
					{
						Close(InputManager.allowedAxes[i].name, (int)Mathf.Sign(value));
						return;
					}
				}
			}
		}
	}
	#endregion
	#region public interface
	public void Open(KeyBindElement element, bool isBind1)
	{
		this.element = element;
		this.isBind1 = isBind1;
		panel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(null);
		checkForKeyUp = false;
		text.text = string.Format(element.visualName);
	}
	#endregion
	#region private interface
	public void ClearBind()
	{
		Close(KeyCode.None);
	}
	private void Close(KeyCode keyCode)
	{
		panel.SetActive(false);
		if (isBind1)
			element.SetBind1(keyCode);
		else
			element.SetBind2(keyCode);
	}

	private void Close(string axis, int sign)
	{
		panel.SetActive(false);
		if (isBind1)
			element.SetBind1(axis, sign);
		else
			element.SetBind2(axis, sign);
	}
	#endregion
	#region events

	private void OnAxisButtonSelected(int index, int sign)
	{
		checkForKeyUp = false;
		Close(InputManager.allowedAxes[index].name, sign);
	}

	#endregion
}