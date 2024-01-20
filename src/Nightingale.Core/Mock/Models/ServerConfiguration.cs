namespace Nightingale.Core.Mock.Models
{
    public enum ServerResourceType
    {
        None,
        Workspace,
        Item
    }

    /// <summary>
    /// Class with configuration data for mock server.
    /// </summary>
    public class ServerConfiguration
    {
        /// <summary>
        /// Port number to use. If null,
        /// a default port will be assigned.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// The Id of environment to use. Optional.
        /// </summary>
        public string EnvironmentId { get; set; }

        /// <summary>
        /// The Id of the workspace to use for mocking. Required.
        /// </summary>
        public string WorkspaceId { get; set; }

        /// <summary>
        /// The Id of the root item to use for mocking. Optional.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Path to NCF file. Required.
        /// </summary>
        public string NcfPath { get; set; }
    }
}
