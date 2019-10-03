// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Petabridge, LLC">
//      Copyright (C) 2017 - 2017 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Petabridge.Cmd.Host;
using System.Threading.Tasks;

namespace Petabridge.Cmd.QuickStart
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var actorSys = ActorSystem.Create("MyActorSystem");

            var pbm = PetabridgeCmd.Get(actorSys); // creates the Petabridge.Cmd.Host
            pbm.RegisterCommandPalette(new MsgCommandPaletteHandler()); // register custom command palette
            pbm.Start(); // begins listening for incoming connections on Petabridge.Cmd.Host

            await actorSys.WhenTerminated;
        }
    }
}