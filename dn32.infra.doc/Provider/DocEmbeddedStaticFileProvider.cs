using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace dn32.infra {
    internal class DocEmbeddedStaticFileProvider : IFileProvider {
        public EmbeddedFileProvider EmbeddedFileProvider { get; set; }

        public DocEmbeddedStaticFileProvider () {
            EmbeddedFileProvider = new EmbeddedFileProvider (typeof (DnDocController).Assembly);
        }

        public IDirectoryContents GetDirectoryContents (string subpath) {
            return EmbeddedFileProvider.GetDirectoryContents (subpath);
        }

        public IFileInfo GetFileInfo (string path) {
            path = path.Replace ("/", "\\");
            if (path.StartsWith ("\\")) { path = path.Substring (1, path.Length - 1); };

            if (!path.StartsWith ("DnDoc")) {
                path = Path.Combine ("wwwroot", path);
                bool isLinux = RuntimeInformation.IsOSPlatform (OSPlatform.Linux);
                if (isLinux) { path = path.Replace ("/", "\\"); }
                var provider = new PhysicalFileProvider (Directory.GetCurrentDirectory ());
                var fileInfo = provider.GetFileInfo (path);
                return fileInfo;
            }

            path = path.Replace ("/", ".").Replace ("\\", ".");
            return EmbeddedFileProvider.GetFileInfo (path);
        }

        public IChangeToken Watch (string filter) {
            return EmbeddedFileProvider.Watch (filter);
        }
    }
}