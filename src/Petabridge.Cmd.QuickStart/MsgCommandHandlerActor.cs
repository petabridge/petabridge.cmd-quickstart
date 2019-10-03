using Akka.Actor;
using Petabridge.Cmd.Host;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Petabridge.Cmd.QuickStart
{
    /// <summary>
    /// Actor responsible for handling all <see cref="Command"/>s defined inside <see cref="MsgCommands.Palette"/>
    /// </summary>
    public class MsgCommandHandlerActor : CommandHandlerActor
    {
        private readonly IActorRef _messageMemorizer;
        private static readonly Regex TimeRegex = new Regex(@"^(?<value>([0-9]+(\.[0-9]+)?))\s*(?<unit>(ms|s|h|m|d))$", RegexOptions.Compiled);

        public MsgCommandHandlerActor(IActorRef messageMemorizer) : base(MsgCommands.Palette)
        {
            _messageMemorizer = messageMemorizer;
            Process(MsgCommands.CheckMessages.Name, HandleCheckMessages);
            Process(MsgCommands.Write.Name, HandleWrite);
            Process(MsgCommands.Echo.Name, HandleEcho);
            Process(MsgCommands.Purge.Name, HandlePurge);
        }

        public void HandlePurge(Command purge)
        {
            _messageMemorizer.Tell(MessageMemorizerActor.PurgeMessages.Instance, Sender);
        }

        public void HandleWrite(Command write)
        {
            var msg =
                write.Arguments.SingleOrDefault(
                        x => MsgCommands.Write.ArgumentsByName["message"].Switch.Contains(x.Item1))?
                    .Item2;

            _messageMemorizer.Tell(new MessageMemorizerActor.Message(msg, DateTime.UtcNow, Sender.Path.Name), Sender);
        }

        public void HandleEcho(Command echo)
        {
            var msg =
                echo.Arguments.SingleOrDefault(
                        x => MsgCommands.Echo.ArgumentsByName["message"].Switch.Contains(x.Item1))?
                    .Item2;

            Sender.Tell(new CommandResponse(new MessageMemorizerActor.Message(msg, DateTime.UtcNow, Sender.Path.Name).ToString())); // will echo what was written on commandline
        }

        public void HandleCheckMessages(Command fetch)
        {
            // check if we have a timeframe for the message specified
            // what this code does: scans the set of arguments in `fetch` to see
            // if any of the switches for the `since` argument have been used, and if
            // so return the value for that argument. Otherwise, return null.
            var containsTimeframe =
                fetch.Arguments.SingleOrDefault(
                    x => MsgCommands.CheckMessages.ArgumentsByName["since"].Switch.Contains(x.Item1))?.Item2;

            if (containsTimeframe == null)
            {
                _messageMemorizer.Tell(new MessageMemorizerActor.FetchMessages(), Sender); // preserve sender so client gets replies directly
                return;
            }

            // using regular expression to extract the time format
            var m = TimeRegex.Match(containsTimeframe);
            if (!m.Success)
            {
                Sender.Tell(new ErroredCommandResponse($"Unable to extract time format from {containsTimeframe}. Should be in format of [value][ms|s|m|h|d] for milliseconds, seconds, minutes, hours, or days respectively."));
                return;
            }


            try
            {
                var unit = m.Groups["unit"].Value;
                var value = Int32.Parse(m.Groups["value"].Value);

                if (value < 0)
                {
                    Sender.Tell(new ErroredCommandResponse($"Need to pass in positive time value for `since` argument. Instead, received [{value} {unit}]"));
                    return;
                }

                TimeSpan time = TimeSpan.Zero;
                switch (unit)
                {
                    case "ms":
                        time = TimeSpan.FromMilliseconds(value);
                        break;
                    case "s":
                        time = TimeSpan.FromSeconds(value);
                        break;
                    case "m":
                        time = TimeSpan.FromMinutes(value);
                        break;
                    case "h":
                        time = TimeSpan.FromHours(value);
                        break;
                    case "d":
                        time = TimeSpan.FromDays(value);
                        break;
                }

                // make sure the original sender is passed along so reply goes to client
                _messageMemorizer.Tell(new MessageMemorizerActor.FetchMessages(time), Sender);

            }
            catch (Exception ex)
            {
                Sender.Tell(new ErroredCommandResponse($"Error occurred while processing command: {ex.Message}"));
            }
        }
    }
}
