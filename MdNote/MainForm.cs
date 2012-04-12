using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MdNote
{
    public partial class MainForm : Form
    {
        NoteManager _NoteManager = new NoteManager();
        Note _CurrentNote = null;

        public MainForm()
        {
            InitializeComponent();

            Sgry.Azuki.FontInfo fontInfo = new Sgry.Azuki.FontInfo();
            fontInfo.Name = "MS UI Gothic";
            fontInfo.Size = 9;
            fontInfo.Style = System.Drawing.FontStyle.Regular;
            this.azukiControl1.FontInfo = fontInfo;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + "  ver" + GetVersion();

            _NoteManager.Items = new NoteManagerFile().read();
            ReflashNoteManagerListBox();
            SetSplitterSize();
        }

        private string GetVersion()
        {
            string v = Application.ProductVersion;
            return v.Substring(0, v.Length - ".0.0".Length);
        }

        private void azukiControl1_TextChanged(object sender, EventArgs e)
        {
            if (_CurrentNote == null) { return; }
            if ( !azukiControl1.Enabled ) { azukiControl1.Enabled = true; }
            _CurrentNote.Body = azukiControl1.Text;

            string before = _CurrentNote.Title;
            string after = GetTitle(azukiControl1.Text);

            if (after == null)
            {
                ResetNoteManager();
                ReflashNoteManagerListBox();
            }
            else if (!before.Equals(after))
            {// テキスト変更後にタイトルが変わっていた場合
                _CurrentNote.Title = GetTitle(azukiControl1.Text);
                ResetNoteManager();
                ReflashNoteManagerListBox();

                new NoteManagerFile().write(_NoteManager);
            }

            MarkdownSharp.Markdown md = new MarkdownSharp.Markdown();
            string html = "";
            html += md.Transform(azukiControl1.Text);
            webBrowser1.DocumentText = html;

            SaveCurrentNote(_CurrentNote);
        }

        private string GetTitle(string text)
        {
            string title = null;

            if (!string.IsNullOrEmpty(text))
            {
                foreach (string line in text.Split('\n'))
                {
                    title = Regex.Replace(line, @"\r", "");
                    if (!string.IsNullOrEmpty(title)) { break; }
                }
            }

            return title;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1) { return; }
            string id = null;
            if (_CurrentNote != null) { id = _CurrentNote.Id; }
            _CurrentNote = (Note)_NoteManager.Items[listBox1.SelectedIndex];
            if (_CurrentNote.Id.Equals(id)) { return; }

            LoadCurrentNote(_CurrentNote);

            azukiControl1.Enabled = true;
            azukiControl1.Text = _CurrentNote.Body;
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveCurrentNote(_CurrentNote);

            ResetNoteManager();
            ReflashNoteManagerListBox();

            new NoteManagerFile().write(_NoteManager);
        }

        private void ResetNoteManager()
        {
            bool isNew = true;
            foreach (Note item in _NoteManager.Items)
            {
                if (item.Id.Equals(_CurrentNote.Id))
                {
                    item.Body = _CurrentNote.Body;
                    isNew = false;
                    break;
                }
            }

            if (isNew)
            {
                _NoteManager.Items.Add(_CurrentNote);
            }
        }

        private void ReflashNoteManagerListBox()
        {
            listBox1.Items.Clear();
            foreach (Note item in _NoteManager.Items)
            {
                listBox1.Items.Add(item.Title);
            }
            if (_CurrentNote == null) { return; }
            for (int i = 0; i < _NoteManager.Items.Count; i++)
            {
                if (((Note)_NoteManager.Items[i]).Id == _CurrentNote.Id)
                {
                    listBox1.SelectedIndex = i;
                }
            }
            listBox1.ItemHeight = 38;
        }

        private void SaveCurrentNote(Note note)
        {
            if (note == null) { return; }

            new NoteFile().write(note);
        }

        private void LoadCurrentNote(Note note)
        {
            if (note == null) { return; }

            new NoteFile().read(note);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            SetSplitterSize();
        }

        private void SetSplitterSize()
        {
            int s2w = splitContainer2.Size.Width;
            int s1w = splitContainer1.Size.Width;
            if (s2w == 0 || s1w == 0) { return; }

            splitContainer2.SplitterDistance = s2w / 5;
            splitContainer1.SplitterDistance = s1w / 2;
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.Graphics.PageUnit = GraphicsUnit.Pixel;
            SizeF size = e.Graphics.MeasureString(
                listBox1.Items[e.Index].ToString(),
                listBox1.Font,
                listBox1.ClientSize.Width);
            e.ItemWidth = Convert.ToInt32(size.Width);
            e.ItemHeight = Convert.ToInt32(size.Height);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index == -1) { return; }
            e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            e.Graphics.DrawString(
                listBox1.Items[e.Index].ToString(),
                listBox1.Font,
                new SolidBrush(e.ForeColor),
                e.Bounds);
        }

        private void listBox1_Resize(object sender, EventArgs e)
        {
            ReflashNoteManagerListBox();
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            string id = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            _CurrentNote = new Note();
            _CurrentNote.Id = id;
            _CurrentNote.Title = "new note";
            _CurrentNote.FileName = id + ".md";

            azukiControl1.Text = _CurrentNote.Body;
        }

        private void DeleteToolStripButton2_Click(object sender, EventArgs e)
        {
            foreach (Note item in _NoteManager.Items)
            {
                if (item.Id.Equals(_CurrentNote.Id))
                {
                    _NoteManager.Items.Remove(item);
                    break;
                }
            }
            new NoteManagerFile().write(_NoteManager);
            new NoteFile().trash(_CurrentNote);

            _CurrentNote = null;
            azukiControl1.Text = "";
            webBrowser1.DocumentText = "";
            azukiControl1.Enabled = false;
            ReflashNoteManagerListBox();
        }

        private void UpToolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void DownToolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void SettingToolStripButton5_Click(object sender, EventArgs e)
        {

        }
    }
}
