using System.Reflection;
using System.Runtime.InteropServices;

namespace MROCoatching.DataObjects.Models.Others
{
    public class IndexModel
    {
        public string AssemblyVersion { get; set; }
        public IndexModel()
        {
            AssemblyVersion = typeof(RuntimeEnvironment).GetTypeInfo()
                .Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            //TODO
            //AssemblyVersion = @Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;
        }
    }
}
