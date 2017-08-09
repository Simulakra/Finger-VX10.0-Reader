using BioMetrixCore;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DoorService
{
    class Program
    {
        static void Main(string[] args)
        {
         ZkemClient objZkeeper;
         bool IsDeviceConnected = false;
            SerialPort ss = new SerialPort("COM1");
            try
            {
                Console.WriteLine("Bağlantı Kuruluyor.\nIP: 192.168.2.55:4370");

                string ipAddress = "192.168.2.55";
                int portNumber = 4370;

                bool isValidIpA = PingTheDevice(ipAddress);
                if (!isValidIpA)
                    throw new Exception(ipAddress + ":" + portNumber + " IP adresindeki cihaz yanıt vermiyor!!");

                objZkeeper = new ZkemClient(RaiseDeviceEvent);
                IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber);

                if (IsDeviceConnected)
                {
                    string deviceInfo = FetchDeviceInfo(objZkeeper, 1);
                    Console.WriteLine("Bağlantı Kuruldu - " + deviceInfo);
                }

                try
                {
                    while (true)
                    {
                        MachineInfo lstMachineInfo = GetLogData(objZkeeper, 1);
                        if (lstMachineInfo != null)
                        {

                            double tolerans = 500;
                            DateTime tNow = DateTime.Now;
                            DateTime tSon = Convert.ToDateTime(lstMachineInfo.DateTimeRecord);
                            if (tNow.AddMilliseconds(-1 * tolerans) < tSon)
                            {
                                ss.Write("0");
                                Console.WriteLine("KAPI AÇ");
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Program kendi çıkışını verdi");
            Console.ReadKey();
        }
        public static bool PingTheDevice(string ipAdd)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ipAdd);

                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;

                // Create a buffer of 32 bytes of data to be transmitted. 
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                PingReply reply = pingSender.Send(ipAddress, timeout, buffer, options);

                if (reply.Status == IPStatus.Success)
                    return true;
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case "Disconnected":
                    {
                        Console.WriteLine("Cihaz kapalı");
                        break;
                    }

                default:
                    break;
            }

        }

        public static string FetchDeviceInfo(ZkemClient objZkeeper, int machineNumber)
        {
            StringBuilder sb = new StringBuilder();

            string returnValue = string.Empty;


            objZkeeper.GetFirmwareVersion(machineNumber, ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Firmware V: ");
                sb.Append(returnValue);
                sb.Append(",");
            }


            returnValue = string.Empty;
            objZkeeper.GetVendor(ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Vendor: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            string sWiegandFmt = string.Empty;
            objZkeeper.GetWiegandFmt(machineNumber, ref sWiegandFmt);

            returnValue = string.Empty;
            objZkeeper.GetSDKVersion(ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("SDK V: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            returnValue = string.Empty;
            objZkeeper.GetSerialNumber(machineNumber, out returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Serial No: ");
                sb.Append(returnValue);
                sb.Append(",");
            }

            returnValue = string.Empty;
            objZkeeper.GetDeviceMAC(machineNumber, ref returnValue);
            if (returnValue.Trim() != string.Empty)
            {
                sb.Append("Device MAC: ");
                sb.Append(returnValue);
            }

            return sb.ToString();
        }

        public static MachineInfo GetLogData(ZkemClient objZkeeper, int machineNumber)
        {
            string dwEnrollNumber1 = "";
            int dwVerifyMode = 0;
            int dwInOutMode = 0;
            int dwYear = 0;
            int dwMonth = 0;
            int dwDay = 0;
            int dwHour = 0;
            int dwMinute = 0;
            int dwSecond = 0;
            int dwWorkCode = 0;

            MachineInfo lstEnrollData = new MachineInfo();

            objZkeeper.ReadAllGLogData(machineNumber);

            while (objZkeeper.SSR_GetGeneralLogData(machineNumber, out dwEnrollNumber1, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))


            {
                string inputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();

                MachineInfo objInfo = new MachineInfo();
                objInfo.MachineNumber = machineNumber;
                objInfo.IndRegID = int.Parse(dwEnrollNumber1);
                objInfo.dwVerifyMode = dwVerifyMode;
                objInfo.dwInOutMode = dwInOutMode;
                objInfo.DateTimeRecord = inputDate;

                lstEnrollData = objInfo;
            }

            return lstEnrollData;
        }
    }
}
