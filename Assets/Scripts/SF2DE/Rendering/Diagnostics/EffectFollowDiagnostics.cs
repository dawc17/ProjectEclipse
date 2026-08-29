using UnityEngine;

namespace SF2DE.Rendering.Diagnostics
{
	public sealed class EffectFollowDiagnostics
	{
		private Vector3 _previousAnchor;
		private bool _hasPreviousAnchor;
		private int _loggedAnchorJumps;
		private int _previousDirection;

		public void Observe(Model actor, ActionEffect action, Vector3 anchor, int direction)
		{
			if (_hasPreviousAnchor)
			{
				float anchorStep = Vector3.Distance(_previousAnchor, anchor);
				if ((anchorStep > 40f || (_previousDirection != 0 && _previousDirection != direction)) &&
					_loggedAnchorJumps < 5)
				{
					Debug.LogWarning("[EffectTransform] follow-jump actor=" + actor.get_Name() +
						" action=" + action.get_Name() +
						" sequence=" + action.EPDMGFELIMC() +
						" step=" + anchorStep +
						" from=" + _previousAnchor.x + "," + _previousAnchor.y +
						" to=" + anchor.x + "," + anchor.y +
						" direction=" + _previousDirection + "->" + direction +
						" frame=" + Time.frameCount);
					_loggedAnchorJumps++;
				}
			}
			_previousAnchor = anchor;
			_previousDirection = direction;
			_hasPreviousAnchor = true;
		}
	}
}
