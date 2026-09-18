using System.Collections.Generic;

public class ModelStrike
{
	private ModelObject _ModelObject;

	public ModelStrike(ModelObject Object)
	{
		_ModelObject = Object;
	}

	public void Strike(ModelEdge Edge, Vector3f Point, Vector3f Impulse)
	{
		if (Edge == null)
		{
			return;
		}
		Decrease();
		ModelNode Start = Edge.GetStartNode();
		ModelNode End = Edge.GetEndNode();
		if (!Start.IsFixed() || !End.IsFixed())
		{
			Vector3f startVector = Start.GetStart();
			Vector3f endVector = End.GetStart();
			float num = Edge.LDKFFINHBOH();
			float num2 = Vector2f.JOIHAKCICMP(Start.GetStart(), Point);
			float num3 = ((!(num < num2)) ? (num2 / num) : 1f);
			if (!Start.IsFixedAndIsNotNode())
			{
				float mass = (1f - num3) / Start.GetWeight();
				Vector3f vector = new Vector3f(Impulse);
				vector.Multiply(mass);
				vector.Add(startVector);
				Edge.GetStartNode().SetStart(vector);
			}
			if (!End.IsFixedAndIsNotNode())
			{
				float force = num3 / End.GetWeight();
				Vector3f vector = new Vector3f(Impulse);
				vector.Multiply(force);
				vector.Add(endVector);
				Edge.GetEndNode().SetStart(vector);
			}
		}
	}

	private void Decrease()
	{
		List<ModelNode> list = _ModelObject.NAMKCLGOPDD();
		foreach (ModelNode item in list)
		{
			Vector3f end = item.GetEnd();
			Vector3f start = item.GetStart();
			end.SetX((end.GetX() + start.GetX()) * 0.5f);
			end.SetY((end.GetY() + start.GetY()) * 0.5f);
			end.SetZ((end.GetZ() + start.GetZ()) * 0.5f);
		}
	}
}
