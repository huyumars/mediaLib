using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MediaLib.UI
{
    

    class MediaEditorViewController
    {
        private bool ifsave = false;
        private MediaEditor mediaEditorForm = new MediaEditor();
        int xOffset = 20;
        int yOffset = 50;
        int xInputOffset = 150;
        int textBoxLength = 450;
        const int yStep = 40;

        void aaa(String type) { }
        private System.Windows.Forms.ComboBox getEnumComboBox(Object media, PropertyInfo property ) 
        {
            System.Windows.Forms.ComboBox cb = new System.Windows.Forms.ComboBox();
            foreach (var item in Enum.GetValues(property.PropertyType))
            {
                cb.Items.Add(item.ToString());
            }
            cb.SelectedIndex = (int)property.GetValue(media, null);
            cb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb.HandleDestroyed += new EventHandler((object sender, EventArgs e)=> {
                if(ifsave)
                    property.SetValue(media, cb.SelectedIndex);
            });
            return cb;
        }

        private System.Windows.Forms.TextBox getStringInputControl(Object media, PropertyInfo property)
        {
            System.Windows.Forms.TextBox tb = new System.Windows.Forms.TextBox();
            tb.Text = (String)property.GetValue(media, null);
            tb.Width = textBoxLength;
            tb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top)|
            System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            tb.HandleDestroyed += new EventHandler((object sender, EventArgs e) => {
                if(ifsave)
                    property.SetValue(media, tb.Text);
            });
            return tb;
        }
        private void addItem(PropertyInfo property, Lib.Media media)
        {
            System.Windows.Forms.Label label= new System.Windows.Forms.Label();
            label.Text = property.Name;
            label.Location = new System.Drawing.Point(xOffset, yOffset);
            mediaEditorForm.Controls.Add(label);
            System.Windows.Forms.Control inputControl = null;
            if (property.GetValue(media, null) is Enum)
            {
                inputControl = getEnumComboBox(media, property);
            }
            else if (property.GetValue(media, null) is string)
            {
                inputControl = getStringInputControl(media, property);
            }
            if (inputControl != null)
            {
                if (_disableItem!=null&&_disableItem.Contains(property.Name))
                    inputControl.Enabled = false;
                inputControl.Location = new System.Drawing.Point(xInputOffset, yOffset);
                mediaEditorForm.Controls.Add(inputControl);
            }
           
            yOffset += yStep;
        }

        private void addSaveBtn()
        {
            System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
            btn.Location = new System.Drawing.Point(mediaEditorForm.Size.Width/2- btn.Size.Width/2, yOffset+20);
            btn.Height = 30;
            btn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            btn.Text = "Save";
            mediaEditorForm.Controls.Add(btn);
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            ifsave = true;
            mediaEditorForm.Close();
            mediaEditorForm.Dispose();
        }
        private string[] _disableItem;
        public MediaEditorViewController(Lib.Media media, string[] disableItem=null)
        {
            _disableItem = disableItem;
            var type = media.GetType();
            string str = Config.JSONHelper.Serialize(media);
            JObject o = Config.JSONHelper.Deserialize<JObject>(str);
            foreach(var i in o)
            {
                addItem(  type.GetProperty(i.Key), media);
            }
            addSaveBtn();
        }

        public void show()
        {
            mediaEditorForm.ShowDialog();
        }
    }
}
