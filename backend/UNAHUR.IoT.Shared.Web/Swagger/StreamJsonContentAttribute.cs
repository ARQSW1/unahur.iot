using System;

namespace UNAHUR.IoT.Shared.Web.Swagger
{
    /// <summary>
    /// Agrega un parametro de body con un esquema vacio que admite propiedades adicionales, es decir json sin esquema
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StreamJsonContentAttribute : Attribute
    {
    }
}
