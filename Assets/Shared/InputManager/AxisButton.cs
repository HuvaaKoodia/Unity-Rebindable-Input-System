using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void AxisSelectedEvent(int index, int sign);

public class AxisButton : MonoBehaviour 
{
#region variables
public AxisSelectedEvent onSelectedEvent;
public Text text;
int index, sign;
#endregion
#region initialization
	public void Init(string text, int index, int sign)
	{
		this.index = index;
		this.sign = sign;
		this.text.text = text + (sign > 0 ? " +" : " -");
	}
#endregion
#region logic
	
#endregion
#region public interface
#endregion
#region private interface
#endregion
#region events
public void OnButtonPressed()
{
	onSelectedEvent(index, sign);
}
#endregion
}
