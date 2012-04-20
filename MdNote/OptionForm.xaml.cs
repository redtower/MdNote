using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Drawing.Text;

namespace MdNote.wpf
{
    /// <summary>
    /// Option.xaml の相互作用ロジック
    /// </summary>
    public partial class OptionForm : Window
    {
        private Option _Opt;
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
            _Opt.Data.FontName = (string)fontListBox.SelectedItem;
            float f;
            if (float.TryParse(sizeTextBox.Text, out f))
            {
                _Opt.Data.FontSize = f;
            }

            if (!wordWrapCheckbox.IsChecked.HasValue)
            {
                _Opt.WordWrap = false;
            }
            else
            {
                _Opt.WordWrap = (bool)wordWrapCheckbox.IsChecked;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(this.bg_DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bg_RunWorkerCompleted);

            fontListBox.IsEnabled = false;
            bg.RunWorkerAsync();
            SetFontSizeList();
        }

        private void SetFontList()
        {
            //InstalledFontCollectionオブジェクトの取得
            InstalledFontCollection ifc = new InstalledFontCollection();
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
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            SetFontList();
        }

        private void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (string s in _FontLists)
            {
                fontListBox.Items.Add(s);
            }
            fontTextBox.Text = _Opt.Data.FontName;
            fontListBox.IsEnabled = true;
        }

        private void fontListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fontTextBox.Text = fontListBox.Items[fontListBox.SelectedIndex].ToString();
            SetFontSample();
        }

        private void sizeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sizeTextBox.Text = sizeListBox.Items[sizeListBox.SelectedIndex].ToString();
            SetFontSample();
        }

        private void SetFontSample()
        {
            if (fontTextBox.Text.Length != 0)
            {
                sampleLabel.FontFamily = new System.Windows.Media.FontFamily(fontTextBox.Text);
                sampleLabel.FontSize = double.Parse(sizeTextBox.Text);
            }
        }

        private void fontTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            for (int i = 0; i < fontListBox.Items.Count; i++)
            {
                if (fontListBox.Items[i].ToString().Equals(fontTextBox.Text))
                {
                    fontListBox.SelectedIndex = i;
                    fontListBox.ScrollIntoView(fontListBox.Items[i].ToString());
                    break;
                }
                else if (fontListBox.Items[i].ToString().IndexOf(fontTextBox.Text) == 0)
                {
                    fontListBox.ScrollIntoView(fontListBox.Items[i].ToString());
                    break;
                }
            }
        }

        private void sizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isReSet = true;
            for (int i = 0; i < sizeListBox.Items.Count; i++)
            {
                if (sizeListBox.Items[i].ToString().Equals(sizeTextBox.Text))
                {
                    sizeListBox.SelectedIndex = i;
                    sizeListBox.ScrollIntoView(sizeListBox.Items[i].ToString());
                    isReSet = false;
                    break;
                }
            }
            float f;
            if (isReSet && float.TryParse(sizeTextBox.Text, out f))
            {
                SetFontSample();
            }
        }
    }
}
