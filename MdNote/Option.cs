using System;
using System.Collections.Generic;
using System.Text;

namespace MdNote
{
    public class Option
    {
        public bool WordWrap { get; set; }
        public Settings.SettingsData Data { get; set; }

        public Option()
        {
            WordWrap = true;
        }

        public bool? show()
        {
            OptionForm f = new OptionForm(this);
            return f.ShowDialog();
        }
    }
}
