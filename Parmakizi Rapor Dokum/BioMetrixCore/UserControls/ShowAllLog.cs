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
    public partial class ShowAllLog : UserControl
    {
        MainForm master;
        DeviceManipulator manipulator = new DeviceManipulator();
        ICollection<MachineInfo> lstMachineInfo;
        ICollection<UserInfo> lstFingerPrintTemplates;
        public ShowAllLog(Form f)
        {
            master = (MainForm)f;
            InitializeComponent();
        }

        private void ShowAllLog_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
            PullAllData();
        }

        private void PullAllData()
        {
            listBox1.Items.Clear();
            try
            {
                master.ShowStatusBar(string.Empty, true);

                lstMachineInfo = manipulator.GetLogData(master.objZkeeper, int.Parse(master.machineNumber.Trim()));
                lstFingerPrintTemplates = manipulator.GetAllUserInfo(master.objZkeeper, int.Parse(master.machineNumber.Trim()));

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    foreach (MachineInfo item in lstMachineInfo)
                    {
                        string g_kisi = item.IndRegID.ToString();
                        string g_cesit = item.dwInOutMode.ToString();
                        string g_yontem = item.dwVerifyMode.ToString();

                        if (radioButton2.Checked && g_yontem == "1") continue;
                        if (radioButton3.Checked && g_yontem == "0") continue;
                        if (!checkedListBox1.GetItemChecked(0) && g_cesit == "0") continue;
                        if (!checkedListBox1.GetItemChecked(1) && g_cesit == "1") continue;
                        if (!checkedListBox1.GetItemChecked(2) && g_cesit == "4") continue;
                        if (!checkedListBox1.GetItemChecked(3) && g_cesit == "5") continue;
                        if(checkBox1.Checked)
                        {
                            DateTime temp = Convert.ToDateTime(item.DateTimeRecord);
                            if (!(monthCalendar1.SelectionStart.Date <= temp && temp <= monthCalendar1.SelectionEnd.Date.AddHours(23).AddMinutes(59))) continue;
                        }

                        if (lstFingerPrintTemplates != null && lstFingerPrintTemplates.Count > 0)
                        {
                            foreach (UserInfo kisi in lstFingerPrintTemplates)
                            {
                                if (g_kisi == kisi.EnrollNumber.ToString())
                                { g_kisi = kisi.Name; break; }
                            }
                        }

                        switch (g_cesit)
                        {
                            case "0": g_cesit = "Giriş"; break;
                            case "1": g_cesit = "Çıkış"; break;
                            case "4": g_cesit = "Fm_Giriş"; break;
                            case "5": g_cesit = "Fm_Çıkış"; break;
                            default: break;
                        }

                        switch (g_yontem)
                        {
                            case "0": g_yontem = "Şifre"; break;
                            case "1": g_yontem = "Parmak İzi"; break;
                            default: break;
                        }

                        listBox1.Items.Add(g_kisi + " - " + g_cesit + " - "
                            + g_yontem + " - " + item.DateTimeRecord);
                    }
                    master.ShowStatusBar(lstMachineInfo.Count + " kayıt bulundu! "+listBox1.Items.Count+" adet kayıt istenilen kriterlerde.", true);
                }
                else
                    master.ShowStatusBar("Hiç kayıt bulunamadı", false);
            }
            catch (Exception ex)
            {
                master.ShowStatusBar(ex.Message, false);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PullAllData();
        }
    }
}
