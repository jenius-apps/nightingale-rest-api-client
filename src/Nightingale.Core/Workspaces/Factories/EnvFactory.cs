using Nightingale.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Workspaces.Factories
{
    /// <summary>
    /// Class for creating environments.
    /// </summary>
    public class EnvFactory : IEnvFactory
    {
        private const string _untitled = "Untitled";
        private const string _baseEnvName = "Base environment";
        private const string _baseEnvIcon = "🌍";
        private const string _defaultEnvIcon = "🌐";

        /// <inheritdoc/>
        public Core.Models.Environment CreateBase(bool isActive = false)
        {
            return Create(_baseEnvName, isActive, EnvType.Base, _baseEnvIcon);
        }

        /// <inheritdoc/>
        public Core.Models.Environment CreateSub(string envName, bool isActive = false, string envIcon = null)
        {
            return Create(envName, isActive, EnvType.Sub, envIcon);
        }

        private Core.Models.Environment Create(
            string envName,
            bool isActive,
            EnvType type,
            string envIcon = null)
        {
            var env = new Core.Models.Environment
            {
                Name = envName?.Trim() ?? _untitled,
                Icon = string.IsNullOrWhiteSpace(envIcon) ? _defaultEnvIcon : envIcon,
                EnvironmentType = type,
                IsActive = isActive,
            };

            return env;
        }
    }
}
