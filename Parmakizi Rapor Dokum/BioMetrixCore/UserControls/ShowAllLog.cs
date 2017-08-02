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
            try
            {
                master.ShowStatusBar(string.Empty, true);

                lstMachineInfo = manipulator.GetLogData(master.objZkeeper, int.Parse(master.machineNumber.Trim()));
                lstFingerPrintTemplates = manipulator.GetAllUserInfo(master.objZkeeper, int.Parse(master.machineNumber.Trim()));

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    foreach(MachineInfo item in lstMachineInfo)
                    {
                        string g_kisi = item.IndRegID.ToString();
                        string g_cesit = item.dwInOutMode.ToString();
                        string g_yontem = item.dwVerifyMode.ToString();

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
                    master.ShowStatusBar(lstMachineInfo.Count + " kayıt bulundu!", true);
                }
                else
                    master.ShowStatusBar("Hiç kayıt bulunamadı", false);
            }
            catch (Exception ex)
            {
                master.ShowStatusBar(ex.Message, false);
            }
        }
    }
}
