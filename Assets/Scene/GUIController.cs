using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class GUIController : MonoBehaviour
{
#region variables
	public static GUIController I;

	public KeyBindMenu keybindMenu;
	public GameObject keybindPanel;
	public UnityEvent onMenuToggled;
#endregion
#region initialization
	private void Awake()
	{
		I = this;
		keybindMenu.gameObject.SetActive(false);
	}
#endregion
#region logic
#endregion
#region public interface
void Update()
{
	if (!keybindPanel.activeSelf && InputManager.GetButtonDown("EscapeMenu"))
	{
		keybindMenu.gameObject.SetActive(!keybindMenu.gameObject.activeSelf);
		onMenuToggled.Invoke();
	}
}
#endregion
#region private interface
#endregion
#region events
#endregion
}
