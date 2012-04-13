using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MdNote
{
    public partial class OptionForm : Form
    {
        private Option _Opt;

        string _FontName = "";
        List<string> _FontLists = new List<string>();

        public OptionForm()
        {
            InitializeComponent();
        }

        public OptionForm(Option opt)
        {
            _Opt = opt;
            InitializeComponent();
        }

        private void OptionForm_Load(object sender, EventArgs e)
        {
            this.ActiveControl = okButton;

            listBox1.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
            SetFontSizeList();
        }

        private void SetFontList()
        {
            //InstalledFontCollectionオブジェクトの取得
            System.Drawing.Text.InstalledFontCollection ifc =
                new System.Drawing.Text.InstalledFontCollection();
            //インストールされているすべてのフォントファミリアを取得
            FontFamily[] ffs = ifc.Families;

            foreach (FontFamily ff in ffs)
            {
                //ここではスタイルにRegularが使用できるフォントのみを表示
                if (ff.IsStyleAvailable(FontStyle.Regular))
                {
                    //Font fnt = new Font(ff, 8);
                    //fnt.Dispose();
                    _FontLists.Add(ff.GetName(0));
                    //listBox1.Items.Add(ff.GetName(0));
                }
            }

            //listBox1.Text = _Opt.FontName;
        }

        private void SetFontSizeList()
        {
            float[] ss = new float[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            foreach (float s in ss)
            {
                listBox2.Items.Add(s);
            }

            textBox4.Text = _Opt.Data.FontSize.ToString();
            listBox2.Text = _Opt.Data.FontSize.ToString();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _Opt.Data.FontName = _FontName;
            _Opt.Data.FontSize = float.Parse(textBox4.Text);
            _Opt.WordWrap = checkBox1.Checked;

            this.Close();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox4.Text = listBox2.Text;
            SetFontSample();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox3.Text = listBox1.Text;
            _FontName = listBox1.Items[listBox1.SelectedIndex].ToString();
            SetFontSample();
        }

        private void SetFontSample()
        {
            if (textBox4.Text.Length != 0)
            {
                label7.Font = new Font(_FontName, float.Parse(textBox4.Text));

                label7.Location = new Point(
                    (panel1.Size.Width - label7.PreferredWidth) / 2,
                    (panel1.Size.Height - label7.PreferredHeight) / 2);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++ )
            {
                if (listBox1.Items[i].ToString().Equals(textBox3.Text))
                {
                    listBox1.Text = listBox1.Items[i].ToString();
                    break;
                }
                else if (listBox1.Items[i].ToString().IndexOf(textBox3.Text) == 0)
                {
                    listBox1.TopIndex = i;
                    break;
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            listBox2.Text = textBox4.Text;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SetFontList();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach(string s in _FontLists)
            {
                listBox1.Items.Add(s);
            }
            listBox1.Text = _Opt.Data.FontName;
            listBox1.Enabled = true;
        }
    }
}