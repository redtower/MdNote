using System;
using System.Xml.Serialization;
using System.IO;
namespace MdNote
{
    public class Settings
    {
        const string SETTINGS_FILENAME = "settings.xml";
        const string FONT_NAME = "‚l‚r ‚oƒSƒVƒbƒN";
        const int FONT_SIZE = 10;
        const int WIDTH = 1024;
        const int HEIGHT = 768;
        const bool MAXIMIZED = false;
        const string CSSURL = @"https://raw.github.com/clownfart/Markdown-CSS/master/markdown.css";

        public class SettingsData
        {
            public string FontName { get; set; }
            public float FontSize { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public bool Maximized { get; set; }
            public string CssUrl { get; set; }

            public SettingsData()
            {
                FontName = FONT_NAME;
                FontSize = FONT_SIZE;
                Width = WIDTH;
                Height = HEIGHT;
                Maximized = MAXIMIZED;
                CssUrl = CSSURL;
            }
        }

        private string FilePath { get; set; }
        public SettingsData AppSettings { get; set; }

        public Settings()
        {
            AppSettings = new SettingsData();
            FilePath = System.AppDomain.CurrentDomain.BaseDirectory
              + SETTINGS_FILENAME;

            read();
        }

        public void write()
        {
            try {
                XmlSerializer xmls = new XmlSerializer(typeof(SettingsData));
                FileStream fs = new FileStream(FilePath, FileMode.Create);
                xmls.Serialize(fs, AppSettings);
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
                FileStream fs = new FileStream(FilePath, FileMode.Open);
                settings = (SettingsData)xmls.Deserialize(fs);
                AppSettings = settings;
                fs.Close();
            } catch (Exception) {
                write();
            }
            return settings;
        }
    }
}
