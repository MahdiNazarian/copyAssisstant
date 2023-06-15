using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;


namespace copyAssisstant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            try
            {
                string[] firstpath = File.ReadAllText("address.txt").Split(',');
                if (firstpath.Length > 0)
                {
                    if (Directory.Exists(firstpath[0]) && Directory.Exists(firstpath[1]))
                        this.Hide();
                }
            }
            catch
            {

            }
        }
        private void btnSelectFirstAddress_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fileDlg = new FolderBrowserDialog();
            fileDlg.Description = "محل نگهداری فایل ها را انتخاب کنید";
            DialogResult result = fileDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                firstAddress.Text = fileDlg.SelectedPath;
            }
            
        }

        private void btnSelectSecondLocation_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fileDlg = new FolderBrowserDialog();
            fileDlg.Description = "محل دخیره نهایی فایل ها را انتخاب کنید";
            DialogResult result = fileDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                secondLocationAddress.Text = fileDlg.SelectedPath;
            }
        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            string filesAddress = firstAddress.Text + "," + secondLocationAddress.Text;
            try
            {
                if (Directory.Exists(firstAddress.Text) && Directory.Exists(secondLocationAddress.Text))
                {
                    File.WriteAllText("address.txt", filesAddress);
                    string MessageBoxText = "اطلاعات با موفقیت ذخیره شد ";
                    string caption = "ذخیره اطلاعات";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    System.Windows.MessageBox.Show(MessageBoxText, caption, button, icon);
                    System.Diagnostics.Process.Start(System.Windows.Application.ResourceAssembly.Location);
                    System.Windows.Application.Current.Shutdown();
                }
                else
                {
                    string MessageBoxText = "مسیر های مشخص شده معتبر نیست ";
                    string caption = "ذخیره اطلاعات";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    System.Windows.MessageBox.Show(MessageBoxText, caption, button, icon);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("address.txt", filesAddress);
                string MessageBoxText = "خطا : "+ex.Message;
                string caption = "ذخیره اطلاعات";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                System.Windows.MessageBox.Show(MessageBoxText, caption, button, icon);
            }

        }
    }
}
