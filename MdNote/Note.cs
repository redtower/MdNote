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
        private string _Body = "";

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
    }

    public class NoteFile
    {
        const string DIRNAME = ".notes";
        Note _Note;

        public NoteFile() { }
        public NoteFile(Note obj) 
        {
            _Note = obj;
        }

        private string GetTrashDirectoryPath(Note obj)
        {
            string p = AppDomain.CurrentDomain.BaseDirectory
                + @"\" + DIRNAME + @"\" + "trash";

            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }

            return p;
        }

        private string GetNoteFilePath(Note obj)
        {
            string p = AppDomain.CurrentDomain.BaseDirectory
                + @"\" + DIRNAME
                + @"\" + obj.FileName;

            if (!Directory.Exists(Path.GetDirectoryName(p)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(p));
            }

            return p;
        }

        public void write()
        {
            write(_Note);
        }

        public void write(Note obj)
        {
            StreamWriter sr = new StreamWriter(
                GetNoteFilePath(obj),
                false,
                System.Text.Encoding.GetEncoding("utf-8"));
            sr.Write(obj.Body);
            sr.Close();
        }

        public string read()
        {
            return read(_Note);
        }

        public string read(Note obj)
        {
            if (!File.Exists(GetNoteFilePath(obj))) { return null; }

            StreamReader sr = new StreamReader(
                GetNoteFilePath(obj),
                System.Text.Encoding.GetEncoding("utf-8"));
            string body = sr.ReadToEnd();
            obj.Body = body;
            sr.Close();

            return body;
        }

        public void trash()
        {
            trash(_Note);
        }

        public void trash(Note obj)
        {
            string f = GetNoteFilePath(obj);
            if (!File.Exists(f)) { return; }

            string p = GetTrashDirectoryPath(obj) + @"\" + Path.GetFileName(f);
            if (!File.Exists(p)) { new FileInfo(p).Delete(); }

            new FileInfo(f).MoveTo(p);
        }
    }
}
