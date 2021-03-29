using System.IO;

namespace UnSave
{
    public static class SaveSerializerExtensions
    {
        public static GvasSaveData ReadFile(this SaveSerializer serializer, string filePath)
        {
            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return serializer.Read(stream);
        }

        public static GvasSaveData ReadFile(this SaveSerializer serializer, FileInfo file)
        {
            using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            return serializer.Read(stream);
        }

        public static void WriteToFile(this SaveSerializer serializer, GvasSaveData saveData, string filePath)
        {
            using var outStream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            serializer.Write(outStream, saveData);
        }

        public static void WriteToFile(this SaveSerializer serializer, GvasSaveData saveData, FileInfo file)
        {
            using var outStream = file.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            serializer.Write(outStream, saveData);
        }

        public static void WriteToFile(this GvasSaveData saveData, SaveSerializer serializer, string filePath)
        {
            using var outStream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            serializer.Write(outStream, saveData);
        }
    }
}