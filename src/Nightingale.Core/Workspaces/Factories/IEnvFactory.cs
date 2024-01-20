namespace Nightingale.Core.Workspaces.Factories
{
    /// <summary>
    /// Interface for creating an environment.
    /// </summary>
    public interface IEnvFactory
    {
        /// <summary>
        /// Base environment created. Returns a base env.
        /// </summary>
        /// <param name="envName">The name of the environment.</param>
        /// <param name="isActive">Default false.</param>
        /// <returns>Returns a base env.</returns>
        Core.Models.Environment CreateBase(bool isActive = false);

        /// <summary>
        /// Sub environment created. Returns a sub env.
        /// </summary>
        /// <param name="envName">The name of the environment.</param>
        /// <param name="isActive">Default false.</param>
        /// <returns>Returns a sub env.</returns>
        Core.Models.Environment CreateSub(
            string envName,
            bool isActive = false,
            string envIcon = null);
    }
}
