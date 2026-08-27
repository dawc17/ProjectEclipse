public static class ModelType
{
	public enum KEIDBIOIFGA
	{
		MODEL_NULL = 0,
		MODEL_THIS = 1,
		MODEL_OTHER = 2,
		MODEL_PARENT = 3,
		MODEL_CHILD = 4,
		MODEL_BOTH = 5,
		MODEL_OTHER_CHILD = 6
	}

	public static KEIDBIOIFGA EHFNOBFLAHI(string LFLGCDNKNJI)
	{
		switch (LFLGCDNKNJI)
		{
		case "Me":
			return KEIDBIOIFGA.MODEL_THIS;
		case "Enemy":
			return KEIDBIOIFGA.MODEL_OTHER;
		case "Parent":
			return KEIDBIOIFGA.MODEL_PARENT;
		case "Both":
			return KEIDBIOIFGA.MODEL_BOTH;
		case "Null":
			return KEIDBIOIFGA.MODEL_NULL;
		case "Child":
			return KEIDBIOIFGA.MODEL_CHILD;
		case "EnemyChild":
			return KEIDBIOIFGA.MODEL_OTHER_CHILD;
		default:
			LLLOJBFMONN.Error("ModelType - parseType - unknownType: " + LFLGCDNKNJI);
			return KEIDBIOIFGA.MODEL_NULL;
		}
	}
}
