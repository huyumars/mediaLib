using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaLib
{
    public partial class Form1 : Form
    {
        UI.MediaListViewHelper listHelper;
        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            listHelper = UI.MediaListViewHelper.instance;
            listHelper.setListView(listView1);
            if (!listHelper.loadList())
            {
                ListViewLoadFailed();
            }
            toolStripComboBox1.SelectedIndex = 0;

        }

        private void ListViewLoadFailed()
        {
            Logger.ERROR("list view load failed, disable everything");
            MessageBox.Show("can not load import configuration file, please check the log file");
            listView1.Enabled = false;
            menuStrip1.Enabled = false;
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listHelper.reSort(e.Column);   
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count >0 ) 
                listHelper.doubleClick(listView1.SelectedIndices[0]);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
            // no input 
            if (textBox1.Text == null || textBox1.Text.Length == 0)
                listHelper.viewfilter = null;

            String inputStr = textBox1.Text;
            // some input
            listHelper.viewfilter = new UI.MediaFilter((Lib.Media media) =>
            {
                if (media.contentDir.ToLower().Contains(inputStr.ToLower()))
                    return true;
                return false;
            });
            listHelper.reloadVirtualData();
        }



        //menu strip events

        private void mediaLibrariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UI.LibraryView lv = new UI.LibraryView();
            lv.ShowDialog();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (toolStripComboBox1.SelectedIndex)
            {
                case 0: listHelper.changeView(View.LargeIcon); break;
                case 1: listHelper.changeView(View.SmallIcon); break;
                case 2: listHelper.changeView(View.Details); break;
            }
        }

        private void clearCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Resource.ImgManager.ClearUseLessImg();
            MessageBox.Show("clear finished", "ok");
        }

        private void addMediaDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UI.AddRootForm addRootForm = new UI.AddRootForm();
            addRootForm.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save everything
            Lib.MediaLib.instance.saveRootManagers();
        }
    }
}
