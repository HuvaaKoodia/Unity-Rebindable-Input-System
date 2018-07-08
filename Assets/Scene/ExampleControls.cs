using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleControls : MonoBehaviour
{
	#region variables
	public new Renderer renderer;
	public Color[] colors;
	int colorIndex;
	#endregion
	#region initialization
	void Start()
	{
		UpdateColor(0);
	}
	#endregion
	#region logic
	void Update()
	{
		float vertical, horizontal;

		if (!InputManager.GetAxis("MoveUp", out vertical))
		{
			if (InputManager.GetAxis("MoveDown", out vertical))
				vertical = -vertical;
		}

		if (!InputManager.GetAxis("MoveRight", out horizontal))
		{
			if (InputManager.GetAxis("MoveLeft", out horizontal))
			{
				horizontal = -1;
			}
		}

		transform.Translate(new Vector3(horizontal * Time.deltaTime, 0, vertical * Time.deltaTime));

		if (InputManager.GetButtonDown("ChangeColorUp"))
		{
			UpdateColor(1);
		}

		if (InputManager.GetButtonDown("ChangeColorDown"))
		{
			UpdateColor(-1);
		}
	}

	private void UpdateColor(int add)
	{
		colorIndex += add;
		if (colorIndex < 0)
			colorIndex = colors.Length - 1;
		if (colorIndex == colors.Length)
			colorIndex = 0;

		renderer.material.color = colors[colorIndex];
	}
	#endregion
	#region public interface
	public void ToggleEnabled()
	{
		enabled = !enabled;
	}
	#endregion
	#region private interface
	#endregion
	#region events
	#endregion
}