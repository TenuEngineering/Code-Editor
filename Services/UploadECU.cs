using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ECUCodeEditor.Services
{
    public class UploadECU
    {
        public SerialPort serialPort;
        public string ConnectedPort { get; private set; } = "";
        public bool ConnInfo { get; private set; } = false;
        public Dictionary<string, string> ReceivedData = new Dictionary<string, string>();
        public Dictionary<string, (string, int)> veriDict = new Dictionary<string, (string, int)>();


        public async Task ConnectPort()
        {
            string[] allPorts = SerialPort.GetPortNames();
            int baudrate = 115200;

            foreach (string port in allPorts)
            {
                try
                {
                    serialPort = new SerialPort(port, baudrate)
                    {
                        ReadTimeout = 70,
                        WriteTimeout = 70
                    };
                    serialPort.Open();
                    ConnectedPort = port;

                    bool result = FindSerialPort();

                    if (result)
                    {
                        ConnInfo = true;
                        Console.WriteLine($"Port {port} ile bağlantı kuruldu.");
                        break;
                    }
                    else
                    {
                        serialPort.Close();
                        ConnInfo = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Port {port} için hata: {ex.Message}");
                    ConnInfo = false;
                }
            }

            if (!ConnInfo)
            {
                Console.WriteLine("Bağlantı başarısız.");
            }
        }
        public void DisconnectCart()
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort.Dispose();
                }

                ReceivedData.Clear();
                ConnectedPort = "";
                ConnInfo = false;

                Console.WriteLine("Bağlantı kesildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bağlantı kesme hatası: " + ex.Message);
            }
        }
        private bool FindSerialPort()
        {
            try
            {
                int sayac = 1;

                for (int i = 0; i < 4; i++)
                {
                    if (sayac == 3)
                    {
                        sayac++;
                        continue;
                    }

                    string input_data = $"58 153 204 145 {sayac} 1 {veriDict[$"145 {sayac}"].Item2} 0";
                    byte[] dataSend = CreateCRCandData(input_data, sayac, 145, veriDict[$"145 {sayac}"].Item2);
                    SendMessage(dataSend);
                    string res = ReceiveMessage();

                    if (GetAndControlCRC(res))
                    {
                        string[] parts = res.Split(' ');
                        string dataName1 = HexToDecimal(parts[3]);
                        string dataName2 = HexToDecimal(parts[4]);
                        int parameter = Convert.ToInt32(HexToDecimal(parts[5]));

                        string reciveData = "";
                        for (int j = 0; j < parameter; j++)
                        {
                            reciveData += parts[6 + j] + " ";
                        }

                        ReceivedData[$"{dataName1} {dataName2}"] = reciveData.Trim();
                        sayac++;
                    }
                    else
                    {
                        Console.WriteLine("Veri yanlış geldi.");
                        return false;
                    }
                }

                // 129 komutlu veriler
                sayac = 1;
                for (int i = 0; i < 3; i++)
                {
                    string input_data = $"58 153 204 129 {sayac} 0 0 0";
                    byte[] dataSend = CreateCRCandData(input_data, sayac, 129, veriDict[$"129 {sayac}"].Item2);
                    SendMessage(dataSend);
                    string res = ReceiveMessage();

                    if (GetAndControlCRC(res))
                    {
                        string[] parts = res.Split(' ');
                        string dataName1 = HexToDecimal(parts[3]);
                        string dataName2 = HexToDecimal(parts[4]);
                        int parameter = Convert.ToInt32(HexToDecimal(parts[5]));

                        string reciveData = "";
                        for (int j = 0; j < parameter; j++)
                        {
                            reciveData += parts[6 + j] + " ";
                        }

                        ReceivedData[$"{dataName1} {dataName2}"] = reciveData.Trim();
                        sayac++;
                    }
                    else
                    {
                        Console.WriteLine("Veri yanlış geldi.");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("FindSerialPort Hatası: " + ex.Message);
                return false;
            }
        }

        public void SendMessage(byte[] dataToSend)
        {
            try
            {
                serialPort.Write(dataToSend, 0, dataToSend.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Veri gönderme hatası: " + ex.Message);
            }
        }

        public string ReceiveMessage()
        {
            try
            {
                List<byte> response = new List<byte>();
                var startTime = DateTime.Now;

                while ((DateTime.Now - startTime).TotalMilliseconds < 500)
                {
                    if (serialPort.BytesToRead > 0)
                    {
                        int readByte = serialPort.ReadByte();
                        response.Add((byte)readByte);
                    }
                }

                return BitConverter.ToString(response.ToArray()).Replace("-", " ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Veri alma hatası: " + ex.Message);
                return "";
            }
        }

        public byte[] DataToByte(string inputData)
        {
            var parts = inputData.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
            var byteList = new List<byte>();

            foreach (var part in parts)
            {
                byteList.Add(Convert.ToByte(part));
            }

            return byteList.ToArray();
        }

        public bool GetAndControlCRC(string veri)
        {
            try
            {
                string[] parts = veri.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 10) return false;

                int parameter = Convert.ToInt32(parts[5], 16);
                string reciveData = "";

                for (int j = 0; j < parameter; j++)
                {
                    reciveData += parts[6 + j];
                }

                int crcStartIndex = 6 + parameter;
                string crcData = string.Join("", parts.Skip(crcStartIndex).Take(4));

                int totalParam = 6 + parameter;
                int requiredPadding = (int)Math.Ceiling(totalParam / 4.0) * 4 - totalParam;

                var dataParts = parts.Take(totalParam).ToList();
                for (int i = 0; i < requiredPadding; i++)
                    dataParts.Add("00");

                List<int> decimalValues = dataParts.Select(x => Convert.ToInt32(x, 16)).ToList();
                byte[] mpeg2_data = decimalValues.Select(x => (byte)x).ToArray();

                uint crc = CalculateCustomCRC32(mpeg2_data, 0x04C11DB7);
                string crcHex = crc.ToString("X8");

                return string.Equals(crcData, crcHex, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Console.WriteLine("CRC kontrol hatası: " + ex.Message);
                return false;
            }
        }

        public uint CalculateCustomCRC32(byte[] data, uint polynomial)
        {
            uint crc = 0xFFFFFFFF;

            foreach (byte b in data)
            {
                crc ^= (uint)(b << 24);

                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x80000000) != 0)
                        crc = (crc << 1) ^ polynomial;
                    else
                        crc <<= 1;
                }
            }

            return crc & 0xFFFFFFFF;
        }

        public byte[] CreateCRCandData(string inputData, int sayac, int komut, int byteLength)
        {
            byte[] data = DataToByte(inputData);
            uint crc = CalculateCustomCRC32(data, 0x04C11DB7);
            string crcHex = crc.ToString("X8");

            List<byte> crcBytes = new List<byte>();
            for (int i = 0; i < 8; i += 2)
            {
                string hexPair = crcHex.Substring(i, 2);
                crcBytes.Add(Convert.ToByte(hexPair, 16));
            }

            crcBytes.Add(59); // End marker

            List<byte> packet = new List<byte>
            {
                58, 153, 204, (byte)komut, (byte)sayac
            };

            if (komut == 129)
                packet.Add(0);
            else
            {
                packet.Add(1); // param sayısı
                packet.Add((byte)byteLength);
            }

            packet.AddRange(crcBytes);
            return packet.ToArray();
        }

        public string HexToDecimal(string hexInput)
        {
            try
            {
                var parts = hexInput.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                var decimals = parts.Select(hex => Convert.ToInt32(hex, 16));
                return string.Join(" ", decimals);
            }
            catch
            {
                return "";
            }
        }

        public void upload()
        {

        }

    }

}
