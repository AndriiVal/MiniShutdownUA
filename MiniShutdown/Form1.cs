using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MiniShutdown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Додавання програми в автозапуск
            Microsoft.Win32.RegistryKey MyKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            try
            {
                if (MyKey.GetValue("MiniShutdown").ToString() != Assembly.GetExecutingAssembly().Location)
                    MyKey.SetValue("MiniShutdown", Assembly.GetExecutingAssembly().Location);
            }

            catch (NullReferenceException)
            {
                MyKey.SetValue("MiniShutdown", Assembly.GetExecutingAssembly().Location);
            }
            MyKey.Flush();
            MyKey.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Приховання програми
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Minimized;
            Height = 0;
            Width = 0;
            ShowIcon = false;
            ShowInTaskbar = false;
        }

        // Вихід з програми
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Про програму
        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MiniShutdown - іконка завершення роботи для області сповіщень на панелі завдань." + Environment.NewLine +
                "Подвійний клік по іконці програми викликає діалогове вікно завершення роботи системи." + Environment.NewLine +
                "Автор: Андрій Вальчук.", "Про програму", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Підключення dll
        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport("user32")]
        public static extern void LockWorkStation();

        private void OutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitWindowsEx(0, 0);
        }

        // Сон
        private void SleepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.SetSuspendState(PowerState.Suspend, true, true);
        }

        // Перезавантаження
        private void ReloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/r /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi);
        }

        // Завершення роботи
        private void ShutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/s /t 0")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process.Start(psi);
        }

        // Блокування
        private void LockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LockWorkStation();
        }

        // Гібернація
        private void HibernateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.SetSuspendState(PowerState.Hibernate, true, true);
        }

        // Діалогове вікно завершення роботи при подвійному кліку
        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogResult result = MessageBox.Show(new Form() { TopMost = true },
                                                  "Завершити роботу?",
                                                  "Вихід",
                                                  MessageBoxButtons.OKCancel,
                                                  MessageBoxIcon.Warning);
            if (result == DialogResult.OK)
            {
                ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/s /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
        }
    }
}
