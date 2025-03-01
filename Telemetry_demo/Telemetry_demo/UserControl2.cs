using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;


namespace Telemetry_demo
{
    
    public partial class UserControl2 : UserControl
    {
        private TcpClient client;
        private NetworkStream stream;
        public UserControl2()
        {
            InitializeComponent();
        }

        private void btnWirelessConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string serverIP = tbIP.Text; // ESP32 IP address
                int port = 80; // ESP32 port number (80 in this example)

                client = new TcpClient(serverIP, port);
                stream = client.GetStream();

                MessageBox.Show("Connected to ESP32", "Connection Status", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Start receiving data in the background
                ReceiveData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed: " + ex.Message);
            }
        }
        private async void ReceiveData()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    // Update the TextBox with received data from ESP32
                    Invoke(new Action(() =>
                    {
                        tbreceived.AppendText("Received: " + receivedData + Environment.NewLine);
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error receiving data: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the connection when the form is closed
            if (client != null)
            {
                stream.Close();
                client.Close();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSend_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    string message = tbSend.Text;
                    byte[] data = Encoding.ASCII.GetBytes(message);

                    // Send the message to ESP32
                    stream.Write(data, 0, data.Length);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending data: " + ex.Message);
            }
        }

        private void btnShowList_Click(object sender, EventArgs e)
        {
            listBoxConfig.Items.Clear();
            
            foreach (var config in sharedInputs.configs)
            {

                string configDisplay = $"Input Name: {config.InputName}, Baud Rate: {config.BaudRate}, COM Port: {config.ComPort}";

                listBoxConfig.Items.Add(configDisplay);// Add each config to the ListBox
            }
        }
    }
}
