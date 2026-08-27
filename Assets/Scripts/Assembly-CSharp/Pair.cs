public class Pair<T1, T2>
{
	public T1 First;

	public T2 Second;

	public Pair(T1 GBCLEDJAOBM, T2 POFHDGJAFMP)
	{
		First = GBCLEDJAOBM;
		Second = POFHDGJAFMP;
	}

	public override string ToString()
	{
		return string.Format("{0} : {1}", First.ToString(), Second.ToString());
	}
}
