using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Host;

namespace Petabridge.Cmd.QuickStart
{
    public class PetabridgeCmdService : ServiceBase
    {
        private ActorSystem _actorSys;

        protected override void OnStart(string[] args)
        {
            var actorSystemName = ConfigurationManager.AppSettings["ActorSystemName"];

            var configuration = AkkaConfiguration.GetAkkaConfig(
                AkkaConfiguration.Sections.AkkaActor,
                AkkaConfiguration.Sections.AkkaRemote,
                AkkaConfiguration.Sections.AkkaCluster,
                AkkaConfiguration.Sections.AkkaPetabridgeCmd);

            _actorSys = ActorSystem.Create(actorSystemName, configuration);
            
            var pbm = PetabridgeCmd.Get(_actorSys); // creates the Petabridge.Cmd.Host
            pbm.RegisterCommandPalette(new MsgCommandPaletteHandler()); // register custom command palette
            pbm.RegisterCommandPalette(ClusterCommands.Instance);

            pbm.Start(); // begins listening for incoming connections on Petabridge.Cmd.Host
                        
        }

        protected override void OnStop()
        {
            _actorSys?.Terminate().Wait();
        }
    }
}
