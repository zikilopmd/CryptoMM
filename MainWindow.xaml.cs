using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

using Microsoft.Win32;
using NickBuhro.Translit;
using QRCoder;
using static System.Net.Mime.MediaTypeNames;

namespace WpfAppCryptoMM
{

    public partial class MainWindow : Window
    {

        RSA rsa = Crypto.GetRSA();
        Aes aes = Crypto.GetAES();

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private static async Task<string> ImportPemCetificate()
        {
            OpenFileDialog dial = new Microsoft.Win32.OpenFileDialog();
            dial.DefaultExt = "Downloads";
            dial.Filter = "PEM cetificate|*.pem;*.crt;*.cer;*.key|All files|*.*";
            dial.Title = "Import PEM cetificate";
            bool? result = dial.ShowDialog();
            if (result == true)
            {
                using (FileStream fstream = File.OpenRead(dial.FileName))
                {
                    byte[] buffer = new byte[fstream.Length];
                    try
                    {
                        await fstream.ReadAsync(buffer, 0, buffer.Length);
                        fstream.Close();
                        return Encoding.UTF8.GetString(buffer);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return "";
                    }
                }
            }
            else
            {
                return "";
            }
        }

        private async void ImportPublicPEM(object sender, RoutedEventArgs e)
        {
            ReadOnlySpan<char> PEM = (await ImportPemCetificate()).AsSpan();
            if (PEM.Length != 0)
            {
                rsa.ImportFromPem(PEM);
                MessageBox.Show(rsa.ExportRSAPublicKeyPem());
            }
        }

        private async void ImportPrivatePEM(object sender, RoutedEventArgs e)
        {
            ReadOnlySpan<char> PEM = (await ImportPemCetificate()).AsSpan();
            if (PEM.Length != 0)
            {
                rsa.ImportFromPem(PEM);
                MessageBox.Show(rsa.ExportRSAPrivateKeyPem());
            }
        }

        private void ImportAESKey(object sender, RoutedEventArgs e)
        {
            aes.Key = Encoding.UTF8.GetBytes(AES_Key.Text);
        }
        private void ImportAES_IV(object sender, RoutedEventArgs e)
        {
            aes.IV = Encoding.UTF8.GetBytes(AES_IV.Text);
        }

        private async void GenerateRSAPairKeys(object sender, RoutedEventArgs e)
        {
            RSA r = Crypto.GetRSA();
            SaveFileDialog dial1 = new Microsoft.Win32.SaveFileDialog();
            dial1.Filter = "PEM public certificates|*.pem|All files|*.*";
            dial1.DefaultExt = "Downloads";
            dial1.Title = "Export PEM Secrtificate";
            dial1.FileName = "Public RSA Secrtificate.pem";
            if (dial1.ShowDialog() == true)
            {
                using (FileStream fstream = File.OpenWrite(dial1.FileName))
                {
                    string publicPem = r.ExportRSAPublicKeyPem();
                    byte[] buffer = Encoding.UTF8.GetBytes(publicPem);
                    try
                    {
                        await fstream.WriteAsync(buffer, 0, buffer.Length);
                        fstream.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }

            SaveFileDialog dial2 = new Microsoft.Win32.SaveFileDialog();
            dial2.Filter = "PEM private certificates (*.pem)|*.pem|All files|*.*";
            dial2.DefaultExt = "Downloads";
            dial2.Title = "Export PEM Secrtificate";
            dial2.FileName = "Private RSA Secrtificate.pem";
            if (dial2.ShowDialog() == true)
            {
                using (FileStream fstream = File.OpenWrite(dial2.FileName))
                {
                    string publicPem = r.ExportRSAPrivateKeyPem();
                    byte[] buffer = Encoding.UTF8.GetBytes(publicPem);
                    try
                    {
                        await fstream.WriteAsync(buffer, 0, buffer.Length);
                        fstream.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private async void GenerateAESPairKeys(object sender, RoutedEventArgs e)
        {
            Aes a = Crypto.GetAES();
            a.GenerateKey();
            a.GenerateIV();
            SaveFileDialog dial = new Microsoft.Win32.SaveFileDialog();
            dial.Filter = "AES Secrtificate|*.aes256|All files|*.*";
            dial.DefaultExt = "Downloads";
            dial.Title = "Export AES Secrtificate";
            dial.FileName = "AES Secrtificate.aes256";
            if (dial.ShowDialog() == true)
            {
                using (FileStream fstream = File.OpenWrite(dial.FileName))
                {
                    string data = "-----BEGIN AES KEY-----\n" + Convert.ToBase64String(a.Key) + "\n-----END AES KEY-----\n\n-----BEGIN AES INITIALIZATION VECTOR-----\n" + Convert.ToBase64String(a.IV) + "\n-----END AES INITIALIZATION VECTOR-----";
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    try
                    {
                        await fstream.WriteAsync(buffer, 0, buffer.Length);
                        fstream.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void EncryptAES(object sender, RoutedEventArgs e)
        {
            AES_Key.Text = Convert.ToBase64String(rsa.Decrypt(Convert.FromBase64String(AES_Key.Text), RSAEncryptionPadding.Pkcs1));
            AES_IV.Text = Convert.ToBase64String(rsa.Decrypt(Convert.FromBase64String(AES_IV.Text), RSAEncryptionPadding.Pkcs1));
        }

        private void Encrypto(object sender, RoutedEventArgs e)
        {
            string buffer = E_TextIn.Text;

            if (EisRussian.IsChecked ?? false)
            {
                buffer = Transliteration.CyrillicToLatin(buffer, NickBuhro.Translit.Language.Russian);
            }

            if ((buffer.Length % 16) != 0)
            {
                for (int i = 16 - (buffer.Length % 16); i > 0; i--)
                {
                    buffer = buffer + " ";
                }
            }
            ICryptoTransform encryptor;
            if ( (AES_Key.Text != "" & AES_IV.Text != "") & (buffer.Length != 0))
            {
                encryptor = aes.CreateEncryptor(Convert.FromBase64String(AES_Key.Text), Convert.FromBase64String(AES_IV.Text));
                try
                {
                    E_TextOut.Text = Convert.ToBase64String(encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(buffer), 0, Encoding.UTF8.GetBytes(buffer).Length));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                }
            }

        }

        private void Decrypto(object sender, RoutedEventArgs e)
        {
            if (AES_Key.Text != "" & AES_IV.Text != "")
            {
                ICryptoTransform encryptor = aes.CreateDecryptor(Convert.FromBase64String(AES_Key.Text), Convert.FromBase64String(AES_IV.Text));
                byte[] buffer = Convert.FromBase64String(D_TextIn.Text);
                if (buffer.Length != 0)
                {
                    string text = Encoding.Default.GetString(encryptor.TransformFinalBlock(buffer, 0, buffer.Length));

                    if (DisRussian.IsChecked ?? false)
                    {
                        text = Transliteration.LatinToCyrillic(text, NickBuhro.Translit.Language.Russian);
                    }
                    D_TextOut.Text = text.TrimEnd();
                }
            }

        }

        private async void QRImportFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dial = new Microsoft.Win32.OpenFileDialog();
            dial.DefaultExt = "Downloads";
            dial.Filter = "PEM cetificate|*.pem;*.crt;*.cer;*.key|AES Secrtificate|*.aes256|All files|*.*";
            dial.Title = "Import file";
            bool? result = dial.ShowDialog();
            if (result == true)
            {
                using (FileStream fstream = File.OpenRead(dial.FileName))
                {
                    byte[] buffer = new byte[fstream.Length];
                    try
                    {
                        await fstream.ReadAsync(buffer, 0, buffer.Length);
                        QRData.Text = Encoding.UTF8.GetString(buffer);
                        fstream.Close();
                        QRMake(QRImage, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        public async void QRMake(object sender, RoutedEventArgs e)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(QRData.Text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "qr.png";
            FileStream fstream = new FileStream(path, FileMode.OpenOrCreate);
            qrCodeImage.Save(fstream, ImageFormat.Png);
            await fstream.FlushAsync();
            fstream.Close();
    
            Uri resourceUri = new Uri(path, UriKind.Relative);

            BitmapImage map = new BitmapImage();
            map.BeginInit();
            map.CacheOption = BitmapCacheOption.OnLoad;
            map.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            map.UriSource = resourceUri;
            map.EndInit();
            QRImage.BeginInit();
            QRImage.Source = map;
            QRImage.EndInit();
            UpdateLayout();
            InitializeComponent();
        }

        private void QRSave(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dial = new Microsoft.Win32.SaveFileDialog();
            dial.Filter = "QR code|*.png;*.jpg;*.jpeg|All files|*.*";
            dial.DefaultExt = "Downloads";
            dial.Title = "Save QR code";
            dial.FileName = "QR.png";
            if (dial.ShowDialog() == true)
            {
                using (FileStream fstream = File.OpenWrite(dial.FileName))
                {

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(QRData.Text, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    try
                    {
                        qrCodeImage.Save(fstream, ImageFormat.Png);
                        fstream.Flush();
                        fstream.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }
    }
}