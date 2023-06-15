using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Permissions;
using System.Threading;
using Microsoft.VisualBasic.FileIO;

namespace copyAssisstant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isExit;
        private FileSystemWatcher wathcer = new FileSystemWatcher();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            _notifyIcon.Icon = copyAssisstant.Properties.Resources.MyIcon;
            _notifyIcon.Visible = true;

            CreateContextMenu();
            try
            {
                string[] firstpath = File.ReadAllText("address.txt").Split(',');
                wathcer.Path = firstpath[0];
                wathcer.NotifyFilter = NotifyFilters.Size;
                wathcer.Changed += onChanged;
                wathcer.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                string MessageBoxText = " مسیر نگهداری و مسیر انتقال فایل ها مشخص نیست جهت استفاده از برنامه تنظیمات لازم را انجام دهید ";
                string caption = "خطا در خواندن فایل";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Information;
                System.Windows.MessageBox.Show(MessageBoxText, caption, button, icon);
            }
            
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
              new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("تنظیمات").Click += (s, e) => ShowMainWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("خروج").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); 
            }
        }
        private static void onChanged (object source , FileSystemEventArgs e) 
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
            
            //foreach(string pathname in filepaths)
            //{
                try
                {
                    if (!File.Exists("address.txt"))
                        File.Create("address.txt");
                    string[] address = File.ReadAllText("address.txt").Split(',');
                    string[] filepaths = Directory.GetFileSystemEntries(address[0]);
                    FileInfo mainPath = new FileInfo(address[1]);
                    DriveInfo driveInfo = new DriveInfo(mainPath.Directory.Root.FullName);
                    FileInfo _source = null;
                    FileInfo _dest = null;
                    _source = new FileInfo(e.FullPath);
                    _dest = new FileInfo(address[1] + "\\" + e.Name);
                    while (driveInfo.AvailableFreeSpace < _source.Length)
                    {
                        deleteLast(driveInfo, filepaths, _source);
                    }
                    if (_dest.Exists)
                        _dest.Delete();
                    while (IsFileLocked(_source))
                        { }
                    Thread.Sleep(10000);
                    while (IsFileLocked(_source))
                    { }
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(e.FullPath, address[1] + "\\" + e.Name, UIOption.AllDialogs);
                }
                catch (Exception ex)
                {
                    string MessageBoxText = "خطا : " + ex.Message;
                    string caption = "خطا در انتقال";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(MessageBoxText, caption, button, icon);
                }
            });

            //}
        }
        
        protected static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        private static void deleteLast(DriveInfo driveInfo,string[] filePaths,FileInfo currentFile)
        {
            if (filePaths.Length != 0)
            {
                FileInfo[] filesInfo = null;
                if (driveInfo.AvailableFreeSpace < currentFile.Length)
                {
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        filesInfo[i] = new FileInfo(filePaths[i]);
                    }
                    filesInfo.OrderByDescending(x => x.CreationTime);
                    File.Delete(filesInfo[filesInfo.Length - 1].FullName);
                }
            }
        }
    }
}
