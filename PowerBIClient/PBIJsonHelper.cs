using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using gbrueckl.PowerBI.API.PowerBIObjects;
using Microsoft.PowerBI.Api.V2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace gbrueckl.PowerBI.API
{
    public static class PBIJsonHelper
    {
        public static string SerializeObject(object objectToSerialize)
        {
            var settings = new JsonSerializerSettings { ContractResolver = new JsonIgnoreSerializeContractResolver(), NullValueHandling = NullValueHandling.Ignore };

            string json = JsonConvert.SerializeObject(objectToSerialize, settings);

            return json;
        }
    }
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    class JsonIgnoreSerialize : Attribute { }

    public class JsonIgnoreSerializeContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            Type classType = null;
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (property != null && property.Writable)
            {
                var attributes = property.AttributeProvider.GetAttributes(typeof(JsonIgnoreSerialize), true);
                if (attributes != null && attributes.Count > 0)
                {
                    property.Writable = false;
                    property.Ignored = true;
                }
            }

            // dynamically get the corresponding PBI-API object if a property from a PBIv2 is serialized
            classType = Type.GetType(this.GetType().Namespace + ".PowerBIObjects.PBI" + member.DeclaringType.Name);
            if(classType == null)
                classType = member.DeclaringType;

            MethodInfo method = classType.GetMethod("ShouldSerialize_" + property.PropertyName);

            if (method != null && method.ReturnType == typeof(bool))
            {
                property.ShouldSerialize = instance =>
                {
                    bool? shouldSerialize = method.Invoke(instance, null) as bool?;
                    return shouldSerialize.HasValue ? shouldSerialize.Value : true;
                };
            }

            return property;
        }
    }
}
