using UnityEngine;
using UnityEngine.Events;

namespace anogame.sample
{
	public class UnityEventJuice : UnityEvent<JuiceModel> { }

	public class JuiceButton : MonoBehaviour
	{
		[SerializeField]
		private JuiceModel m_juiceModel;
		public JuiceModel model { get { return m_juiceModel; } }
		public UnityEventJuice m_eventJuice = new UnityEventJuice();
		public void OnClick()
		{
			//Debug.Log(m_juiceModel.juice_name);
			m_eventJuice.Invoke(m_juiceModel);
		}
		private void Start()
		{
			GetComponent<SpriteRenderer>().sprite = m_juiceModel.sprite;
		}
		public void ShowJuice(int _iCoin)
		{
			if (model.price <= _iCoin)
			{
				GetComponent<SpriteRenderer>().color = Color.white;
			}
			else
			{
				GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
			}
		}
	}
}
