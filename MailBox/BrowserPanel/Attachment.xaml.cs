using MimeKit;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Attachment.xaml
    /// </summary>
    public partial class Attachment : UserControl
    {
        public MimeEntity Attach { get; set; }
        public Attachment(string label, MimeEntity attachment)
        {
            InitializeComponent();
            attachmentLabel.Text = label;
            Attach = attachment;
        }

        public void SetDownloaded()
        {
            attachmentLabel.Background = Brushes.LightGreen;
        }
    }
}
