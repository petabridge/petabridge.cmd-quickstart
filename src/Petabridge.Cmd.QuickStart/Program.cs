// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Petabridge, LLC">
//      Copyright (C) 2017 - 2017 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Configuration;
using Akka.Actor;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;

namespace Petabridge.Cmd.QuickStart
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var actorSystemName = ConfigurationManager.AppSettings["ActorSystemName"];

            var configuration = AkkaConfiguration.GetAkkaConfig(
                AkkaConfiguration.Sections.AkkaActor,
                AkkaConfiguration.Sections.AkkaRemote,
                AkkaConfiguration.Sections.AkkaCluster,
                AkkaConfiguration.Sections.AkkaPetabridgeCmd);

            using (var actorSys = ActorSystem.Create(actorSystemName, configuration))
            {
                var pbm = PetabridgeCmd.Get(actorSys); // creates the Petabridge.Cmd.Host
                pbm.RegisterCommandPalette(new MsgCommandPaletteHandler()); // register custom command palette
                pbm.RegisterCommandPalette(ClusterCommands.Instance);

                pbm.Start(); // begins listening for incoming connections on Petabridge.Cmd.Host

                actorSys.WhenTerminated.Wait();
            }
        }
    }
}