using System;
using System.IO;
using System.Xml.Serialization;

namespace Engine.Utils
{
    public class FlxSave
    {
        public string FileName;
        public string FilePath = null;

        /// <summary>
        /// Root directory to store game files.
        /// </summary>
        public string GameFilesPath = Path.Combine("GameData", "files");

        public FlxSave(string folderName)
        {
            //GameFilesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), folderName);
        }


        public bool Bind(string fileName)
        {
#if WINDOWS
            FileName = fileName;
            // create the game files dir
            Directory.CreateDirectory(GameFilesPath);
            FilePath = ToGameFilesPath(FileName, true);
#endif
            return true;
        }


        /// <summary>
        /// Writes the given object instance to a file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="format">Which format to use with this file.</param>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public void WriteToFile<T>(FileFormats format, T objectToWrite, bool append = false)
        {
#if WINDOWS
            switch (format)
            {
                // write as binary
                case FileFormats.Binary:
                    WriteToBinaryFile<T>(objectToWrite, append);
                    break;

                // write as xml
                case FileFormats.Xml:
                    if (append)
                    {
                        throw new NotSupportedException("Cannot use 'append' option when writing an XML file.");
                    }
                    WriteToXmlFile<T>(objectToWrite);
                    break;

                // should never happen.
                default:
                    throw new NotSupportedException("Unknown file format!");
            }
#endif
        }

        /// <summary>
        /// Reads an object instance from a file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="format">Which format to use with this file.</param>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public T ReadFromFile<T>(FileFormats format)
        {
#if WINDOWS
            if (!File.Exists(FilePath))
                return default;

            switch (format)
            {
                // write as binary
                case FileFormats.Binary:
                    return ReadFromBinaryFile<T>();

                // write as xml
                case FileFormats.Xml:
                    return ReadFromXmlFile<T>();

                // should never happen.
                default:
                    throw new NotSupportedException("Unknown file format!");
            }
#else
            return default;
#endif
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        private void WriteToBinaryFile<T>(T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(FilePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        private T ReadFromBinaryFile<T>()
        {
            using (Stream stream = File.Open(FilePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        private void WriteToXmlFile<T>(T objectToWrite)
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(FilePath, false);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        private T ReadFromXmlFile<T>()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(FilePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


        /// <summary>
        /// Delete an existing file.
        /// </summary>
        /// <param name="filePath">File to delete.</param>
        public void DeleteFile()
        {
#if WINDOWS
            // delete file
            File.Delete(FilePath);
#endif
        }


        // <summary>
        /// Convert path to be under the the game files path (based on GameFilesPath).
        /// </summary>
        /// <param name="path">Path to set.</param>
        /// <param name="createPath">If true, will also create the folders required for path.</param>
        /// <returns>The given path under Game files folder.</returns>
        private string ToGameFilesPath(string path, bool createPath = false)
        {
            // get the full path of the file
            string ret = Path.Combine(GameFilesPath, path);

            // create path if needed
            if (createPath)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ret));
            }

            // return the full path
            return ret;
        }

    }

    /// <summary>
    /// Different file formats we can use.
    /// </summary>
    public enum FileFormats
    {
        /// <summary>
        /// Read / write objects as binary data.
        /// </summary>
        Binary,

        /// <summary>
        /// Read / write objects as XML.
        /// </summary>
        Xml,
    }

}
