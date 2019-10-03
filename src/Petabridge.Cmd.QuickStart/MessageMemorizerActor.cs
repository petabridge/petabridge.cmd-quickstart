// -----------------------------------------------------------------------
// <copyright file="MessageMemorizerActor.cs" company="Petabridge, LLC">
//      Copyright (C) 2017 - 2017 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------
using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Petabridge.Cmd.QuickStart
{
    /// <summary>
    ///     Actor responsible for memorizing messages that will be saved on the server.
    /// </summary>
    public class MessageMemorizerActor : ReceiveActor
    {
        private readonly SortedSet<Message> _messages = new SortedSet<Message>();

        public MessageMemorizerActor()
        {
            Receive<Message>(m =>
            {
                _messages.Add(m);
                Sender.Tell(CommandResponse.Empty);
            });

            Receive<FetchMessages>(f => f.Since == null, f => // all messages
            {
                foreach (var msg in _messages)
                    Sender.Tell(new CommandResponse(msg.ToString(), false));
                // by setting final:false we signal to client that more responses are coming
                Sender.Tell(CommandResponse.Empty); // tells the client not to expect any more responses (final == true)
            });

            Receive<FetchMessages>(f =>
            {
                var acceptableTime = DateTime.UtcNow - f.Since;
                var matchingMessages =
                    _messages.Where(x => x.TimeStamp >= acceptableTime).OrderBy(x => x.TimeStamp).ToList();
                foreach (var msg in matchingMessages)
                    Sender.Tell(new CommandResponse(msg.ToString(), false));
                // by setting final:false we signal to client that more responses are coming
                Sender.Tell(CommandResponse.Empty); // tells the client not to expect any more responses (final == true)
            });

            Receive<PurgeMessages>(_ =>
            {
                _messages.Clear();
                Sender.Tell(CommandResponse.Empty);
            });
        }

        public class Message : IComparable<Message>
        {
            public Message(string msg, DateTime timeStamp, string ip)
            {
                Msg = msg;
                TimeStamp = timeStamp;
                Ip = ip;
            }

            public DateTime TimeStamp { get; }
            public string Msg { get; }
            public string Ip { get; }

            public int CompareTo(Message other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                return TimeStamp.CompareTo(other.TimeStamp);
            }

            public override string ToString()
            {
                return $"[{Ip}][{TimeStamp.ToShortTimeString()}]: {Msg}";
            }
        }

        public class FetchMessages
        {
            public FetchMessages(TimeSpan? since = null)
            {
                Since = since;
            }

            public TimeSpan? Since { get; }
        }

        public class PurgeMessages
        {
            public static readonly PurgeMessages Instance = new PurgeMessages();

            private PurgeMessages()
            {
            }
        }
    }
}