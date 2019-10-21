using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using MessagingToolkit.QRCode.Codec;
using ZXing;
using ZXing.QrCode;


namespace QR
{
    public partial class KareKod : Form
    {
        public KareKod()
        {
            InitializeComponent();
        }

        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;


        public string textcode
        {
            get { if (textBox1.Text == null) return this.textBox1.Text = null; return this.textBox1.Text; }
            set { this.textBox1.Text = value; }
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Image)eventArgs.Frame.Clone();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo Device in CaptureDevice)
            {
                comboBox1.Items.Add(Device.Name);
            }
            comboBox1.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice();
        }

        private Image qrCreate(string Data, int qrCode)
        {
            QRCodeEncoder qr = new QRCodeEncoder();
            qr.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qr.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            qr.QRCodeVersion = qrCode;

            Bitmap bm = qr.Encode(Data);

            return bm;
        }


        private void button1_Click(object sender, EventArgs e)
        {

            if (FinalFrame.IsRunning == true)
                FinalFrame.Stop();
            pictureBox1.Image = qrCreate(textBox1.Text, 8);
            timer1.Enabled = false;
            button2.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {

                BarcodeReader Reader = new BarcodeReader();
                Result result = Reader.Decode((Bitmap)pictureBox1.Image);
                string decoded = result.ToString().Trim();
                textBox1.Text = decoded;
                if (decoded == "")
                {
                    timer1.Stop();
                    timer1.Enabled = false;
                    button2.Enabled = true;
                    textBox1.Text = decoded;

                }
               
            }
            catch (Exception)
            {
                //throw;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
            timer1.Enabled = true;
            timer1.Start();
            button2.Enabled = false;
            textBox1.Text = "";
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FinalFrame.IsRunning == true)
                FinalFrame.Stop();
        }
    }
}
