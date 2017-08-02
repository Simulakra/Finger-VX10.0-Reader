using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BioMetrixCore.UserControls
{
    public partial class ShowUsers : UserControl
    {
        MainForm master;
        DeviceManipulator manipulator = new DeviceManipulator();
        ICollection<UserInfo> lstFingerPrintTemplates;
        public ShowUsers(Form m)
        {
            master = (MainForm)m;
            InitializeComponent();
        }

        private void ShowUsers_Load(object sender, EventArgs e)
        {
            try
            {
                master.ShowStatusBar(string.Empty, true);

                lstFingerPrintTemplates = manipulator.GetAllUserInfo(master.objZkeeper, int.Parse(master.machineNumber.Trim()));
                if (lstFingerPrintTemplates != null && lstFingerPrintTemplates.Count > 0)
                {
                    foreach (UserInfo item in lstFingerPrintTemplates)
                    {
                        listBox1.Items.Add(item.EnrollNumber + " - " + item.Name);
                    }
                    master.ShowStatusBar(lstFingerPrintTemplates.Count + " kayıt bulundu!", true);
                }
                else
                    master.ShowStatusBar("Hiç kayıt bulunamadı",false);
            }
            catch (Exception ex)
            {
                master.ShowStatusBar(ex.Message,false);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            lb_data.Text = "";
            if(listBox1.SelectedIndex!=-1)
            {
                int i = 0;
                foreach (UserInfo item in lstFingerPrintTemplates)
                {
                    if (i != listBox1.SelectedIndex) i++;
                    else
                    {
                        lb_data.Text += item.EnrollNumber + "\n";
                        lb_data.Text += item.Name + "\n";
                        lb_data.Text += item.FingerIndex + "\n";
                        lb_data.Text += item.Password + "\n";
                        break;
                    }
                }
            }

        }
    }
}
