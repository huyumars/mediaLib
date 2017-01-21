using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaLib.Config;
using MediaLib.Lib;

namespace MediaLib.UI
{
    public partial class AddRootForm : Form
    {
        public AddRootForm()
        {
            InitializeComponent();

            //init ui
            foreach (var item in Enum.GetValues(typeof(Config.MediaType)))
            {
                typeCombox.Items.Add(item.ToString());
            }
            typeCombox.SelectedIndex = 0;
            typeCombox.DropDownStyle = ComboBoxStyle.DropDownList;

            
        }

        public void initWithRootPrefix(String Prefix)
        {

        }

        private void ChoseDirBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Choose The Root";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                dirPathTextBox.Text = foldPath;
            }
        }
        interface IFixDepthRootConfig : IRootConfig, IO.IFixDepthFileTravelerConfig { }
        private void Finished_Click(object sender, EventArgs e)
        {
            String typeStr = typeCombox.Items[typeCombox.SelectedIndex].ToString();
            Config.MediaType mType =(Config.MediaType) Enum.Parse(typeof(Config.MediaType), typeStr);
            Config.IRootConfig config = null;
            try
            {
                String rootCofigName = "MediaLib.Lib." + mType + "RootConfig";
                config = (Config.IRootConfig)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(rootCofigName);
                (config ).name = nameTextBox.Text;
                (config as IO.IFixDepthFileTravelerConfig).dirName = dirPathTextBox.Text;
                (config as IO.IFixDepthFileTravelerConfig).mediaFileExistDirLevel = int.Parse(depthTextBox.Text);
                
                if ((config ).valid())
                {
                    Lib.MediaLib.instance.addRoot(config);
                    this.Close();
                }
                else throw new Exception();
            }
            catch
            {
                MessageBox.Show("this library is invalid, try another one", "Error");
            }
        }

        private void depthTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 8 is backspace
            if(!(Char.IsNumber(e.KeyChar))&& e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }
    }
}
