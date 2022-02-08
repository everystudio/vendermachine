using UnityEngine;
using UnityEngine.Events;

namespace anogame.sample
{
	public class ChangeButton : MonoBehaviour
	{
		public UnityEvent m_eventPush = new UnityEvent();
		public void OnClick()
		{
			Debug.Log("ChangeButton.Clicked");
			m_eventPush.Invoke();
		}
	}
}
