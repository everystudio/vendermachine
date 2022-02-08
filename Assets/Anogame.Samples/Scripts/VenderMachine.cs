using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace anogame.sample
{
	public class VenderMachine : StateMachineBase<VenderMachine>
	{
		#region 共通部分
		private JuiceButton[] m_juiceButtonArr;
		private ChangeButton m_changeButton;
		public Text m_txtCoin;
		public int m_iCoinValue;

		private class UnityEventInt : UnityEvent<int> { }
		private UnityEventInt OnAddCoin = new UnityEventInt();

		private void showCoinText(int _iCoin)
		{
			m_txtCoin.text = _iCoin.ToString();
		}
		private void addCoin(int _iAdd)
		{
			m_iCoinValue += _iAdd;
			showCoinText(m_iCoinValue);
		}
		public void AddCoin(int _iAdd)
		{
			OnAddCoin.Invoke(_iAdd);
		}
		#endregion 共通部分

		private void Start()
		{
			m_iCoinValue = 0;
			showCoinText(0);
			m_juiceButtonArr = FindObjectsOfType<JuiceButton>();
			m_changeButton = FindObjectOfType<ChangeButton>();
			ChangeState(new VenderMachine.Neutral(this));
		}

		private class Neutral : StateBase<VenderMachine>
		{
			public Neutral(VenderMachine _machine) : base(_machine)
			{
			}
			public override void OnEnterState()
			{
				machine.OnAddCoin.AddListener((value) =>
				{
					machine.addCoin(value);
					machine.ChangeState(new VenderMachine.Selecting(machine));
				});
				foreach (JuiceButton button in machine.m_juiceButtonArr)
				{
					button.ShowJuice(button.model.price);
				}
				machine.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.75f);
			}
			public override void OnExitState()
			{
				machine.OnAddCoin.RemoveAllListeners();
				machine.GetComponent<SpriteRenderer>().color = Color.white;
			}
		}
		private class Selecting : StateBase<VenderMachine>
		{
			private float m_fTimer = 0f;
			public Selecting(VenderMachine _machine) : base(_machine)
			{
			}
			public override void OnEnterState()
			{
				machine.OnAddCoin.AddListener((value) =>
				{
					machine.addCoin(value);
					machine.ChangeState(new VenderMachine.Selecting(machine));
				});
				foreach (JuiceButton button in machine.m_juiceButtonArr)
				{
					bool bAbleBuy = button.model.price <= machine.m_iCoinValue;
					button.ShowJuice(machine.m_iCoinValue);
					if (bAbleBuy)
					{
						button.m_eventJuice.AddListener((value) =>
						{
							machine.ChangeState(new VenderMachine.Payout(machine, value));
						});
					}
				}
				machine.m_changeButton.m_eventPush.AddListener(() =>
				{
					machine.ChangeState(new VenderMachine.Changing(machine));
				});
			}
			public override void OnUpdate()
			{
				m_fTimer += Time.deltaTime;
				if (30f < m_fTimer)
				{
					machine.ChangeState(new VenderMachine.Changing(machine));
				}
			}
			public override void OnExitState()
			{
				machine.OnAddCoin.RemoveAllListeners();
				foreach (JuiceButton button in machine.m_juiceButtonArr)
				{
					button.m_eventJuice.RemoveAllListeners();
				}
				machine.m_changeButton.m_eventPush.RemoveAllListeners();
			}
		}
		private class Payout : StateBase<VenderMachine>
		{
			private JuiceModel m_juiceModel;
			private float m_fTimer;
			public Payout(VenderMachine _machine, JuiceModel _juice) : base(_machine)
			{
				m_fTimer = 0f;
				m_juiceModel = _juice;
			}
			public override void OnEnterState()
			{
				// 作る
				GameObject objJuice = new GameObject();
				objJuice.transform.position = new Vector3(Random.Range(-5f, 5f), 7f, 0f);
				SpriteRenderer sr = objJuice.AddComponent<SpriteRenderer>();
				sr.sprite = m_juiceModel.sprite;
				sr.sortingOrder = 15;
				objJuice.AddComponent<Rigidbody2D>();
				Destroy(objJuice, 5f);

				machine.addCoin(-1 * m_juiceModel.price);
			}
			public override void OnUpdate()
			{
				m_fTimer += Time.deltaTime;
				if (1f < m_fTimer)
				{
					if (0 < machine.m_iCoinValue)
					{
						machine.ChangeState(new VenderMachine.Selecting(machine));
					}
					else
					{
						machine.ChangeState(new VenderMachine.Neutral(machine));
					}
				}
			}
		}
		private class Changing : StateBase<VenderMachine>
		{
			private readonly int[] CoinValues = new int[4] { 500, 100, 50, 10 };
			private int m_iTempCoinValue;
			private float m_fTimer = 0f;

			public Changing(VenderMachine _machine) : base(_machine)
			{
			}
			public override void OnEnterState()
			{
				m_iTempCoinValue = machine.m_iCoinValue;
			}
			public override void OnUpdate()
			{
				m_fTimer += Time.deltaTime;
				if (1f < m_fTimer)
				{
					m_fTimer -= 1f;
					if (0 < m_iTempCoinValue)
					{
						for (int i = 0; i < CoinValues.Length; i++)
						{
							if (CoinValues[i] <= m_iTempCoinValue)
							{
								m_iTempCoinValue -= CoinValues[i];
								machine.showCoinText(m_iTempCoinValue);
								break;
							}
						}
					}
					else
					{
						machine.ChangeState(new VenderMachine.Neutral(machine));
					}
				}
			}
			public override void OnExitState()
			{
				machine.m_iCoinValue = 0;
			}

		}
	}
}