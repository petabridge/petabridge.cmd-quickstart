using Akka.Configuration.Hocon;
using System.Configuration;
using System.Linq;
using AkkaConfigurationConfig = Akka.Configuration.Config;

namespace Petabridge.Cmd.QuickStart
{
    public static class AkkaConfiguration
    {
        public static AkkaConfigurationConfig GetAkkaConfig(string sectionName)
        {
            return ((AkkaConfigurationSection)ConfigurationManager.GetSection(sectionName)).AkkaConfig;
        }

        public static AkkaConfigurationConfig GetAkkaConfig(params string[] sectionNames)
        {
            var configurations = sectionNames.Select(GetAkkaConfig);
            var configuration = configurations.Aggregate((c1, c2) => c1.WithFallback(c2));
            return configuration;
        }

        public static class Sections
        {
            public const string AkkaActor = "akka/akka.actor";
            public const string AkkaSerialization = "akka/akka.serialization";
            public const string AkkaLogging = "akka/akka.logging";
            public const string AkkaRemote = "akka/akka.remote";
            public const string AkkaCluster = "akka/akka.cluster";
            public const string AkkaCoordinatedshutdown = "akka/akka.coordinatedshutdown";
            public const string AkkaPetabridgeCmd = "akka/akka.petabridge.cmd";
        }
    }
}