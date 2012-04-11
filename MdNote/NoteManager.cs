using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Collections;

namespace MdNote
{
    public class NoteManager
    {
        public List<Note> Items = new List<Note>();
    }

    public class NoteManagerFile
    {
        const string DIRNAME = ".notes";
        const string FILENAME = "noteslist.xml";
        string _FilePath;

        public NoteManagerFile()
        {
            _FilePath = System.AppDomain.CurrentDomain.BaseDirectory;
            _FilePath += @"\" + DIRNAME;

            if (!Directory.Exists(_FilePath))
            {
                Directory.CreateDirectory(_FilePath);
            }

            _FilePath += @"\" + FILENAME;
        }

        public void write(NoteManager obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NoteManager));
            FileStream fs = new FileStream(_FilePath, FileMode.Create);
            serializer.Serialize(fs, obj);
            fs.Close();
        }

        public List<Note> read()
        {
            NoteManager nm = new NoteManager();
            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(NoteManager));
                FileStream fs = new FileStream(_FilePath, FileMode.Open);
                nm = (NoteManager)xmls.Deserialize(fs);
                fs.Close();
            }
            catch (Exception)
            {
                write(new NoteManager());
            }

            return nm.Items;
        }
    }
}
