using System;

public abstract class ChainedEventEmitter : IEventEmitter
{
	protected readonly IEventEmitter JDJEJDIJLLE;

	protected ChainedEventEmitter(IEventEmitter JDJEJDIJLLE)
	{
		if (JDJEJDIJLLE == null)
		{
			throw new ArgumentNullException("nextEmitter");
		}
		this.JDJEJDIJLLE = JDJEJDIJLLE;
	}

	public virtual void Emit(AliasEventInfo FNHCFCAALAE)
	{
		JDJEJDIJLLE.Emit(FNHCFCAALAE);
	}

	public virtual void Emit(ScalarEventInfo FNHCFCAALAE)
	{
		JDJEJDIJLLE.Emit(FNHCFCAALAE);
	}

	public virtual void Emit(LPADMPIAIPF FNHCFCAALAE)
	{
		JDJEJDIJLLE.Emit(FNHCFCAALAE);
	}

	public virtual void Emit(EKKDGIILGMA FNHCFCAALAE)
	{
		JDJEJDIJLLE.Emit(FNHCFCAALAE);
	}

	public virtual void Emit(PBGMOJFHMGI FNHCFCAALAE)
	{
		JDJEJDIJLLE.Emit(FNHCFCAALAE);
	}

	public virtual void Emit(NCGDJIDCIIM FNHCFCAALAE)
	{
		JDJEJDIJLLE.Emit(FNHCFCAALAE);
	}
}
