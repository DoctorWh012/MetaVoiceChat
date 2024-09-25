![MetaVoiceChat Banner](MetaVoiceChat.png)

## A simple and self-contained proximity voice chat solution for Unity's popular client-server networking libraries (currently only is Mirror supported*) with all of the features you need.
&ast; Other libraries can easily be implemented by composing an agnostic MonoBehaviour — please feel free to contribute additional implementations.

### A massive thank you to [Vatsal Ambastha](https://github.com/adrenak) and his projects [UniVoice](https://github.com/adrenak/univoice) and [UniMic](https://github.com/adrenak/unimic) that were heavily referenced when starting this project in late 2023.

## Installation
TODO

## Features
- No code required
- No complicated cloud services required
- Many configurable settings
- Voice activation detection and latching
- Pooled data buffers with no runtime allocat6ion
- 16-bit PCM audio
- "ECHO" compilation symbol to playback your own voice when testing
- Functionality and events for
    - Speaking
    - Deafening yourself
    - Input muting yourself
    - Output muting others
- UI settings and indicators with hooks and an official implementation that saves to PlayerPrefs
- Dynamic desync compensation using a latency error P-controller with RMS jitter adjustment
- Opus features
    - Variable bitrate encoding
    - Many exposed settings
- TODO FILL IN OTHER FEATURES
- TODO Set default values for everything
- TODO Add tooltips to all settings (Check audio input and output mic and audio sources)

### Planned Features
- Opus audio encoding
- Documentation
- Push to talk
- Tutorial
    - TODO Include configuration of output audio source, 3d sound and drop off to zero after a distance

## Implementations
- [Mirror](https://github.com/MirrorNetworking/Mirror)

## Improvements Over [UniVoice](https://github.com/adrenak/univoice)
- Fixed memory leak
- Many playback algorithm improvements
- Lower latency
- Dynamic desync compensation

## Missing Things

### Missing Implementations
- [Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/about/)
- [LiteNetLib](https://github.com/RevenantX/LiteNetLib)
- [Fish-Net](https://fish-networking.gitbook.io/docs)
- [Dark Rift 2](https://github.com/DarkRiftNetworking/DarkRift)
- [Unity WebRTC](https://github.com/Unity-Technologies/com.unity.webrtc)
- [Photon Unity Networking 2](https://www.photonengine.com/pun)

### Missing Features
- Examples
- Multiple sample rates
- Dual audio channel support
- Multithreading for Opus
- Agnostic audio inputs and outputs
    - Audio transmission other than voice
    - Configurable filtering pipeline
- Compared to [Dissonace Voice Chat](https://assetstore.unity.com/packages/tools/audio/dissonance-voice-chat-70078)
    - Audio preprocessing
        - Noise supression
        - Dynamic range compression
        - Automatic gain control
    - Audio postprocessing
        - Acoustic echo cancellation
        - Soft clipping
        - Soft channel fade
    - Opus features
        - Forward error correction
    - Multiple chat rooms

## Tips
- Change Project Settings/Audio/DSP Buffer Size from "Best performance" to "Best latency"

## Questions
- Should code fail without throwing exceptions? Should silent failure be an option? E.g. VcMicAudioInput throws sometimes.

## License
This project is licensed under the [MIT License](LICENSE)
