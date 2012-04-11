using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MdNote
{
    public class Note
    {
        private string _Id;
        private string _Title;
        private string _FileName;
        private string _Body;

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public string Body
        {
            get { return _Body; }
            set { _Body = value; }
        }

        public Note()
        {
        }

        public Note(string id, string title, string filename, string body)
        {
            _Id = id;
            _Title = title;
            _FileName = filename;
            _Body = body;
        }
    }

    public class NoteFile
    {
        Note _Note;
        const string DIRNAME = ".notes";
        string _FilePath;

        public NoteFile(Note obj)
        {
            _Note = obj;

            _FilePath = System.AppDomain.CurrentDomain.BaseDirectory;
            _FilePath += @"\" + DIRNAME;

            if (!Directory.Exists(_FilePath))
            {
                Directory.CreateDirectory(_FilePath);
            }

            _FilePath += @"\" + obj.FileName;
        }

        public void write()
        {
            StreamWriter sr = new StreamWriter(
                _FilePath,
                false,                
                System.Text.Encoding.GetEncoding("utf-8"));
            sr.Write(_Note.Body);
            sr.Close();
        }

        public string read()
        {
            if (!File.Exists(_FilePath)) { return null; }

            StreamReader sr = new StreamReader(
                _FilePath,
                System.Text.Encoding.GetEncoding("utf-8"));
            string body = sr.ReadToEnd();
            sr.Close();

            return body;
        }
    }
}
