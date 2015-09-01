using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace QuestClient
{
    public class Player : IDisposable
    {
        private static readonly float BackgroundVolume =
            float.Parse(ConfigurationManager.AppSettings["BackgroundMusicVolume"], CultureInfo.InvariantCulture);
        private VolumeSampleProvider _volumeProvider;
        private MediaFoundationReader _mp3File;
        private WaveOut _player;
        private readonly string[] _mp3Files;
        private int _currentMp3FileIndex;
        private bool _mute;
        private bool _playing;

        public bool Mute {
            get { return _mute; }
            set
            {
                _mute = value;
                _volumeProvider.Volume = _mute ? 0 : BackgroundVolume;
            }
        }

        public Player()
        {
            _mp3Files = Directory.GetFiles(@"Media", "*.mp3", SearchOption.TopDirectoryOnly).OrderBy(x => x).ToArray();
        }

        public void Play()
        {
            if (_mp3Files.Length == 0)    
                return;

            _currentMp3FileIndex = 0;

             PrepareTrack(_currentMp3FileIndex);
            _playing = true;
            _player.Play();
        }

        private void PrepareTrack(int trackIndex)
        {
            _player = new WaveOut();
            _mp3File = new MediaFoundationReader(_mp3Files[trackIndex]);
            _volumeProvider = new VolumeSampleProvider(_mp3File.ToSampleProvider()) { Volume = _mute ? 0 : BackgroundVolume };
            _player.DesiredLatency = 1000;
            _player.Init(_volumeProvider);
            _mp3File.CurrentTime = TimeSpan.Zero;
            _player.PlaybackStopped += OnPlaybackStopped;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {
            if (!_playing)
                return;
            
            _player.Dispose();

            if (_currentMp3FileIndex < _mp3Files.Length - 1)
            {
                _currentMp3FileIndex++;
            }
            else
            {
                _currentMp3FileIndex = 0;
            }

            PrepareTrack(_currentMp3FileIndex);
            _player.Play();
        }

        public void Stop()
        {
            if (!_playing)
                return;

            _player.Dispose();
            _currentMp3FileIndex = 0;
            _playing = false;
        }

        public void Dispose()
        {
            if (_player != null)
            {
                _player.Dispose();
            }
        }
    }
}
