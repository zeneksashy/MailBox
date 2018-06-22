using Microsoft.Win32;
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

namespace MailBox.Send
{
    /// <summary>
    /// Interaction logic for Attachment.xaml
    /// </summary>
    public partial class Attachment : UserControl
    {
        public string FilePath { get; private set; }
        public string FileName { get { return attachmentLabel.Text; } private set { attachmentLabel.Text = value; } }
        public Attachment(OpenFileDialog file)
        {
            InitializeComponent();
            FileName = file.SafeFileName;
            FilePath = file.FileName;
        }
    }
}
