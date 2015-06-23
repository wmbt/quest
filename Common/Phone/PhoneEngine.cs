using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using NAudio.Wave;


namespace Common.Phone
{
    public class PhoneEngine : IDisposable
    {
        private const int VoicePort = 1550;
        public event CallDroppedEventHandler OnCallDropped;
        public event CallBeginEventHandler OnCallBegin;
        public event CallRecivedEventHandler OnCallRecieved;
        
        private Socket _clientSocket;
        //private EndPoint _ourEndpoint;
        //private EndPoint _remoteEndpoint;
        private EndPoint _otherSideEndPoint;
        private byte[] _byteData = new byte[1024];
        private bool _connected;
        private bool _busy;
        private readonly int _questId;
        
        private UdpClient _udpSender;
        private UdpClient _udpListener;
        
        private readonly WaveIn _waveIn;
        private readonly WaveOut _player;
        private readonly BufferedWaveProvider _waveProvider;
        private readonly AcmALawChatCodec _codec = new AcmALawChatCodec();

        private readonly int _listenPort;
        private readonly int _sendToPort;
        private bool _stopListening;
        private readonly WaveOut _ringer;
        private readonly Mp3FileReader _mp3FileReader;


        public PhoneEngine(int questId, bool disableListening, int sendToPort, int listenPort)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var ring = assembly.GetManifestResourceStream("Common.ring.mp3");
            _mp3FileReader = new Mp3FileReader(ring);
            var loop = new LoopStream(_mp3FileReader);
            _ringer = new WaveOut();
            _ringer.Init(loop);
            
            _listenPort = listenPort;
            _sendToPort = sendToPort;
            _questId = questId;
            var thisMachine = new IPEndPoint(IPAddress.Any, _listenPort);
            
            EndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _clientSocket.Bind(thisMachine);

            if (!disableListening)
            {
                _clientSocket.BeginReceiveFrom(_byteData, 0, _byteData.Length, SocketFlags.None, ref remoteEndpoint,
                    OnCommandReciveCallback, null);
            }

            _waveIn = new WaveIn
            {
                BufferMilliseconds = 100,
                DeviceNumber = 0,
                WaveFormat = _codec.RecordFormat
            };
            _waveIn.DataAvailable += RecorderOnDataAvailable;

            _player = new WaveOut() { DesiredLatency = 100};
            _waveProvider = new BufferedWaveProvider(_codec.RecordFormat);
            _player.Init(_waveProvider);
        }

        public void BeginCall(string serverIp)
        {
            _otherSideEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), _sendToPort);
            _busy = true;
            SendMessage(Command.Invite);
        }

        public void AcceptCall()
        {
            _ringer.Stop();
            SendMessage(Command.OK);
            Connect((IPEndPoint)_otherSideEndPoint);
        }
        
        private void SendMessage(Command cmd)
        {
            try
            {
                //Create the message to send.
                var msgToSend = new Data
                {
                    strName = _questId.ToString(),
                    cmdCommand = cmd
                };

                //Name of the user.
                //Message to send.
                //msgToSend.vocoder = vocoder;        //Vocoder to be used.

                byte[] message = msgToSend.ToByte();

                //Send the message asynchronously.
                _clientSocket.SendTo(message, 0, message.Length, SocketFlags.None, _otherSideEndPoint);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "VoiceChat-SendMessage ()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                _clientSocket.EndSendTo(ar);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "VoiceChat-OnSend ()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnCommandReciveCallback(IAsyncResult ar)
        {
            if (_stopListening)
                return;
            
            _otherSideEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _clientSocket.EndReceiveFrom(ar, ref _otherSideEndPoint);

            var msgRecived = new Data(_byteData);

        //Act according to the received message.
            switch (msgRecived.cmdCommand)
            {
                //We have an incoming call.
                case Command.Invite:
                    {
                        if (_connected == false && _busy == false)
                        {
                            _busy = true;
                            _mp3FileReader.CurrentTime = TimeSpan.Zero;
                            _ringer.Play();
                            OnOnCallRecieved(new CallRecivedEventHandlerArgs(msgRecived.strName));
                        }
                        else
                        {
                            SendMessage(Command.Busy);
                        }
                        break;
                    }

                //OK is received in response to an Invite.
                case Command.OK:
                    {
                        //Start a call.
                        _ringer.Stop();
                        OnOnCallBegin(new CallBeginEventHandlerArgs());
                        Connect((IPEndPoint)_otherSideEndPoint);
                        break;
                    }

                //Remote party is busy.
                case Command.Busy:
                    {
                        //MessageBox.Show("User busy.", "VoiceChat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        OnOnCallDropped(new CallDroppedEventHandlerArgs(false, true));
                        _busy = false;
                        break;
                    }

                case Command.Bye:
                    {
                        _ringer.Stop();
                        Disconnect((IPEndPoint)_otherSideEndPoint);
                        OnOnCallDropped(new CallDroppedEventHandlerArgs(true, false));
                        _busy = false;
                        break;
                    }
            }

            if (_stopListening)
                return;

            _byteData = new byte[1024];
            EndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            //Get ready to receive more commands.
            _clientSocket.BeginReceiveFrom(_byteData, 0, _byteData.Length, SocketFlags.None, ref remoteEndpoint, new AsyncCallback(OnCommandReciveCallback), null);
        }

        public void Connect(IPEndPoint clientEndPoint)
        {
            //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse("1550"));
            var thisMachine = new IPEndPoint(IPAddress.Any, VoicePort);
            _udpSender = new UdpClient();
            _udpListener = new UdpClient();

            // To allow us to talk to ourselves for test purposes:
            // http://stackoverflow.com/questions/687868/sending-and-receiving-udp-packets-between-two-programs-on-the-same-computer
            _udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpListener.Client.Bind(thisMachine);

            _udpSender.Connect(clientEndPoint.Address, VoicePort);

            _player.Play();
            _waveIn.StartRecording();

            _connected = true;
            var state = new ListenerThreadState { Codec = _codec, EndPoint = clientEndPoint };
            ThreadPool.QueueUserWorkItem(ListenerThread, state);
        }

        private void ListenerThread(object state)
        {
            var listenerThreadState = (ListenerThreadState)state;
            var endPoint = listenerThreadState.EndPoint;
            try
            {
                while (_connected)
                {
                    byte[] b = _udpListener.Receive(ref endPoint);
                    byte[] decoded = listenerThreadState.Codec.Decode(b, 0, b.Length);
                    if (_player.PlaybackState == PlaybackState.Playing)
                    {
                        _waveProvider.AddSamples(decoded, 0, decoded.Length);
                    }
                }
            }
            catch (SocketException ex)
            {
                // usually not a problem - just means we have disconnected
            }
        }

        private void Disconnect(IPEndPoint clientEndPoint)
        {
            if (!_connected) 
                    return;
            
            var remoteIp = ((IPEndPoint) _udpSender.Client.RemoteEndPoint).Address;
            var commandIp = clientEndPoint.Address;

            if (!Equals(commandIp, remoteIp))
                return;
            _connected = false;
            _waveIn.StopRecording();
            _player.Stop();
            
            
            _waveIn.DataAvailable -= RecorderOnDataAvailable;
            

            _udpSender.Close();
            _udpListener.Close();
        }

        
        private void RecorderOnDataAvailable(object sender, WaveInEventArgs e)
        {

            byte[] encoded = _codec.Encode(e.Buffer, 0, e.BytesRecorded);
            _udpSender.Send(encoded, encoded.Length);
        }

        public void DropCall()
        {
            try
            {
                _ringer.Stop();
                var otherSideEndPoint = (IPEndPoint)_otherSideEndPoint;
                //Send a Bye message to the user to end the call.
                SendMessage(Command.Bye);
                Disconnect(new IPEndPoint(otherSideEndPoint.Address, VoicePort));
                _busy = false;
                OnOnCallDropped(new CallDroppedEventHandlerArgs(true, false));
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "VoiceChat-DropCall ()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void OnOnCallDropped(CallDroppedEventHandlerArgs args)
        {
            var handler = OnCallDropped;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnOnCallBegin(CallBeginEventHandlerArgs args)
        {
            var handler = OnCallBegin;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnOnCallRecieved(CallRecivedEventHandlerArgs args)
        {
            var handler = OnCallRecieved;
            if (handler != null) handler(this, args);   
        }

        public void Dispose()
        {
            if (_clientSocket != null)
            {
                _stopListening = true;
                _clientSocket.Dispose();
            }
            if (_udpListener != null)
                _udpListener.Close();
            if (_udpSender != null)
                _udpSender.Close();
            if (_player != null)
                _player.Dispose();
            if (_waveIn != null)
                _waveIn.Dispose();

            _ringer.Dispose();
        }
    }

    public delegate void CallRecivedEventHandler(object sender, CallRecivedEventHandlerArgs args);

    public class CallRecivedEventHandlerArgs : EventArgs
    {
        public string ClientId { get; set; }
        public CallRecivedEventHandlerArgs(string clientId)
        {
            ClientId = clientId;
        }
    }

    public delegate void CallBeginEventHandler(object sender, CallBeginEventHandlerArgs args);

    public class CallBeginEventHandlerArgs : EventArgs
    {
    }

    public delegate void CallDroppedEventHandler(object sender, CallDroppedEventHandlerArgs args);

    public class CallDroppedEventHandlerArgs : EventArgs
    {
        public CallDroppedEventHandlerArgs(bool rejected, bool busy)
        {
            Rejected = rejected;
            Busy = busy;
        }

        public bool Rejected { get; set; }
        public bool Busy { get; set; }
    }

    class ListenerThreadState
    {
        public IPEndPoint EndPoint { get; set; }
        public INetworkChatCodec Codec { get; set; }
    }
 
}
