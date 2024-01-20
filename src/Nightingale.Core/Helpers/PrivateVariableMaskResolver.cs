using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nightingale.Core.Models;
using System.Reflection;

namespace Nightingale.Core.Helpers
{
    /// <summary>
    /// Serialization resolver used to ensure private variables
    /// are not serialized. Often used when exporting Nightingale files.
    /// </summary>
    public class PrivateVariableMaskResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(Parameter) && property.PropertyName == nameof(Parameter.Value))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        Parameter e = (Parameter)instance;
                        return !e.Private;
                    };
            }

            return property;
        }
    }
}
