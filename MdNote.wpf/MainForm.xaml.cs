﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Reflection;

namespace MdNote.wpf
{
    public partial class MainForm : Window
    {
        NoteManager _NoteManager = new NoteManager();
        Note _CurrentNote = null;
        Option _Option = new Option();

        public MainForm()
        {
            InitializeComponent();

            this.Title = this.Title + "  ver" + GetVersion();
            _Option.Data = new Settings().AppSettings;

            this.Width = _Option.Data.Width;
            this.Height = _Option.Data.Height;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_Option.Data.Maximized)
            {
                this.WindowState = WindowState.Maximized;
            }

            _NoteManager.Items = new NoteManagerFile().read();
            ReflashNoteManagerListBox();
        }

        private string GetVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return String.Format("{0}.{1}", v.Major, v.Minor);
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            string id = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            _CurrentNote = new Note();
            _CurrentNote.Id = id;
            _CurrentNote.Title = "new note";
            _CurrentNote.FileName = id + ".md";

            editBox.Text = _CurrentNote.Body;
            editBox.Focus();
        }

        private void editBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_CurrentNote == null) { return; }
            if (!editBox.IsEnabled) { editBox.IsEnabled = true; }
            _CurrentNote.Body = editBox.Text;

            string before = _CurrentNote.Title;
            string after = GetTitle(editBox.Text);

            if (after == null)
            {
                ResetNoteManager();
                ReflashNoteManagerListBox();
            }
            else if (!before.Equals(after))
            {// テキスト変更後にタイトルが変わっていた場合
                _CurrentNote.Title = GetTitle(editBox.Text);
                ResetNoteManager();
                ReflashNoteManagerListBox();

                new NoteManagerFile().write(_NoteManager);
            }

            MarkdownSharp.Markdown md = new MarkdownSharp.Markdown();
            string html = "";
            html += @"<link href=""" + _Option.Data.CssUrl + @""" rel=""stylesheet""></link>";

            string p = AppDomain.CurrentDomain.BaseDirectory + @"css\mdnote.css"; ;
            if (System.IO.File.Exists(p))
            {
                html += @"<link href=""" + new Uri(p) + @""" rel=""stylesheet""></link>";
            }

            html += md.Transform(editBox.Text);

            p = AppDomain.CurrentDomain.BaseDirectory + @".notes\temp.html";
            System.IO.StreamWriter sr = new System.IO.StreamWriter(
                p,
                false,
                System.Text.Encoding.GetEncoding("utf-8"));
            sr.Write(html);
            sr.Close();
            webBrowser.Navigate(new Uri(p));

            if (_CurrentNote.IsSave)
            {
                SaveCurrentNote(_CurrentNote);
            }
            else
            {
                _CurrentNote.IsSave = !_CurrentNote.IsSave;
            }
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

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedIndex == -1) { return; }
            string id = null;
            if (_CurrentNote != null) { id = _CurrentNote.Id; }
            _CurrentNote = (Note)_NoteManager.Items[listBox1.SelectedIndex];
            if (_CurrentNote.Id.Equals(id)) { return; }

            LoadCurrentNote(_CurrentNote);

            _CurrentNote.IsSave = false;
            editBox.IsEnabled = true;
            editBox.Text = _CurrentNote.Body;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings set = new Settings();

            bool update = false;

            if (this.WindowState == WindowState.Maximized)
            {// 最大化状態
                if (!set.AppSettings.Maximized)
                {
                    set.AppSettings.Maximized = true;
                    update = true;
                }
            }
            else
            {// 最大化以外
                if (set.AppSettings.Maximized)
                {
                    set.AppSettings.Maximized = false;
                    update = true;
                }

                if (set.AppSettings.Width != this.Width ||
                    set.AppSettings.Height != this.Height)
                {// Windows サイズが変化している
                    set.AppSettings.Width = (int)this.Width;
                    set.AppSettings.Height = (int)this.Height;

                    update = true;
                }
            }

            if (update) { set.write(); }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_CurrentNote == null) { return; }
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
            editBox.Text = "";
            webBrowser.Source = new Uri("about:blank");
            editBox.IsEnabled = false;
            ReflashNoteManagerListBox();
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if (_CurrentNote == null) { return; }
            NoteManager nnm = new NoteManager();
            Note buffer = null;

            foreach (Note item in _NoteManager.Items)
            {
                if (item.Id.Equals(_CurrentNote.Id))
                {
                    nnm.Items.Add(item);
                }
                else
                {
                    if (buffer != null) { nnm.Items.Add(buffer); }
                    buffer = item;
                }
            }

            nnm.Items.Add(buffer);

            _NoteManager = nnm;
            new NoteManagerFile().write(_NoteManager);
            ReflashNoteManagerListBox();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if (_CurrentNote == null) { return; }
            NoteManager nnm = new NoteManager();
            Note buffer = null;

            foreach (Note item in _NoteManager.Items)
            {
                if (item.Id.Equals(_CurrentNote.Id))
                {
                    buffer = item;
                }
                else
                {
                    nnm.Items.Add(item);
                    if (buffer != null)
                    {
                        nnm.Items.Add(buffer);
                        buffer = null;
                    }
                }
            }

            if (buffer != null)
            {
                nnm.Items.Add(buffer);
            }

            _NoteManager = nnm;
            new NoteManagerFile().write(_NoteManager);
            ReflashNoteManagerListBox();
        }

        private void SetupButton_Click(object sender, RoutedEventArgs e)
        {
            bool? result = _Option.show();
            if (result == true)
            {
                Settings set = new Settings();
                set.AppSettings = _Option.Data;
                set.write();

                //TextBox.Font = new Font(_Option.Data.FontName, _Option.Data.FontSize);
                //if (_Option.WordWrap)
                //{
                //    azukiControl1.ViewType = Sgry.Azuki.ViewType.WrappedProportional;
                //    azukiControl1.ViewWidth = azukiControl1.ClientSize.Width;
                //}
                //else
                //{
                //    azukiControl1.ViewType = Sgry.Azuki.ViewType.Proportional;
                //}
            }
        }
    }
}
