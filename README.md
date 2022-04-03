# Loading async Audio clip or streaming audio clip
[![License][License-Badge]][License]

> **Requires** Unity Editor 2017.4 +

## Getting Started

Please refer to the sample usage sccript [Example]

See full example scene:

/Runtime/ExampleScene/PlayAudio.unity


## Usage

```
var requestTask = AudioFromWebRequest.LoadAudioFrom(audiosource, url,headers,audioType,enableStreaming,minKbForStreaming,cancelationToken );

var requestWithAudio = await requestTask.Task.ConfigureAwait(true);

// Get ready clip to be used in Unity Audio Source
_audioSource.clip = requestWithAudio.AudioClip.AudioClip;
```

- can be used both with local files (System.Uri that starts with file:///)
- can only be used with Get requests and custom headers


## License

Code released under the [MIT License][License].

[License-Badge]: https://img.shields.io/github/license/ExtendRealityLtd/Tilia.Utilities.Shaders.Unity.svg


[License]: LICENSE.md

[Example]: ./Runtime/ExampleScene/PlayAudioFromUnityWebRequestFileUrl.cs