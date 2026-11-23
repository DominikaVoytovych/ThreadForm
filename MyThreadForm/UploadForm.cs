using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;

namespace MyThreadForm
{
    public partial class UploadForm : Form
    {
        public UploadForm()
        {
            InitializeComponent();
        }

        private void btnStartDownload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDirPath.Text))
            {
                MessageBox.Show("Спочатку оберіть папку для збереження!");
                return;
            }

            int count = int.Parse(txtCount.Text);
            string folderPath = txtDirPath.Text;

            lCountDownloads.Text = $"0/{count}";
            pbDownloads.Maximum = count;
            pbDownloads.Value = 0;

            // Це основний потік
            Thread myThread = new Thread(RunDownloadThread);
            myThread.Start(new object[] { count, folderPath });
        }

        private void RunDownloadThread(object data)
        {
            // Інший потік, який буде завантажувати фото
            object[] parameters = (object[])data;
            int countInt = (int)parameters[0]; // Кількість елементів для завантаження
            string folderPath = (string)parameters[1];

            string url = "https://i.pravatar.cc/1200";

            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < countInt; i++)
                {
                    // Завантажуємо фото з мережі та зберігаємо у вибрану папку
                    byte[] imageBytes = client.GetByteArrayAsync(url).Result;
                    string filePath = Path.Combine(folderPath, $"photo_{i + 1}.jpg");
                    File.WriteAllBytes(filePath, imageBytes);

                    this.Invoke(new Action(() =>
                    {
                        lCountDownloads.Text = $"{i + 1}/{countInt}";
                        pbDownloads.Value = i + 1;
                    }));
                }
            }

            this.Invoke(new Action(() =>
            {
                MessageBox.Show("Завантаження фото завершено!");
            }));
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog myDialog = new FolderBrowserDialog();
            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                string folder = myDialog.SelectedPath;
                txtDirPath.Text = folder;
            }
        }
    }
}