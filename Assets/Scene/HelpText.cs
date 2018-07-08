using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpText : MonoBehaviour
{
	#region variables
	public Text text;
	#endregion
	#region initialization
	void Start()
	{
		UpdateText();
	}

	#endregion
	#region logic
	#endregion
	#region public interface
	public void UpdateText()
	{
		text.text = string.Format("Press {0} to toggle the keybind menu.", InputManager.GetKeyBind("EscapeMenu").GetInputName());
	}
	#endregion
	#region private interface
	#endregion
	#region events
	#endregion
}