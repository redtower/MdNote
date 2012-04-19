using System;
using System.Xml.Serialization;
using System.IO;
namespace MdNote.wpf
{
    public class Settings
    {
        const string SETTINGS_FILENAME = "settings.xml";
        const string FONT_NAME = "‚l‚r ‚oƒSƒVƒbƒN";
        const int FONT_SIZE = 10;
        const int WIDTH = 1024;
        const int HEIGHT = 768;
        const bool MAXIMIZED = false;
        const string CSSURL = @"http://kevinburke.bitbucket.org/markdowncss/markdown.css";

        public class SettingsData
        {
            private string _FontName = "";
            private float _FontSize;
            private int _Width;
            private int _Height;
            private bool _Maximized;
            private string _CssUrl;

            public string FontName
            {
                get { return _FontName; }
                set { _FontName = value; }
            }

            public float FontSize
            {
                get { return _FontSize; }
                set { _FontSize = value; }
            }

            public int Width
            {
                get { return _Width; }
                set { _Width = value; }
            }

            public int Height
            {
                get { return _Height; }
                set { _Height = value; }
            }

            public bool Maximized
            {
                get { return _Maximized; }
                set { _Maximized = value; }
            }

            public string CssUrl
            {
                get { return _CssUrl; }
                set { _CssUrl = value; }
            }

            public SettingsData()
            {
                _FontName = FONT_NAME;
                _FontSize = FONT_SIZE;
                _Width = WIDTH;
                _Height = HEIGHT;
                _Maximized = MAXIMIZED;
                _CssUrl = CSSURL;
            }
        }

        string _FilePath;
        private SettingsData _AppSettings = new SettingsData();

        public SettingsData AppSettings
        {
            get { return _AppSettings; }
            set { _AppSettings = value; }
        }

        public Settings()
        {
            _FilePath = System.AppDomain.CurrentDomain.BaseDirectory
              + SETTINGS_FILENAME;

            read();
        }

        public void write()
        {
            try {
                XmlSerializer xmls = new XmlSerializer(typeof(SettingsData));
                FileStream fs = new FileStream(_FilePath, FileMode.Create);
                xmls.Serialize(fs, _AppSettings);
                fs.Close();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public SettingsData read()
        {
            SettingsData settings = null;
            try {
                XmlSerializer xmls = new XmlSerializer(typeof(SettingsData));
                FileStream fs = new FileStream(_FilePath, FileMode.Open);
                settings = (SettingsData)xmls.Deserialize(fs);
                _AppSettings = settings;
                fs.Close();
            } catch (Exception) {
                write();
            }
            return settings;
        }
    }
}
