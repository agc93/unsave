using System;
using System.IO;
using System.Text;

namespace UnSave
{
    public static class BinaryWriterExtensions
    {
        private static readonly Encoding Utf8 = new UTF8Encoding(false);
        
        public static void WriteUEString(this BinaryWriter writer, string value)
        {
            if (value == null)
            {
                writer.Write(0);
                return;
            }

            var valueBytes = Utf8.GetBytes(value);
            writer.Write(valueBytes.Length + 1);
            if (valueBytes.Length > 0)
                writer.Write(valueBytes);
            writer.Write((byte)0);
        }

        public static void WriteUEString(this BinaryWriter writer, string value, long vl)
        {
            if (value == null)
            {
                writer.Write(0);
                return;
            }

            var valueBytes = Utf8.GetBytes(value);
            writer.Write(valueBytes.Length + 1);
            if (valueBytes.Length > 0)
                writer.Write(valueBytes);
            writer.Write(false);
            while (vl > valueBytes.Length + 5)
            {
                writer.Write(false);
                vl--;
            }
        }

        public static void WriteInt64(this BinaryWriter writer, long value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }

        public static void WriteInt32(this BinaryWriter writer, int value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }

        public static void WriteInt16(this BinaryWriter writer, short value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }
        public static void WriteSingle(this BinaryWriter writer, float value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }

        public static void WriteDouble(this BinaryWriter writer, double value)
        {
            writer.Write(BitConverter.GetBytes(value));
        }
    }
}