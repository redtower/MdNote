using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MdNote
{
    public class Option
    {
        Settings.SettingsData _Data;
        private bool _WordWrap;

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

        public DialogResult show()
        {
            return new OptionForm(this).ShowDialog();
        }
    }
}
