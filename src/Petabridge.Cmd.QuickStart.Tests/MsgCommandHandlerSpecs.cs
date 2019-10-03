using System;
using Akka.Actor;
using Akka.Configuration.Hocon;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Xunit;
using Xunit.Abstractions;

namespace Petabridge.Cmd.QuickStart.Tests
{
    public class MsgCommandHandlerSpecs : TestKit
    {
        public static readonly CommandParser Parser = new CommandParser(MsgCommands.Palette);

        public MsgCommandHandlerSpecs(ITestOutputHelper output) : base(output: output)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Memorizer = Sys.ActorOf(Props.Create(() => new MessageMemorizerActor()), "msgs");
            MsgCommandHandlerActor =
                Sys.ActorOf(Props.Create(() => new MsgCommandHandlerActor(Memorizer)),
                    "commandHandler");
        }

        public IActorRef Memorizer { get; }

        public IActorRef MsgCommandHandlerActor { get; }

        [Theory]
        [InlineData("msg view", true, 5)]
        public void ShouldHandlFetchCmd(string cmdText, bool isValid, int numResponses)
        {
            Memorizer.Tell(new MessageMemorizerActor.Message("foo1", DateTime.UtcNow, "any"));
            Memorizer.Tell(new MessageMemorizerActor.Message("foo2", DateTime.UtcNow.AddMinutes(1), "any"));
            Memorizer.Tell(new MessageMemorizerActor.Message("foo3", DateTime.UtcNow.AddMinutes(5), "any"));
            Memorizer.Tell(new MessageMemorizerActor.Message("foo4", DateTime.UtcNow.AddHours(1), "any"));
            Memorizer.Tell(new MessageMemorizerActor.Message("foo5", DateTime.UtcNow.AddDays(1), "any"));
            ReceiveN(5); 

            
        }
    }
}
