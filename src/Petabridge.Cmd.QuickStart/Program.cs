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
using System.ServiceProcess;
using System.Reflection;

namespace Petabridge.Cmd.QuickStart
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var servicesToRun = new ServiceBase[]
            {
                new PetabridgeCmdService()
            };

            if (Environment.UserInteractive)
            {
                var type = typeof(PetabridgeCmdService);
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

                var onStart = type.GetMethod("OnStart", flags);

                foreach (var service in servicesToRun)
                {
                    onStart.Invoke(service, new object[] { null });
                }

                Console.Title = "Petabridge.Cmd.QuickStart";
                Console.WriteLine("... Press [Enter] to stop service");
                Console.ReadLine();

                var onStop = type.GetMethod("OnStop", flags);

                foreach (var service in servicesToRun)
                {
                    onStop.Invoke(service, null);
                }
            }
            else
            {
                ServiceBase.Run(servicesToRun);
            }            
        }
    }
}