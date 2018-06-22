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
using MimeKit;
namespace MailBox
{
    /// <summary>
    /// Interaction logic for MailList.xaml
    /// </summary>
    public partial class MailList : UserControl
    {
        public MailList()
        {
            InitializeComponent();
        }
        private Grid CreateGrid(MimeMessage msg)
        {
            int i = 0;
            var grid = new Grid();
            grid.Margin = new Thickness(0,0,10,0);
            GenerateRowsAndColumns(grid);
            var from = getMailbox(msg.From.Mailboxes);
            GenerateTextBlock(grid,GetString(msg.To.Mailboxes),ref i);
            GenerateTextBlock(grid, GetString(msg.From.Mailboxes), ref i);
            GenerateTextBlock(grid, msg.Subject, ref i);
            GenerateImage(grid);
            return grid;
        }
        private string GetString(IEnumerable<MailboxAddress> addresses)
        {
            var addrs = getMailbox(addresses);
            string str = String.Empty;
            foreach (var item in addrs)
            {
                str += item;
            }
            return str;
        }
        private void GenerateImage(Grid grid)
        {
            var img = new Image();
            var uri = new Uri(@"/images/x_button.png",UriKind.Relative);
            var bitmap = new BitmapImage(uri);
            img.Source = bitmap;
            img.Visibility = Visibility.Hidden;
            grid.Children.Add(img);
            Grid.SetColumn(img, 1);
            Grid.SetRow(img, 0);
        }
        private void GenerateTextBlock(Grid grid, string text,ref int ix)
        {
            var block = new TextBlock();
            block.Text = text;
            grid.Children.Add(block);
            Grid.SetColumn(block, 0);
            Grid.SetRow(block, ix);
            ix++;
        }
        private void GenerateRowsAndColumns(Grid grid)
        {
            grid.RowDefinitions.Add(GenerateRow(20));
            grid.RowDefinitions.Add(GenerateRow(20));
            grid.RowDefinitions.Add(GenerateRow(30));
            grid.ColumnDefinitions.Add(GenerateColumn(230));
            grid.ColumnDefinitions.Add(GenerateColumn(20));

        }
        private RowDefinition GenerateRow(int height)
        {
            var row = new RowDefinition();
            row.Height = new GridLength(height);
            return row;
        }
        private ColumnDefinition GenerateColumn(int width)
        {
            var col = new ColumnDefinition();
            col.Width = new GridLength(width);
            return col;
        }
        public void ShowMessageList(List<MimeMessage> messages)
        {
            panel.Children.Clear();

            int i = 1;
            foreach (var msg in messages)
            {
                var button = new Button();
                button.Style= Resources["ListButton"] as Style;
                button.Background = Brushes.White;
                button.Foreground = Brushes.Black;
                button.BorderBrush = Brushes.White;
                button.Uid = i.ToString();
                button.MouseEnter += Button_MouseEnter;
                button.MouseLeave += Button_MouseLeave;
                button.Click += Button_Click;
                button.Content = CreateGrid(msg);
                panel.Children.Add(button);
                i++;
            }

        }
        public void NewMessage(MimeMessage message)
        {

        }
      
        private string Splitted(string str)
        {
            var split= str.Split();
            string body=String.Empty;
            if(split.Length>5)
                for (int i = 0; i < 5; i++)
                {
                    body += split[i] + " ";
                }
            else
            {
                for (int i = 0; i < split.Length; i++)
                {
                    body += split[i] + " ";
                }
            }
            return body;
        }
        private void Button_Click(object sender, EventArgs arg)
        {
            var button = sender as Button;
            int uid = int.Parse(button.Uid);
            MainWindow win = App.Current.MainWindow as MainWindow;
            win.OpenInBrowser(uid);

        }
        private void TextBlock_MouseLeave(object sender, MouseEventArgs arg)
        {
            var block = sender as TextBlock;
            block.Foreground = Brushes.Black;
        }
        private void TextBlock_MouseEnter(object sender, MouseEventArgs arg)
        {
            var block = sender as TextBlock;
            block.Foreground = Brushes.Blue;
        }
        private List<string> getMailbox(IEnumerable<MailboxAddress> addresses)
        {
            var listofadrs = new List<string>();
            foreach (var addres in addresses)
            {
                listofadrs.Add(addres.Address);
            }
            return listofadrs;
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
         //   btn.Background = Brushes.DimGray;
            var grid = btn.Content as Grid;
            grid.Children[3].Visibility = Visibility.Visible;
            var controls = grid.Children;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            btn.Background = Brushes.White;
            var grid = btn.Content as Grid;
            grid.Children[3].Visibility = Visibility.Hidden;
            var controls = grid.Children;
        }
    }
}
