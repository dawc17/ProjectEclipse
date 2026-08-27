using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

public class Hub : IHub
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Connection HBIPDLLOGKF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string HKGHEJDKCPI;

	private Dictionary<string, object> state;

	[CompilerGenerated]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private OnMethodCallDelegate OnMethodCall;

	private Dictionary<ulong, ClientMessage> CLOFJFHDFOM = new Dictionary<ulong, ClientMessage>();

	private Dictionary<string, JAHNDKHBMPG> ANINDNCGFLN = new Dictionary<string, JAHNDKHBMPG>();

	private StringBuilder builder = new StringBuilder();

	public Connection BAFGHLCPPHM
	{
		get
		{
			return HBIPDLLOGKF;
		}
		set
		{
			HBIPDLLOGKF = value;
		}
	}

	public string MENAJEAJJBE
	{
		get
		{
			return get_Name();
		}
		private set
		{
			set_Name(value);
		}
	}

	public Dictionary<string, object> AFINHOBCHMC
	{
		get
		{
			return FLBBFDNHJAJ();
		}
	}

	public event OnMethodCallDelegate CPILPJBJMKN
	{
		add
		{
			OPHFDPDINKG(value);
		}
		remove
		{
			GHANJIPIGMG(value);
		}
	}

	public Hub(string name)
		: this(name, null)
	{
	}

	public Hub(string name, Connection BJGMPDIKEJC)
	{
		set_Name(name);
		((IHub)this).GNLCPJFBAJE(BJGMPDIKEJC);
	}

	public string get_Name()
	{
		return HKGHEJDKCPI;
	}

	private void set_Name(string value)
	{
		HKGHEJDKCPI = value;
	}

	public Dictionary<string, object> FLBBFDNHJAJ()
	{
		if (state == null)
		{
			state = new Dictionary<string, object>();
		}
		return state;
	}

	public void OPHFDPDINKG(OnMethodCallDelegate value)
	{
		OnMethodCallDelegate kOBOMHLOBON = OnMethodCall;
		OnMethodCallDelegate kOBOMHLOBON2;
		do
		{
			kOBOMHLOBON2 = kOBOMHLOBON;
			kOBOMHLOBON = Interlocked.CompareExchange(ref OnMethodCall, (OnMethodCallDelegate)Delegate.Combine(kOBOMHLOBON2, value), kOBOMHLOBON);
		}
		while ((object)kOBOMHLOBON != kOBOMHLOBON2);
	}

	public void GHANJIPIGMG(OnMethodCallDelegate value)
	{
		OnMethodCallDelegate kOBOMHLOBON = OnMethodCall;
		OnMethodCallDelegate kOBOMHLOBON2;
		do
		{
			kOBOMHLOBON2 = kOBOMHLOBON;
			kOBOMHLOBON = Interlocked.CompareExchange(ref OnMethodCall, (OnMethodCallDelegate)Delegate.Remove(kOBOMHLOBON2, value), kOBOMHLOBON);
		}
		while ((object)kOBOMHLOBON != kOBOMHLOBON2);
	}

	Connection IHub.PEBFDIFIMBO
	{
		get
		{
			return HBIPDLLOGKF;
		}
		set
		{
			HBIPDLLOGKF = value;
		}
	}

	void IHub.GNLCPJFBAJE(Connection value)
	{
		HBIPDLLOGKF = value;
	}

	public void JPJAFMLNALO(string FJLOLCPJACB, JAHNDKHBMPG callback)
	{
		ANINDNCGFLN[FJLOLCPJACB] = callback;
	}

	public void Off(string FJLOLCPJACB)
	{
		ANINDNCGFLN[FJLOLCPJACB] = null;
	}

	public void Call(string FJLOLCPJACB, params object[] LKIOKGCNKHE)
	{
		Call(FJLOLCPJACB, null, null, null, LKIOKGCNKHE);
	}

	public void Call(string FJLOLCPJACB, FEENMMBNDJA KGLHKHHFNOO, params object[] LKIOKGCNKHE)
	{
		Call(FJLOLCPJACB, KGLHKHHFNOO, null, null, LKIOKGCNKHE);
	}

	public void Call(string FJLOLCPJACB, FEENMMBNDJA KGLHKHHFNOO, FGNDEBGHBMC PLEIBDIHIFO, params object[] LKIOKGCNKHE)
	{
		Call(FJLOLCPJACB, KGLHKHHFNOO, PLEIBDIHIFO, null, LKIOKGCNKHE);
	}

	public void Call(string FJLOLCPJACB, FEENMMBNDJA KGLHKHHFNOO, NNBBLIKMEDJ LFAIENNBBMK, params object[] LKIOKGCNKHE)
	{
		Call(FJLOLCPJACB, KGLHKHHFNOO, null, LFAIENNBBMK, LKIOKGCNKHE);
	}

	public void Call(string FJLOLCPJACB, FEENMMBNDJA KGLHKHHFNOO, FGNDEBGHBMC PLEIBDIHIFO, NNBBLIKMEDJ LFAIENNBBMK, params object[] LKIOKGCNKHE)
	{
		lock (((IHub)this).BAFGHLCPPHM.SyncRoot)
		{
			Connection hDMLLEEKKLF = ((IHub)this).BAFGHLCPPHM;
			hDMLLEEKKLF.set_ClientMessageCounter(hDMLLEEKKLF.FOIDELLGGOL() % ulong.MaxValue);
			Connection hDMLLEEKKLF2 = ((IHub)this).BAFGHLCPPHM;
			ulong kKAADAAPLDC;
			hDMLLEEKKLF2.set_ClientMessageCounter((kKAADAAPLDC = hDMLLEEKKLF2.FOIDELLGGOL()) + 1);
			((IHub)this).Call(new ClientMessage(this, FJLOLCPJACB, LKIOKGCNKHE, kKAADAAPLDC, KGLHKHHFNOO, PLEIBDIHIFO, LFAIENNBBMK));
		}
	}

	void IHub.Call(ClientMessage CKEHOEGLMBM)
	{
		lock (((IHub)this).BAFGHLCPPHM.SyncRoot)
		{
			CLOFJFHDFOM.Add(CKEHOEGLMBM.CallIdx, CKEHOEGLMBM);
			((IHub)this).BAFGHLCPPHM.CJDGGCJDHIE(PDGGBABAPJF(CKEHOEGLMBM));
		}
	}

	bool IHub.HasSentMessageId(ulong OKNNNLIPODI)
	{
		return CLOFJFHDFOM.ContainsKey(OKNNNLIPODI);
	}

	void IHub.Close()
	{
		CLOFJFHDFOM.Clear();
	}

	void IHub.OnMethod(MethodCallMessage CKEHOEGLMBM)
	{
		MergeState(CKEHOEGLMBM.FLBBFDNHJAJ());
		if (OnMethodCall != null)
		{
			try
			{
				OnMethodCall(this, CKEHOEGLMBM.OIPIMPLLDCP(), CKEHOEGLMBM.FNKPHEHFKEI());
			}
			catch (Exception mPFFFAOGBJE)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("Hub - " + get_Name(), "IHub.OnMethod - OnMethodCall", mPFFFAOGBJE);
			}
		}
		JAHNDKHBMPG value;
		if (ANINDNCGFLN.TryGetValue(CKEHOEGLMBM.OIPIMPLLDCP(), out value) && value != null)
		{
			try
			{
				value(this, CKEHOEGLMBM);
				return;
			}
			catch (Exception mPFFFAOGBJE2)
			{
				HTTPManager.MBBMPNDDPIH().COHEDILAHFD("Hub - " + get_Name(), "IHub.OnMethod - callback", mPFFFAOGBJE2);
				return;
			}
		}
		HTTPManager.MBBMPNDDPIH().KDAFBLAKBMI("Hub - " + get_Name(), string.Format("[Client] {0}.{1} (args: {2})", get_Name(), CKEHOEGLMBM.OIPIMPLLDCP(), CKEHOEGLMBM.FNKPHEHFKEI().Length));
	}

	void IHub.OnMessage(IServerMessage CKEHOEGLMBM)
	{
		ulong key = (CKEHOEGLMBM as IHubMessage).HGFDDMNOPJA();
		ClientMessage value;
		if (!CLOFJFHDFOM.TryGetValue(key, out value))
		{
			HTTPManager.MBBMPNDDPIH().GLCKHLCAPIN("Hub - " + get_Name(), "OnMessage - Sent message not found with id: " + key);
			return;
		}
		switch (CKEHOEGLMBM.get_Type())
		{
		case LENCKBHFKLD.Result:
		{
			ResultMessage mKINDKDMCJO = CKEHOEGLMBM as ResultMessage;
			MergeState(mKINDKDMCJO.FLBBFDNHJAJ());
			if (value.CJPBGBGPFCA != null)
			{
				value.CJPBGBGPFCA(this, value, mKINDKDMCJO);
			}
			CLOFJFHDFOM.Remove(key);
			break;
		}
		case LENCKBHFKLD.Failure:
		{
			FailureMessage kGPJFMCLKDJ = CKEHOEGLMBM as FailureMessage;
			MergeState(kGPJFMCLKDJ.FLBBFDNHJAJ());
			if (value.LIDCKDCPGBB != null)
			{
				value.LIDCKDCPGBB(this, value, kGPJFMCLKDJ);
			}
			CLOFJFHDFOM.Remove(key);
			break;
		}
		case LENCKBHFKLD.Progress:
			if (value.KLNBGJEPNHP != null)
			{
				value.KLNBGJEPNHP(this, value, CKEHOEGLMBM as ProgressMessage);
			}
			break;
		}
	}

	private void MergeState(IDictionary<string, object> state)
	{
		if (state == null || state.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in state)
		{
			FLBBFDNHJAJ()[item.Key] = item.Value;
		}
	}

	private string PDGGBABAPJF(ClientMessage CKEHOEGLMBM)
	{
		try
		{
			builder.Append("{\"H\":\"");
			builder.Append(get_Name());
			builder.Append("\",\"M\":\"");
			builder.Append(CKEHOEGLMBM.Method);
			builder.Append("\",\"A\":");
			string empty = string.Empty;
			empty = ((CKEHOEGLMBM.Args == null || CKEHOEGLMBM.Args.Length <= 0) ? "[]" : ((IHub)this).BAFGHLCPPHM.IBNMFHGHIBI().Encode(CKEHOEGLMBM.Args));
			builder.Append(empty);
			builder.Append(",\"I\":\"");
			builder.Append(CKEHOEGLMBM.CallIdx.ToString());
			builder.Append("\"");
			if (CKEHOEGLMBM.LHDEDFFGBHI.state != null && CKEHOEGLMBM.LHDEDFFGBHI.state.Count > 0)
			{
				builder.Append(",\"S\":");
				empty = ((IHub)this).BAFGHLCPPHM.IBNMFHGHIBI().Encode(CKEHOEGLMBM.LHDEDFFGBHI.state);
				builder.Append(empty);
			}
			builder.Append("}");
			return builder.ToString();
		}
		catch (Exception mPFFFAOGBJE)
		{
			HTTPManager.MBBMPNDDPIH().COHEDILAHFD("Hub - " + get_Name(), "Send", mPFFFAOGBJE);
			return null;
		}
		finally
		{
			builder.Length = 0;
		}
	}
}
