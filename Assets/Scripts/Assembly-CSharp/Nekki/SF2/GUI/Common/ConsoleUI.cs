using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Nekki.SF2.GUI.Common
{
	public class ConsoleUI : UIModule
	{
		[SerializeField]
		private RectTransform _Window;

		[SerializeField]
		private Text _Text;

		[SerializeField]
		private InputField _Input;

		[SerializeField]
		private Button _NextCommandButton;

		[SerializeField]
		private Button _PrevCommandButton;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static Action<bool> OnConsoleActive;

		private static ConsoleUI _Current;

		private static StringBuilder _OutputList = new StringBuilder();

		private static List<string> _CommandList = new List<string>();

		private int _LastCommandIndex = -1;

		private RectTransform DEFEGINEEKB;

		public static ConsoleUI BLOOLFFMKFI
		{
			get
			{
				return get_Current();
			}
		}

		public bool NJKPPJDCHPE
		{
			get
			{
				return get_IsWindowActive();
			}
		}

		public static event Action<bool> GKMLAKACCFO
		{
			add
			{
				add_OnConsoleActive(value);
			}
			remove
			{
				remove_OnConsoleActive(value);
			}
		}

		public static void add_OnConsoleActive(Action<bool> value)
		{
			Action<bool> action = OnConsoleActive;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnConsoleActive, (Action<bool>)Delegate.Combine(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static void remove_OnConsoleActive(Action<bool> value)
		{
			Action<bool> action = OnConsoleActive;
			Action<bool> action2;
			do
			{
				action2 = action;
				action = Interlocked.CompareExchange(ref OnConsoleActive, (Action<bool>)Delegate.Remove(action2, value), action);
			}
			while ((object)action != action2);
		}

		public static ConsoleUI get_Current()
		{
			return _Current;
		}

		public bool get_IsWindowActive()
		{
			return _Window.gameObject.activeSelf;
		}

		protected override void Init()
		{
			base.Init();
			_Current = this;
			_Text.text = _OutputList.ToString();
			_Window.gameObject.SetActive(false);
			DEFEGINEEKB = GetComponent<RectTransform>();
		}

		protected override void PJNFHNFLNNO()
		{
			base.PJNFHNFLNNO();
			_Current = null;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				OnToggleConsoleButton();
			}
			if (get_IsWindowActive() && _CommandList.Count > 0)
			{
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					SelectPrevCommand();
				}
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					SelectNextCommand();
				}
			}
		}

		public static void Log(string BFFNFGKHBJA)
		{
			if (_Current != null)
			{
				_Current.AddText(BFFNFGKHBJA);
			}
			else
			{
				_OutputList.AppendLine(BFFNFGKHBJA);
			}
		}

		public static void Clear()
		{
			_OutputList.Length = 0;
			if (_Current != null)
			{
				_Current._Text.text = string.Empty;
			}
		}

		public void OnSubmit()
		{
			if (!get_IsWindowActive())
			{
				return;
			}
			string text = _Input.text;
			string text2 = ConsoleDatabase.ExecuteCommand(text);
			if (!string.IsNullOrEmpty(text))
			{
				if (!_CommandList.Contains(text))
				{
					_CommandList.Add(text);
				}
				AddText(string.Format("<b>{0}</b>", text));
				LLLOJBFMONN.Write(text2);
				string[] array = text2.Split('\n');
				foreach (string nGEPNAJJHCD in array)
				{
					AddText(nGEPNAJJHCD);
				}
				_Input.text = string.Empty;
				_Input.ActivateInputField();
			}
		}

		public void OnChange()
		{
			_Input.text = _Input.text.Replace("`", string.Empty);
		}

		public void OnToggleConsoleButton()
		{
			Vector3 localPosition = DEFEGINEEKB.localPosition;
			if (!get_IsWindowActive())
			{
				Activate(true);
				DEFEGINEEKB.localPosition = localPosition - new Vector3(0f, _Window.rect.height, 0f);
				_Input.ActivateInputField();
			}
			else
			{
				Activate(false);
				DEFEGINEEKB.localPosition = localPosition + new Vector3(0f, _Window.rect.height, 0f);
				_Input.DeactivateInputField();
			}
		}

		protected void Activate(bool DMOGLMLJCDP)
		{
			_Window.gameObject.SetActive(DMOGLMLJCDP);
			if (NDHHFHHBFEC && OnConsoleActive != null)
			{
				OnConsoleActive(DMOGLMLJCDP);
			}
		}

		private void AddText(string NGEPNAJJHCD)
		{
			_OutputList.AppendLine(NGEPNAJJHCD);
			_Text.text = _OutputList.ToString();
		}

		public void SelectNextCommand()
		{
			_LastCommandIndex++;
			if (_LastCommandIndex >= _CommandList.Count)
			{
				_LastCommandIndex = 0;
			}
			_Input.text = _CommandList[_LastCommandIndex];
			_Input.caretPosition = _Input.text.Length;
		}

		public void SelectPrevCommand()
		{
			_LastCommandIndex--;
			if (_LastCommandIndex < 0)
			{
				_LastCommandIndex = _CommandList.Count - 1;
			}
			_Input.text = _CommandList[_LastCommandIndex];
			_Input.caretPosition = _Input.text.Length;
		}
	}
}
