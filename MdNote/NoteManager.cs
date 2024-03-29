using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;

namespace MdNote
{
    public class NoteManager
    {
        public ObservableCollection<Note> Items = new ObservableCollection<Note>();
    }

    public class NoteManagerFile
    {
        const string DIRNAME = @".notes\";
        const string FILENAME = "noteslist.xml";

        public NoteManagerFile() { }

        private string GetNoteManagerFilePath()
        {
            string p = AppDomain.CurrentDomain.BaseDirectory
                + DIRNAME + FILENAME;

            if (!Directory.Exists(Path.GetDirectoryName(p)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(p));
            }

            return p;
        }

        public void write(NoteManager obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NoteManager));
            FileStream fs = new FileStream(GetNoteManagerFilePath(), FileMode.Create);
            serializer.Serialize(fs, obj);
            fs.Close();
        }

        public ObservableCollection<Note> read()
        {
            NoteManager nm = new NoteManager();
            try
            {
                XmlSerializer xmls = new XmlSerializer(typeof(NoteManager));
                FileStream fs = new FileStream(GetNoteManagerFilePath(), FileMode.Open);
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
