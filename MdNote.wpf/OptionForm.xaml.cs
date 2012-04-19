using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Drawing;

namespace MdNote.wpf
{
    /// <summary>
    /// Option.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionForm : Window
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

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            bg.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);

            fontListBox.IsEnabled = false;
            bg.RunWorkerAsync();
            SetFontSizeList();
        }

        private void SetFontList()
        {
            //InstalledFontCollectionオブジェクトの取得
            System.Drawing.Text.InstalledFontCollection ifc =
                new System.Drawing.Text.InstalledFontCollection();
            //インストールされているすべてのフォントファミリアを取得
            System.Drawing.FontFamily[] ffs = ifc.Families;

            foreach (System.Drawing.FontFamily ff in ffs)
            {
                //ここではスタイルにRegularが使用できるフォントのみを表示
                if (ff.IsStyleAvailable(System.Drawing.FontStyle.Regular))
                {
                    _FontLists.Add(ff.GetName(0));
                }
            }
        }

        private void SetFontSizeList()
        {
            float[] ss = new float[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            foreach (float s in ss)
            {
                sizeListBox.Items.Add(s);
            }

            sizeTextBox.Text = _Opt.Data.FontSize.ToString();
            sizeListBox.SelectedItem = _Opt.Data.FontSize.ToString();
        }

    //    private void cancelButton_Click(object sender, EventArgs e)
    //    {
    //        this.Close();
    //    }

    //    private void okButton_Click(object sender, EventArgs e)
    //    {
    //        _Opt.Data.FontName = _FontName;
    //        _Opt.Data.FontSize = float.Parse(textBox4.Text);
    //        _Opt.WordWrap = checkBox1.Checked;

    //        this.Close();
    //    }

    //    private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        textBox4.Text = listBox2.Text;
    //        SetFontSample();
    //    }

    //    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        textBox3.Text = listBox1.Text;
    //        _FontName = listBox1.Items[listBox1.SelectedIndex].ToString();
    //        SetFontSample();
    //    }

    //    private void SetFontSample()
    //    {
    //        if (textBox4.Text.Length != 0)
    //        {
    //            label7.Font = new Font(_FontName, float.Parse(textBox4.Text));

    //            label7.Location = new Point(
    //                (panel1.Size.Width - label7.PreferredWidth) / 2,
    //                (panel1.Size.Height - label7.PreferredHeight) / 2);
    //        }
    //    }

    //    private void textBox3_TextChanged(object sender, EventArgs e)
    //    {
    //        for (int i = 0; i < listBox1.Items.Count; i++ )
    //        {
    //            if (listBox1.Items[i].ToString().Equals(textBox3.Text))
    //            {
    //                listBox1.Text = listBox1.Items[i].ToString();
    //                break;
    //            }
    //            else if (listBox1.Items[i].ToString().IndexOf(textBox3.Text) == 0)
    //            {
    //                listBox1.TopIndex = i;
    //                break;
    //            }
    //        }
    //    }

    //    private void textBox4_TextChanged(object sender, EventArgs e)
    //    {
    //        listBox2.Text = textBox4.Text;
    //    }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            SetFontList();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (string s in _FontLists)
            {
                fontListBox.Items.Add(s);
            }
            fontListBox.SelectedItem = _Opt.Data.FontName;
            fontListBox.IsEnabled = true;
        }
    }
}
