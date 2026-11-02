using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// Playing a friend over the internet or a home network, for free: one player hosts (their copy runs the game) and the
    /// other joins by typing the host's address. No server, store or paid service is involved: a plain TCP connection, which
    /// works directly with the host's port open, or through a free virtual network (Tailscale, ZeroTier, Radmin VPN).
    ///
    /// The host's engine plays the joining player's seat through a RemoteSeat: every decision is sent to the friend's copy
    /// with only what that player may see (the engine builds each request and each view for that seat), and their answer comes
    /// back. The friend's copy runs no engine; it draws the board from those views. So hidden cards never reach the other
    /// player, and the two copies can't drift apart.
    /// </summary>
    public static class OnlinePlay
    {
        /// <summary>Bumped whenever the messages change; both copies must match (and run the same trainer version: same cards and rules).</summary>
        public const int Protocol = 1;
        public const int DefaultPort = 7777;
        /// <summary>The largest message accepted (a board view is a few kilobytes; this only stops a broken or hostile peer).</summary>
        public const int MaxMessage = 8 << 20;

        /// <summary>"host", "host:port" or "[ipv6]:port" into a host name and port (DefaultPort when none is given).</summary>
        public static bool ParseAddress(string text, out string host, out int port)
        {
            host = null;
            port = DefaultPort;
            text = (text ?? "").Trim();
            if (text.Length == 0) return false;
            if (text.StartsWith("[", StringComparison.Ordinal))
            {
                int close = text.IndexOf(']');
                if (close < 0) return false;
                host = text.Substring(1, close - 1);
                string rest = text.Substring(close + 1);
                if (rest.StartsWith(":", StringComparison.Ordinal) && !int.TryParse(rest.Substring(1), out port)) return false;
            }
            else
            {
                int colon = text.LastIndexOf(':');
                if (colon > 0 && text.IndexOf(':') == colon)
                {
                    if (!int.TryParse(text.Substring(colon + 1), out port)) return false;
                    host = text.Substring(0, colon);
                }
                else host = text;
            }
            return host.Length > 0 && port > 0 && port < 65536;
        }

        /// <summary>This computer's own addresses (IPv4), for the host to tell friends (a home network one, or a VPN's).</summary>
        public static List<string> LocalAddresses()
        {
            var list = new List<string>();
            try
            {
                foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
                    if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip)) list.Add(ip.ToString());
            }
            catch (SocketException)
            {
            }
            return list;
        }
    }

    /// <summary>The kinds of message between host and guest.</summary>
    public enum NetMessageType : byte
    {
        /// <summary>Guest to host: protocol and trainer version, name, deck name and decklist.</summary>
        Hello = 1,
        /// <summary>Host to guest: accepted, with the host's name.</summary>
        Welcome = 2,
        /// <summary>Host to guest: refused, with the reason.</summary>
        Reject = 3,
        /// <summary>Host to guest: a game starts (the guest's seat, both names, both deck names, a label).</summary>
        Start = 4,
        /// <summary>Host to guest: a game event and the board as the guest sees it after it.</summary>
        Event = 5,
        /// <summary>Host to guest: a decision for the guest to make.</summary>
        Decide = 6,
        /// <summary>Guest to host: the options chosen.</summary>
        Answer = 7,
        /// <summary>Host to guest: the game is over (back to the lobby).</summary>
        End = 8,
        /// <summary>Either way: leaving, with a reason.</summary>
        Bye = 9,
        /// <summary>Host to guest: a line of text for the lobby or the event screen (standings, pairings...).</summary>
        Note = 10,
        /// <summary>Guest to host: a different deck for the next game.</summary>
        Deck = 11,
        /// <summary>Guest to host: ready for their next Regional Sim game.</summary>
        Ready = 12
    }

    /// <summary>One message as read from the connection.</summary>
    public sealed class NetMessage
    {
        public NetMessageType Type;
        public byte[] Body;
    }

    /// <summary>
    /// One TCP connection carrying length-prefixed messages. A background thread reads them into a queue (Receive takes
    /// them); Send may be called from any thread. Closed when either side leaves or the connection breaks (Closed, Reason).
    /// </summary>
    public sealed class NetConnection : IDisposable
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly object sendLock = new object();
        private readonly BlockingCollection<NetMessage> inbox = new BlockingCollection<NetMessage>();
        private readonly Thread reader;
        private volatile bool closed;

        public bool Closed { get { return closed; } }
        public string Reason { get; private set; }
        public string Remote { get; private set; }

        public NetConnection(TcpClient client)
        {
            this.client = client;
            client.NoDelay = true;
            stream = client.GetStream();
            try
            {
                Remote = client.Client.RemoteEndPoint != null ? client.Client.RemoteEndPoint.ToString() : "";
            }
            catch (SocketException)
            {
                Remote = "";
            }
            reader = new Thread(ReadLoop) { IsBackground = true, Name = "Pokemon TCG online" };
            reader.Start();
        }

        /// <summary>Connects to a host (throws SocketException or TimeoutException on failure).</summary>
        public static NetConnection Connect(string host, int port, int timeoutMs)
        {
            var client = new TcpClient();
            var attempt = client.BeginConnect(host, port, null, null);
            if (!attempt.AsyncWaitHandle.WaitOne(timeoutMs))
            {
                client.Close();
                throw new TimeoutException("No answer from " + host + ":" + port + ".");
            }
            client.EndConnect(attempt);
            return new NetConnection(client);
        }

        private void ReadLoop()
        {
            try
            {
                var head = new byte[5];
                while (!closed)
                {
                    if (!ReadExactly(head, 5)) break;
                    int length = BitConverter.ToInt32(head, 0);
                    if (length < 0 || length > OnlinePlay.MaxMessage) throw new InvalidDataException("message too large");
                    var body = new byte[length];
                    if (!ReadExactly(body, length)) break;
                    inbox.Add(new NetMessage { Type = (NetMessageType)head[4], Body = body });
                }
                Close(Reason ?? "The other player left.");
            }
            catch (Exception e)
            {
                Close(Reason ?? "The connection was lost (" + e.Message + ").");
            }
            finally
            {
                inbox.CompleteAdding();
            }
        }

        private bool ReadExactly(byte[] buffer, int count)
        {
            int at = 0;
            while (at < count)
            {
                int n = stream.Read(buffer, at, count - at);
                if (n <= 0) return false;
                at += n;
            }
            return true;
        }

        public void Send(NetMessageType type, byte[] body)
        {
            if (closed) throw new IOException(Reason ?? "The connection is closed.");
            body = body ?? new byte[0];
            lock (sendLock)
            {
                try
                {
                    stream.Write(BitConverter.GetBytes(body.Length), 0, 4);
                    stream.WriteByte((byte)type);
                    stream.Write(body, 0, body.Length);
                    stream.Flush();
                }
                catch (Exception e)
                {
                    Close("The connection was lost (" + e.Message + ").");
                    throw new IOException(Reason, e);
                }
            }
        }

        public void Send(NetMessageType type, Action<BinaryWriter> write)
        {
            using (var ms = new MemoryStream())
            {
                using (var w = new BinaryWriter(ms))
                {
                    if (write != null) write(w);
                }
                Send(type, ms.ToArray());
            }
        }

        /// <summary>The next message, waiting up to timeoutMs (-1: forever); null on timeout or once the connection has closed.</summary>
        public NetMessage Receive(int timeoutMs, CancellationToken cancel = default(CancellationToken))
        {
            NetMessage m;
            try
            {
                return inbox.TryTake(out m, timeoutMs, cancel) ? m : null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null; // completed: closed and empty
            }
        }

        public void Close(string reason)
        {
            if (closed) return;
            Reason = reason;
            closed = true;
            try
            {
                client.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>Says goodbye (the other side sees why) and closes.</summary>
        public void Leave(string reason)
        {
            try
            {
                if (!closed) Send(NetMessageType.Bye, w => w.Write(reason ?? ""));
            }
            catch (IOException)
            {
            }
            Close(reason);
        }

        public void Dispose()
        {
            Close(Reason ?? "Closed.");
        }
    }

    /// <summary>
    /// Writes and reads the game's own objects (board views, events, decisions) for the connection: every public field, in
    /// name order, of the classes the engine hands players (PlayerView, SideView, PokemonView, CardRef, GameEvent,
    /// DecisionRequest, DecisionOption), with cards sent by their key and looked up in the reader's own card data (both copies
    /// have the same cards). Only those types are ever made: what is read is decided by the field types, never by the data.
    /// </summary>
    public sealed class NetCodec
    {
        public const string FaceDownKey = "#face-down";
        private readonly Func<string, CardDef> cards;
        private static readonly ConcurrentDictionary<Type, FieldInfo[]> fieldCache = new ConcurrentDictionary<Type, FieldInfo[]>();

        /// <param name="cards">Finds a card by its key (null when unknown: a stand-in with the name sent is used).</param>
        public NetCodec(Func<string, CardDef> cards)
        {
            this.cards = cards;
        }

        public byte[] ToBytes<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                using (var w = new BinaryWriter(ms)) Write(w, value, typeof(T));
                return ms.ToArray();
            }
        }

        public T FromBytes<T>(byte[] bytes)
        {
            using (var r = new BinaryReader(new MemoryStream(bytes))) return (T)Read(r, typeof(T));
        }

        private static FieldInfo[] Fields(Type t)
        {
            return fieldCache.GetOrAdd(t, type => type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(f => f.Name, StringComparer.Ordinal).ToArray());
        }

        public void Write(BinaryWriter w, object value, Type type)
        {
            if (type == typeof(int)) w.Write((int)value);
            else if (type == typeof(bool)) w.Write((bool)value);
            else if (type == typeof(long)) w.Write((long)value);
            else if (type == typeof(float)) w.Write((float)value);
            else if (type == typeof(double)) w.Write((double)value);
            else if (type == typeof(byte)) w.Write((byte)value);
            else if (type.IsEnum) w.Write(Convert.ToInt64(value));
            else if (Nullable.GetUnderlyingType(type) != null)
            {
                w.Write(value != null);
                if (value != null) Write(w, value, Nullable.GetUnderlyingType(type));
            }
            else if (type == typeof(string))
            {
                w.Write(value != null);
                if (value != null) w.Write((string)value);
            }
            else if (type == typeof(CardDef))
            {
                var c = (CardDef)value;
                w.Write(c != null);
                if (c == null) return;
                w.Write(ReferenceEquals(c, GameEngine.FaceDownDef) ? FaceDownKey : c.Key ?? "");
                w.Write(c.Name ?? "");
            }
            else if (type.IsArray)
            {
                var a = (Array)value;
                w.Write(a == null ? -1 : a.Length);
                if (a != null) foreach (var item in a) Write(w, item, type.GetElementType());
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var list = (System.Collections.IList)value;
                w.Write(list == null ? -1 : list.Count);
                if (list != null) foreach (var item in list) Write(w, item, type.GetGenericArguments()[0]);
            }
            else if (type.IsClass && type.Namespace == typeof(PlayerView).Namespace)
            {
                w.Write(value != null);
                if (value == null) return;
                foreach (var f in Fields(type)) Write(w, f.GetValue(value), f.FieldType);
            }
            else throw new NotSupportedException("Can't send a " + type.Name + " online.");
        }

        public object Read(BinaryReader r, Type type)
        {
            if (type == typeof(int)) return r.ReadInt32();
            if (type == typeof(bool)) return r.ReadBoolean();
            if (type == typeof(long)) return r.ReadInt64();
            if (type == typeof(float)) return r.ReadSingle();
            if (type == typeof(double)) return r.ReadDouble();
            if (type == typeof(byte)) return r.ReadByte();
            if (type.IsEnum) return Enum.ToObject(type, r.ReadInt64());
            var under = Nullable.GetUnderlyingType(type);
            if (under != null) return r.ReadBoolean() ? Read(r, under) : null;
            if (type == typeof(string)) return r.ReadBoolean() ? r.ReadString() : null;
            if (type == typeof(CardDef))
            {
                if (!r.ReadBoolean()) return null;
                string key = r.ReadString();
                string name = r.ReadString();
                if (key == FaceDownKey) return GameEngine.FaceDownDef;
                return (cards != null ? cards(key) : null) ?? new CardDef { Key = key, Name = name };
            }
            if (type.IsArray)
            {
                int n = Count(r);
                if (n < 0) return null;
                var a = Array.CreateInstance(type.GetElementType(), n);
                for (int i = 0; i < n; i++) a.SetValue(Read(r, type.GetElementType()), i);
                return a;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                int n = Count(r);
                if (n < 0) return null;
                var list = (System.Collections.IList)Activator.CreateInstance(type);
                var item = type.GetGenericArguments()[0];
                for (int i = 0; i < n; i++) list.Add(Read(r, item));
                return list;
            }
            if (type.IsClass && type.Namespace == typeof(PlayerView).Namespace)
            {
                if (!r.ReadBoolean()) return null;
                var ctor = type.GetConstructor(Type.EmptyTypes);
                object o = ctor != null ? ctor.Invoke(null) : FormatterServices.GetUninitializedObject(type);
                foreach (var f in Fields(type)) f.SetValue(o, Read(r, f.FieldType));
                return o;
            }
            throw new NotSupportedException("Can't read a " + type.Name + " sent online.");
        }

        private static int Count(BinaryReader r)
        {
            int n = r.ReadInt32();
            if (n > 100000) throw new InvalidDataException("list too long");
            return n;
        }
    }

    /// <summary>
    /// The joining player's seat in the host's game: the engine asks it like a person on screen (IInteractiveAgent), it sends
    /// each decision to the friend's copy and waits for the answer (checked: a broken one is asked again), and it passes on
    /// every event with the board as the friend sees it. Register it as an observer too (GameEngine.AddObserver): a recorder
    /// wrapped round it doesn't pass events on. If the friend leaves, Fallback plays the rest of their game.
    /// </summary>
    public sealed class RemoteSeat : IInteractiveAgent, IGameObserver
    {
        private readonly NetConnection connection;
        private readonly NetCodec codec;
        public readonly int Seat;
        /// <summary>Plays the seat once the friend has gone (and says so in Lost).</summary>
        public IPlayerAgent Fallback;
        /// <summary>Stops waiting for the friend (the game is being closed).</summary>
        public CancellationToken Cancel;
        public string Lost { get; private set; }

        public RemoteSeat(NetConnection connection, NetCodec codec, int seat, IPlayerAgent fallback)
        {
            this.connection = connection;
            this.codec = codec;
            Seat = seat;
            Fallback = fallback;
        }

        public void OnEvent(GameEngine engine, GameEvent ev)
        {
            if (Lost != null) return;
            try
            {
                var view = PlayerView.Create(engine, Seat);
                connection.Send(NetMessageType.Event, w =>
                {
                    codec.Write(w, ev, typeof(GameEvent));
                    codec.Write(w, view, typeof(PlayerView));
                });
            }
            catch (IOException)
            {
                Lose();
            }
        }

        public List<int> Decide(DecisionRequest r)
        {
            for (int attempt = 0; attempt < 3 && Lost == null; attempt++)
            {
                try
                {
                    connection.Send(NetMessageType.Decide, codec.ToBytes(r));
                    while (true)
                    {
                        if (Cancel.IsCancellationRequested) throw new OperationCanceledException("Game closed.");
                        var m = connection.Receive(500, Cancel);
                        if (m == null)
                        {
                            if (connection.Closed) break;
                            continue;
                        }
                        if (m.Type == NetMessageType.Bye || m.Type == NetMessageType.Reject) break;
                        if (m.Type != NetMessageType.Answer) continue; // anything else is out of place here
                        var picks = codec.FromBytes<List<int>>(m.Body);
                        if (Valid(r, picks)) return picks;
                        break; // asked again
                    }
                    if (connection.Closed) break;
                }
                catch (EndOfStreamException)
                {
                    // a broken answer: asked again
                }
                catch (InvalidDataException)
                {
                }
                catch (IOException)
                {
                    break;
                }
            }
            if (Cancel.IsCancellationRequested) throw new OperationCanceledException("Game closed.");
            Lose();
            return Fallback.Decide(r);
        }

        private void Lose()
        {
            if (Lost == null) Lost = connection.Reason ?? "Your friend left.";
        }

        /// <summary>An answer the engine can take: MinChoices to MaxChoices different options, each one offered.</summary>
        public static bool Valid(DecisionRequest r, List<int> picks)
        {
            if (picks == null || picks.Count < r.MinChoices || picks.Count > Math.Max(r.MaxChoices, 0)) return false;
            var seen = new HashSet<int>();
            foreach (int i in picks) if (i < 0 || i >= r.Options.Count || !seen.Add(i)) return false;
            return true;
        }
    }

    /// <summary>
    /// The host's side before a game: listens on a port and takes one friend, checking they run the same version (protocol and
    /// trainer: same cards and rules) and noting their name, deck name and decklist. Guest is set once someone has joined.
    /// </summary>
    public sealed class OnlineHost : IDisposable
    {
        private readonly TcpListener listener;
        private readonly Thread thread;
        private volatile bool stopping;
        private readonly string hostName;

        public readonly int Port;
        public volatile NetConnection Guest;
        public string GuestName, GuestDeckName, GuestDeckText;
        public string Problem;

        public OnlineHost(int port, string hostName)
        {
            this.hostName = hostName ?? "Host";
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Port = ((IPEndPoint)listener.LocalEndpoint).Port; // the one chosen, for port 0
            thread = new Thread(AcceptLoop) { IsBackground = true, Name = "Pokemon TCG online host" };
            thread.Start();
        }

        private void AcceptLoop()
        {
            while (!stopping)
            {
                TcpClient client;
                try
                {
                    client = listener.AcceptTcpClient();
                }
                catch (Exception)
                {
                    return; // stopped
                }
                var c = new NetConnection(client);
                try
                {
                    var hello = c.Receive(10000);
                    if (hello == null || hello.Type != NetMessageType.Hello)
                    {
                        c.Close("No hello.");
                        continue;
                    }
                    string why = Greet(hello);
                    if (why != null)
                    {
                        c.Send(NetMessageType.Reject, w => w.Write(why));
                        c.Close(why);
                        continue;
                    }
                    if (Guest != null && !Guest.Closed)
                    {
                        c.Send(NetMessageType.Reject, w => w.Write("Someone has already joined this game."));
                        c.Close("busy");
                        continue;
                    }
                    var r = new BinaryReader(new MemoryStream(hello.Body));
                    r.ReadInt32();
                    r.ReadInt32();
                    GuestName = r.ReadString();
                    GuestDeckName = r.ReadString();
                    GuestDeckText = r.ReadString();
                    c.Send(NetMessageType.Welcome, w => w.Write(hostName));
                    Guest = c;
                }
                catch (Exception e)
                {
                    Problem = "A friend tried to join but it failed: " + e.Message;
                    c.Close(Problem);
                }
            }
        }

        /// <summary>Null when the Hello is from the same version, else why not.</summary>
        private static string Greet(NetMessage hello)
        {
            var r = new BinaryReader(new MemoryStream(hello.Body));
            int protocol = r.ReadInt32();
            int trainer = r.ReadInt32();
            if (protocol != OnlinePlay.Protocol || trainer != TrainerVersion.Number)
                return "Different versions of the game (host " + OnlinePlay.Protocol + "/" + TrainerVersion.Number + ", you " + protocol + "/" + trainer
                       + "): both players need the same update.";
            return null;
        }

        /// <summary>Joins a host: sends the Hello and waits for Welcome (returns the host's name) or Reject (throws with the reason).</summary>
        public static NetConnection Join(string address, string name, string deckName, string deckText, out string hostName)
        {
            string host;
            int port;
            if (!OnlinePlay.ParseAddress(address, out host, out port)) throw new ArgumentException("That address isn't valid: type it like 100.64.0.5 or 100.64.0.5:7777.");
            var c = NetConnection.Connect(host, port, 8000);
            c.Send(NetMessageType.Hello, w =>
            {
                w.Write(OnlinePlay.Protocol);
                w.Write(TrainerVersion.Number);
                w.Write(name ?? "Friend");
                w.Write(deckName ?? "");
                w.Write(deckText ?? "");
            });
            var m = c.Receive(10000);
            if (m == null) throw new IOException(c.Reason ?? "The host didn't answer.");
            var r = new BinaryReader(new MemoryStream(m.Body));
            if (m.Type == NetMessageType.Reject)
            {
                string why = r.ReadString();
                c.Close(why);
                throw new IOException(why);
            }
            if (m.Type != NetMessageType.Welcome) throw new IOException("Unexpected answer from the host.");
            hostName = r.ReadString();
            return c;
        }

        /// <summary>Lets the next friend join (after the last one left).</summary>
        public void Forget()
        {
            if (Guest != null) Guest.Close("Left.");
            Guest = null;
        }

        public void Dispose()
        {
            stopping = true;
            try
            {
                listener.Stop();
            }
            catch (Exception)
            {
            }
            if (Guest != null) Guest.Leave("The host closed the game.");
        }
    }
}
