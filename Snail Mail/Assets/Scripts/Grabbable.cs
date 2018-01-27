using UnityEngine;

public class Grabbable : MonoBehaviour
{
	public delegate void ThingGrabbedAction(Grabbable thing);
	public static event ThingGrabbedAction ThingGrabbedEvent;

	void OnMouseDown()
	{
		if (ThingGrabbedEvent != null)
		{
			ThingGrabbedEvent(this);
		}
	}
}
