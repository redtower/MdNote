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
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _NoteManager.Items = new NoteManagerFile().read();
            ReflashNoteManagerListBox();
            SetSplitterSize();
        }

        private void azukiControl1_TextChanged(object sender, EventArgs e)
        {
            if (_CurrentNote == null) { return; }
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
            }

            MarkdownSharp.Markdown md = new MarkdownSharp.Markdown();
            webBrowser1.DocumentText = md.Transform(azukiControl1.Text);

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
            if (_CurrentNote != null)
            {
                id = _CurrentNote.Id;
                SaveCurrentNote(_CurrentNote);
                new NoteManagerFile().write(_NoteManager);
            }
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
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            SaveCurrentNote(_CurrentNote);

            string id = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            _CurrentNote = new Note();
            _CurrentNote.Id = id;
            _CurrentNote.Title = "new note";
            _CurrentNote.FileName = id + ".md";

            azukiControl1.Enabled = true;
            azukiControl1.Text = "";
        }

        private void SaveCurrentNote(Note note)
        {
            if (note == null) { return; }

            NoteFile nf = new NoteFile(note);
            nf.write();
        }

        private void LoadCurrentNote(Note note)
        {
            if (note == null) { return; }

            NoteFile nf = new NoteFile(note);
            note.Body = nf.read();
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
    }
}