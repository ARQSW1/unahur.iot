using System;
using System.IO;

namespace UNAHUR.IoT.FirmwareService.Storage
{
    public class FirmwareModel
    {
        public string Repo { get; private set; }
        public string Name { get; private set; }
        public string ContentType { get; private set; }

        public Stream DataStream { get; private set; }

        public FirmwareModel(string repo, string name, string contentType, Stream dataStream) {
            this.Repo = repo;
            this.Name=name;
            this.ContentType = contentType;
            this.DataStream = dataStream;
        }
    }
}
