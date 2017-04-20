// -----------------------------------------------------------------------
// <copyright file="MsgCommandPaletteHandler.cs" company="Petabridge, LLC">
//      Copyright (C) 2017 - 2017 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------
using Akka.Actor;
using Petabridge.Cmd.Host;

namespace Petabridge.Cmd.QuickStart
{
    /// <summary>
    ///     Gets registered with <see cref="PetabridgeCmd" /> in order to enable the server to begin processing
    ///     <see cref="MsgCommands.Palette" /> using the <see cref="MsgCommandHandlerActor" /> and the
    ///     <see cref="MessageMemorizerActor" />.
    /// </summary>
    public class MsgCommandPaletteHandler : CommandPaletteHandler
    {
        private Props _underlyingProps;

        public MsgCommandPaletteHandler()
            : base(MsgCommands.Palette) // registers the command palette with this handler.
        {
        }

        public override Props HandlerProps => _underlyingProps;

        /*
         * Overriding this method gives us the ability to do things like create the MessageMemorizerActor before HandlerProps gets used
         */

        public override void OnRegister(PetabridgeCmd plugin)
        {
            var memorizer = plugin.Sys.ActorOf(Props.Create(() => new MessageMemorizerActor()), "pbm-msg-memorizier");

            // will be used to create a new MsgCommandHandlerActor instance per connection
            _underlyingProps = Props.Create(() => new MsgCommandHandlerActor(memorizer));
            base.OnRegister(plugin);
        }
    }
}