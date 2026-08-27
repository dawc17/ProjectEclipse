using System.Collections.Generic;

public class LoadingModule
{
	private List<LoadingModule> _modules = new List<LoadingModule>();

	private int _currentModuleIndex;

	protected bool CHIHBINEGFL;

	private bool EBOAEEHCMDN;

	public LoadingModule()
	{
		CHIHBINEGFL = false;
		EBOAEEHCMDN = false;
		_currentModuleIndex = 0;
	}

	public virtual void Start()
	{
		CHIHBINEGFL = false;
		EBOAEEHCMDN = true;
		_currentModuleIndex = 0;
	}

	public virtual void Stop()
	{
		EBOAEEHCMDN = false;
	}

	public virtual bool GCHANFIHDGH()
	{
		return CHIHBINEGFL;
	}

	public virtual bool JPDPHACFBFB()
	{
		return EBOAEEHCMDN;
	}

	public virtual bool OOPMAAHJMCE()
	{
		return _modules.Count == 0;
	}

	public virtual void JLPMOKPFECK()
	{
		if (!JPDPHACFBFB())
		{
			return;
		}
		if (_currentModuleIndex < _modules.Count)
		{
			LoadingModule pHNHABBBKKL = _modules[_currentModuleIndex];
			if (!pHNHABBBKKL.JPDPHACFBFB())
			{
				pHNHABBBKKL.Start();
			}
			if (!pHNHABBBKKL.GCHANFIHDGH())
			{
				pHNHABBBKKL.JLPMOKPFECK();
			}
			else
			{
				_currentModuleIndex++;
			}
		}
		else
		{
			CHIHBINEGFL = true;
		}
	}

	public virtual void AddModule(LoadingModule ENJECLFOHLD)
	{
		_modules.AddIfNotExist(ENJECLFOHLD);
	}

	public virtual void AddModules(List<LoadingModule> CBLIEGAIBLP)
	{
		_modules.AddIfNotExist(CBLIEGAIBLP);
	}

	public virtual void ClearModules(bool LJCIEGKNKGG = false)
	{
		_modules.Clear();
	}
}
