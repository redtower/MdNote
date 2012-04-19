using System;
using System.Collections.Generic;
using System.Text;

namespace MdNote.wpf
{
    public class Option
    {
        Settings.SettingsData _Data;
        private bool _WordWrap = true;

        public bool WordWrap
        {
            get { return _WordWrap; }
            set { _WordWrap = value; }
        }

        public Settings.SettingsData Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        public bool? show()
        {
            OptionForm f = new OptionForm(this);
            return f.ShowDialog();
        }
    }
}
