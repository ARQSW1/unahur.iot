using Microsoft.EntityFrameworkCore;
using System;

using System.Text.Json.Serialization;
using System.Text.Json;


namespace UNAHUR.IoT.DAL.Converters
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    /// <summary>
    /// Serializa y des-serializa un <see cref="HierarchyId"/> 
    /// </summary>
    public class SqlHierarchyIdJsonConverter : JsonConverter<HierarchyId>
    {

        public override HierarchyId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return HierarchyId.Parse(reader.GetString());

        }


        public override void Write(Utf8JsonWriter writer, HierarchyId value, JsonSerializerOptions options)

        {
            writer.WriteStringValue(value.ToString());
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
