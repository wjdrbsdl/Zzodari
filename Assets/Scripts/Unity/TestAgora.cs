using UnityEngine;
using UnityEngine.UI;
using Agora.Rtc;

public class TestAgora : MonoBehaviour
{
    internal IRtcEngine RtcEngine;
    // Fill in your app ID
    private string _appID = "99c7c0a0367b42f2aa11bf43e48134a8";

    private void Start()
    {
        SetupVideoSDKEngine();
        InitEventHandler();
        
    }

    private void SetupVideoSDKEngine()
    {
        // Create an IRtcEngine instance
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        RtcEngineContext context = new RtcEngineContext();
        context.appId = _appID;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION;
        context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        // Initialize the instance
        RtcEngine.Initialize(context);
    }

    // Fill in your channel name
    private string _channelName = "TT";
    // Fill in a temporary token
    public static string _token;
    
    public void Join()
    {
        Debug.Log("Joining _channelName");
        // Enable the audio module
        RtcEngine.EnableAudio();
        // Set channel media options
        ChannelMediaOptions options = new ChannelMediaOptions();
        // Publish the audio stream captured by the microphone
        options.publishMicrophoneTrack.SetValue(true);
        // Automatically subscribe to all audio streams
        options.autoSubscribeAudio.SetValue(true);
        // Set the channel profile to live broadcast
        options.channelProfile.SetValue(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION);
        // Set the user role to broadcaster
        options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        // Join the channel
        RtcEngine.JoinChannel(_token, _channelName, 0, options);
    }

    public void Leave()
    {
        Debug.Log("Leaving " + _channelName);
        // Leave the channel
        RtcEngine.LeaveChannel();
        // Disable the audio module
        RtcEngine.DisableAudio();
    }

    private void InitEventHandler()
    {
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngine.InitEventHandler(handler);
    }
}

internal class UserEventHandler : IRtcEngineEventHandler
{
    private readonly TestAgora _audioSample;
    internal UserEventHandler(TestAgora audioSample)
    {
        _audioSample = audioSample;
    }
    // Triggered when the local user successfully joins a channel
    public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
    {
        Debug.Log("OnJoinChannelSuccess _channelName");
    }

    public override void OnError(int err, string msg)
    {
        Debug.Log("¿¡·¯ " + msg);
    }
    // Triggered when a remote user successfully joins a channel
    public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
    {
        Debug.Log("Remote user joined");
    }
    // Triggered when a remote user leaves the current channel
    public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
    {
    }
}