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
        //ArrayListに追加される型を指定する
        [XmlArrayItem(typeof(Note))]
        public ArrayList Items = new ArrayList();
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
            //ArrayListに追加されているオブジェクトを指定してXMLファイルに保存する
            XmlSerializer serializer = new XmlSerializer(typeof(NoteManager));
            FileStream fs = new FileStream(_FilePath, FileMode.Create);
            serializer.Serialize(fs, obj);
            fs.Close();
        }

        public ArrayList read()
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
