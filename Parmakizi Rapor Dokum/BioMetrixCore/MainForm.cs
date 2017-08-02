using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BioMetrixCore
{
    public partial class MainForm : Form
    {
        public string machineNumber = "1";
        public MainForm()
        {
            InitializeComponent();
            ArrayList menü = new ArrayList();
            menü.Add(MakeMenu("Kullanıcılar", new UserControls.ShowUsers(this)));
            menü.Add(MakeMenu("Kayıtlar", new UserControls.ShowAllLog(this)));
            int pad = 3;
            for (int i = 0; i < menü.Count; i++)
            {
                Button temp = (Button)menü[i];
                temp.Left = pad;
                temp.Width = pn_menu.Width - pad*2;
                temp.Height = 40;
                temp.Top = (pad + temp.Height) * i + pad;
                pn_menu.Controls.Add(temp);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //tsm_baglan_Click(sender, e);
        }

        private Button MakeMenu(string text, UserControl form)
        {
            Button temp = new Button();
            temp.Text = text;
            temp.Tag = form;
            temp.Click += MenuButtonClick;
            return temp;
        }

        private void MenuButtonClick(object sender, EventArgs e)
        {
            UserControl temp = (UserControl)((Button)sender).Tag;
            pn_ana.Controls.Clear();
            temp.Dock = DockStyle.Fill;
            pn_ana.Controls.Add(temp);
        }

        private void tsm_baglan_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ShowStatusBar("Bağlantı Kuruluyor.", true);

                if (IsDeviceConnected)
                {
                    IsDeviceConnected = false;
                    this.Cursor = Cursors.Default;
                    //enable device
                    if (checkBox1.Checked)
                    {
                        bool deviceEnabled = objZkeeper.EnableDevice(int.Parse(toolStripTextBox3.Text.Trim()), true);
                    }
                    return;
                }

                string ipAddress = toolStripTextBox1.Text.Trim();
                string port = toolStripTextBox2.Text.Trim();
                if (ipAddress == string.Empty || port == string.Empty)
                    throw new Exception("The Device IP Address and Port is mandotory !!");

                int portNumber = 4370;
                if (!int.TryParse(port, out portNumber))
                    throw new Exception("Not a valid port number");

                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The Device IP is invalid !!");

                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The device at " + ipAddress + ":" + port + " did not respond!!");

                objZkeeper = new ZkemClient(RaiseDeviceEvent);
                IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber);

                if (IsDeviceConnected)
                {
                    string deviceInfo = manipulator.FetchDeviceInfo(objZkeeper, int.Parse(toolStripTextBox3.Text.Trim()));
                    this.Text = "Ana Menü - " + deviceInfo;
                }
                machineNumber = toolStripTextBox3.Text;

                //disable device
                if (checkBox1.Checked)
                {
                    bool deviceDisabled = objZkeeper.DisableDeviceWithTimeOut(int.Parse(toolStripTextBox3.Text.Trim()), 3000);
                }
            }
            catch (Exception ex)
            {
                ShowStatusBar(ex.Message, false);
            }
            this.Cursor = Cursors.Default;
        }

        #region HelperMethods
        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;

        public void ShowStatusBar(string text, bool safe = true)
        {
            lb_info.Text = text;
            lb_info.ForeColor = (safe) ? Color.Green : Color.Red;
        }

        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    ShowStatusBar("Aygıta bağlandı!", true);
                    tsm_baglan.Text = "Bağlantı Kes";
                    ToggleControls(true);
                }
                else
                {
                    ShowStatusBar("Aygıtla bağlantı kesildi!", true);
                    objZkeeper.Disconnect();
                    tsm_baglan.Text = "Bağlan";
                    ToggleControls(false);
                }
            }
        }

        private void ToggleControls(bool enabled)
        {
            pn_menu.Enabled = enabled;
            toolStripTextBox1.Enabled = toolStripTextBox2.Enabled = toolStripTextBox3.Enabled = checkBox1.Enabled = !enabled;
            toolStripTextBox1.Size =new Size(100 - ((enabled) ? 30 : 0),toolStripTextBox1.Height);
            pn_ana.Controls.Clear();
        }

        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        ShowStatusBar("Cihaz kapalı", true);
                        tsm_baglan.Text = "Bağlan";
                        ToggleControls(false);
                        break;
                    }

                default:
                    break;
            }

        }
        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDeviceConnected)
            {
                IsDeviceConnected = false;
                this.Cursor = Cursors.Default;
                //enable device
                if (checkBox1.Checked)
                {
                    bool deviceEnabled = objZkeeper.EnableDevice(int.Parse(toolStripTextBox3.Text.Trim()), true);
                }
            }
        }
    }
}
