using Microsoft.Win32;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MailBox.BrowserPanel
{
    /// <summary>
    /// Interaction logic for BrowserPanel.xaml
    /// </summary>
    public partial class BrowserPanel : UserControl
    {
        private MimeMessage _message;
        private string _path;

        public BrowserPanel()
        {
            InitializeComponent();
        }

        public void ChangeTarget(MimeMessage message, int uid, string path, HashSet<string> tempDirs)
        {
            _message = message;
            _path = path;

            attachments.Children.Clear();
            scrollViewerAttachments.Visibility = Visibility.Collapsed;

            FillTextBlocks();
            FillBrowser(System.IO.Path.Combine(_path, "msg" + uid), tempDirs);
            FillAttachments();
           
        }

        private void FillAttachments()
        {
            List<MimeEntity> list = new List<MimeEntity>(_message.Attachments);
            if (list.Count() != 0)
            {
                scrollViewerAttachments.Visibility = Visibility.Visible;
                foreach (var foo in list)
                {
                    Attachment attach = new Attachment(foo.ContentType.Name, foo);
                    attach.MouseLeftButtonDown += Attach_MouseLeftButtonDown;
                    attachments.Children.Add(attach);
                }
            }
        }

        private void Attach_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MimeEntity attachment = (sender as Attachment).Attach; 

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = attachment.ContentType.Name,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = attachment.ContentType.Name.Split('.').Last(),
                Filter = "All files (*.*)|*.*",
                AddExtension = true,
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var stream = File.Create(saveFileDialog.FileName))
                {
                    if (attachment is MessagePart)
                    {
                        var part = (MessagePart)attachment;

                        part.Message.WriteTo(stream);
                    }
                    else
                    {
                        var part = (MimePart)attachment;

                        part.Content.DecodeTo(stream);
                    }
                }
                (sender as Attachment).SetDownloaded();
            }
            if (MessageBox.Show("Open file?", attachment.ContentType.Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                System.Diagnostics.Process.Start(saveFileDialog.FileName);
            
        }

        private void FillTextBlocks()
        {
            infoStackPanel.DataContext = _message;
            fromTextBlock.Text = String.Join(" ", _message.From);
            toTextBlock.Text = String.Join(" ", _message.To);
        }

        private void FillBrowser(string fullPath, HashSet<string> tempDirs)
        {
            Directory.CreateDirectory(fullPath);
            tempDirs.Add(fullPath);
            HtmlPreviewVisitor htmlPreview = new HtmlPreviewVisitor(fullPath);
            _message.Accept(htmlPreview);
            browserBrowser.NavigateToString(htmlPreview.HtmlBody);
        }


        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            new Send.SendWindow(String.Join(";",_message.From), "Re: " +_message.Subject).Show();
        }
    }
}
