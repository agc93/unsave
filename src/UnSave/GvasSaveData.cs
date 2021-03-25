using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnSave.Types;

namespace UnSave
{
    /*
     * General format notes:
     * Strings are 4-byte length + value + \0, length includes \0
     *
     */
    public class GvasSaveData
    {
        public static readonly byte[] Header = Encoding.ASCII.GetBytes("GVAS");
        public int SaveGameVersion;
        public int PackageVersion;
        public EngineVersion EngineVersion = new EngineVersion();
        public int CustomFormatVersion;
        public CustomFormatData CustomFormatData = new CustomFormatData();
        public string SaveGameType;
        public List<IUnrealProperty> Properties = new List<IUnrealProperty>();

        public IUnrealProperty this[string index]
        {
            get { return this.Properties.FirstOrDefault(p => p.Name == index); }
        }

       
    }

    public static class SaveDataExtensions
    {
        public static T Get<T>(this GvasSaveData data, string propertyName) where T : class
        {
            var prop = data.Properties.FirstOrDefault(p => p.Name == propertyName);
            return prop as T;
        }
    }
}