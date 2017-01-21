using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaLib.UI
{
    public partial class LibraryView : Form
    {
        public LibraryView()
        {
            InitializeComponent();
            loadListView();
        }

        private void loadListView()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (IO.IMediaRootManager mgr in Lib.MediaLib.instance.getRootMgrs())
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Name = mgr.Prefix;
                lvi.Text = mgr.Name;
                lvi.SubItems.Add(mgr.type);
                lvi.SubItems.Add(mgr.rootPath);
                listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                button2.Enabled = true;
            }
        }


        //remove root;
        private void button2_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem item in listView1.SelectedItems)
            {
                Lib.MediaLib.instance.removeRoot(item.Name);
            }
            loadListView();
        }

        //add root
        private void button1_Click(object sender, EventArgs e)
        {
            AddRootForm art = new AddRootForm();
            art.ShowDialog();
            loadListView();

        }
    }
}
