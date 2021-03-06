﻿using System;
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
        MainWindow mainWindow;
        Messages msgclient;

        public MailList()
        {
            InitializeComponent();
        }
        #region Generators
        private Grid CreateGrid(MimeMessage msg)
        {
            int i = 0;
            var grid = new Grid();
            grid.Margin = new Thickness(0,0,10,0);
            GenerateRowsAndColumns(grid);
            var from = getMailbox(msg.From.Mailboxes);
            GenerateTextBlock(grid, GetString(msg.To.Mailboxes), ref i);
            GenerateTextBlock(grid, GetString(msg.From.Mailboxes), ref i);
            GenerateTextBlock(grid, msg.Subject, ref i);
            GenerateImage(grid);
            return grid;
        }
        private void GenerateImage(Grid grid)
        {
            var img = new Image();
            var uri = new Uri(@"/images/x_button.png", UriKind.Relative);
            var bitmap = new BitmapImage(uri);
            img.Source = bitmap;
            img.Visibility = Visibility.Hidden;
            grid.Children.Add(img);
            img.MouseLeftButtonDown += Img_MouseLeftButtonDown;
            Grid.SetColumn(img, 1);
            Grid.SetRow(img, 0);
        }
        private void GenerateTextBlock(Grid grid, string text, ref int ix)
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
        #endregion
        #region event handlers
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var grid = btn.Content as Grid;
            grid.Children[3].Visibility = Visibility.Visible;
            var controls = grid.Children;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            var btn = sender as Button;
            var grid = btn.Content as Grid;
            grid.Children[3].Visibility = Visibility.Hidden;
            var controls = grid.Children;
        }
        private void Button_Click(object sender, EventArgs arg)
        {
            var button = sender as Button;
            
            int uid = int.Parse(button.Uid);
            msgclient.MessageShown(uid);
            mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow.OpenInBrowser(uid);

        }
        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button button = ((sender as Image).Parent as Grid).Parent as Button;
            button.Background = Brushes.White;
            button.Foreground = Brushes.Black;
            button.BorderBrush = Brushes.White;
            var i = int.Parse(button.Uid);
            mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow.DeleteMessage(i);
            panel.Children.Remove(button);
        }
        #endregion
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

        public void MarkAsRead(int index)
        {
            var message = panel.Children[index] as Button;
            message.Background = Brushes.White;
            message.Foreground = Brushes.Black;
            message.BorderBrush = Brushes.White;
        }

        public void ShowMessageList(Messages msgclient,List<MimeMessage> messages)
        {
            panel.Children.Clear();
            int i = 1;
            uint index = 0;

            foreach (var msg in messages)
            {
                var button = new Button();
                button.Style= Resources["ListButton"] as Style;
                if (msgclient.CheckIfRead(msg))
                {
                    button.Background = Brushes.LightCyan;
                    button.BorderBrush = Brushes.LightBlue;
                }
                else
                {
                    button.Background = Brushes.White;
                    button.Foreground = Brushes.Black;
                    button.BorderBrush = Brushes.White;
                }
               
                button.Uid = i.ToString();
                

                button.MouseEnter += Button_MouseEnter;
                button.MouseLeave += Button_MouseLeave;
                button.Click += Button_Click;
                button.Content = CreateGrid(msg);
                panel.Children.Add(button);
                i++;
                index++;
            }
            this.msgclient = msgclient;
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
       
        private List<string> getMailbox(IEnumerable<MailboxAddress> addresses)
        {
            var listofadrs = new List<string>();
            foreach (var addres in addresses)
            {
                listofadrs.Add(addres.Address);
            }
            return listofadrs;
        }

     
    }
}
