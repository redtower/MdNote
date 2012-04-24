using System;
using System.IO;
using System.ComponentModel;

namespace MdNote
{
    public class Note : INotifyPropertyChanged
    {
        private string _Title;
        public string Id { get; set; }
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }
        public string FileName { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public string Body { get; set; }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public bool IsSave { get; set; }

        public Note()
        {
            Body = "";
            IsSave = false;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NoteFile
    {
        const string DIRNAME = @".notes\";
        const string TRASHDIRNAME = @"trash\";
        Note _Note;

        public NoteFile() { }
        public NoteFile(Note obj) 
        {
            _Note = obj;
        }

        private string GetTrashDirectoryPath(Note obj)
        {
            string p = AppDomain.CurrentDomain.BaseDirectory
                + DIRNAME + TRASHDIRNAME;

            if (!Directory.Exists(p))
            {
                Directory.CreateDirectory(p);
            }

            return p;
        }

        private string GetNoteFilePath(Note obj)
        {
            string p = AppDomain.CurrentDomain.BaseDirectory
                + DIRNAME + obj.FileName;

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

            string p = GetTrashDirectoryPath(obj) + Path.GetFileName(f);
            if (!File.Exists(p)) { new FileInfo(p).Delete(); }

            new FileInfo(f).MoveTo(p);
        }
    }
}
