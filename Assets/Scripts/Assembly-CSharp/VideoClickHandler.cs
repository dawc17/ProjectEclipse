using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoClickHandler : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public VideoPlayer video;

	public UnityEvent onClickVideo;

	public void OnPointerClick(PointerEventData BHOLFGOGPCP)
	{
		onClickVideo.Invoke();
	}
}
