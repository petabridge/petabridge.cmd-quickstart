// -----------------------------------------------------------------------
// <copyright file="MsgCommands.cs" company="Petabridge, LLC">
//      Copyright (C) 2017 - 2017 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------
namespace Petabridge.Cmd.QuickStart
{
    /// <summary>
    ///     Contains all of the custom commands used in the Petabridge.Cmd QuickStart
    /// </summary>
    public static class MsgCommands
    {
        public static readonly CommandDefinition CheckMessages =
            new CommandDefinitionBuilder().WithName("view")
                .WithDescription("Views the set of saved messages on the server")
                .WithArgument(
                    builder =>
                        builder.WithName("since")
                            .WithDescription(
                                "A timerange for messages we wish to view. Values should be in the format of 5s, 5m, 10m, 1h.")
                            .IsMandatory(false)
                            .WithDefaultValues("1m", "5m", "30m", "1h", "4hr", "1d")
                            .WithSwitch("-s").WithSwitch("-S"))
                .Build();

        public static readonly CommandDefinition Echo = new CommandDefinitionBuilder().WithName("echo")
            .WithDescription("Echoes a message from the server back to the client")
            .WithArgument(
                b =>
                    b.WithName("message")
                        .WithDescription("The message that will be echoed back from the server to the client.")
                        .IsMandatory(true)
                        .WithSwitch("-m")
                        .WithSwitch("-M"))
            .Build();

        public static readonly CommandDefinition Write = new CommandDefinitionBuilder().WithName("write")
            .WithDescription("Writes a message to the server that can be accessed by other clients.")
            .WithArgument(
                b =>
                    b.WithName("message")
                        .WithDescription("The message that will be written to the server.")
                        .IsMandatory(true)
                        .WithSwitch("-m")
                        .WithSwitch("-M"))
            .Build();

        public static readonly CommandDefinition Purge = new CommandDefinitionBuilder().WithName("purge")
            .WithDescription("Purges all saved messages on the server.").Build();

        public static readonly CommandPalette Palette = new CommandPalette("msg",
            new[] { Write, Echo, CheckMessages, Purge });
    }
}