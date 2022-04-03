using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace WebRequestAudio.ExampleScene
{
	public class PlayAudioFromUnityWebRequestFileUrl : MonoBehaviour
	{
		[SerializeField] private string _urlToLoadFile;
		[SerializeField] private Button _button;
		[SerializeField] private AudioSource _audioSource;
		[SerializeField] private AudioType _audioType;
		[SerializeField] private bool _enableStreaming = false;

		private System.Action _disposer;

		private void OnEnable()
		{
			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnDisable()
		{
			_button.onClick.RemoveListener(OnButtonClick);
			_disposer?.Invoke();
		}

		private async void OnButtonClick()
		{
			Debug.Log("Click On " + gameObject.name);

			_disposer?.Invoke();
			_disposer = () => { };
			var cancellationToken = new CancellationTokenSource();
			DisposableAudioWebRequest requestWithAudio = null;
			_disposer += () =>
			{
				cancellationToken.Cancel();
				if (requestWithAudio == null)
				{
					return;
				}

				if (requestWithAudio.AudioClip != null)
				{
					Debug.Log("AudioClip disposed " + requestWithAudio.AudioClip.Disposed);
					requestWithAudio.AudioClip.Dispose();
				}

				Debug.Log("Request was finished " + requestWithAudio.IsDone);
				Debug.Log("Request Already disposed " + requestWithAudio.IsDisposed);

				requestWithAudio.Dispose();
			};
			var requestTask = AudioFromWebRequest
				.LoadAudioFrom(
					_audioSource,
					_urlToLoadFile,
					null,
					_audioType,
					_enableStreaming,
					1024,
					cancellationToken.Token);
			requestWithAudio = await requestTask.Task.ConfigureAwait(true);

			if (cancellationToken.IsCancellationRequested || requestWithAudio.HasErrors())
			{
				Debug.Log("Cancel Or Error Request " + requestWithAudio.GetReadableError());
				Debug.Log("Request was finished " + requestWithAudio.IsDone);
				Debug.Log("Request disposed " + requestWithAudio.IsDisposed);
				Debug.Log("Request had audio " + (requestWithAudio.AudioClip != null));
				requestWithAudio.AudioClip?.Dispose();
				return;
			}

			Debug.Log("Request done " + requestWithAudio.IsDone);

			_audioSource.Stop();
			_audioSource.clip = requestWithAudio.AudioClip.AudioClip;
			_audioSource.Play();
			Debug.Log("Playing clip: " + _audioSource.isPlaying);

			await WaitForDispose(requestWithAudio, cancellationToken.Token);
			Debug.Log("Request done " + requestWithAudio.IsDone);
			Debug.Log("Request Already disposed " + requestWithAudio.IsDisposed);
		}

		private static async Task WaitForDispose(DisposableAudioWebRequest request, CancellationToken token)
		{
			while (!request.IsDone && !request.HasErrors() && !token.IsCancellationRequested)
			{
				await Task.Delay(500);
			}
		}
	}
}