using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MimeKit;
namespace MailBox
{
    enum SortFilters
    {
        Subject=0,Date=1,From=2
    }
    enum MessageParts
    {
        Subject=0,Body=1,Attachments=2,From=4,Date=8
    }
    enum SearchFilters
    {
        After=0,Before=1,Contains=2,Longer=4,Shorter=8,Send=16,From=32,HasAttachments=64
    }
    enum Order
    {
        ASC =0, DSC = 1
    }
    
    class Imapfeatures
    {
        List<MimeMessage> messages = new List<MimeMessage>();
        List<MimeMessage> filtred = new List<MimeMessage>();
        public Imapfeatures(List<MimeMessage> messages)
        {
            this.messages = messages;
            this.filtred = messages;
        }
        public void ResetFilters()
        {
            filtred = messages;
        }
        public List<MimeMessage> SortBy(SortFilters filter,Order sort)
        {
            switch (filter)
            {
                case SortFilters.Subject:
                    if (sort == Order.ASC) SortAsc(filter); else SortDsc(filter);
                    break;
                case SortFilters.Date:
                    if (sort == Order.ASC) SortAsc(filter); else SortDsc(filter);
                    break;
                case SortFilters.From:
                    if (sort == Order.ASC) SortAsc(filter); else SortDsc(filter);
                    break;
            }
            return messages;
        }
        private void SortAsc(SortFilters filter)
        {
            switch (filter)
            {
                case SortFilters.Subject:
                        messages.Sort((m1, m2) =>m1.Subject.CompareTo(m2.Subject));
                    break;
                case SortFilters.Date:
                    messages.Sort((m1, m2) => m1.Date.CompareTo(m2.Date));
                    break;
                case SortFilters.From:
                    messages.Sort((m1, m2) => m1.From.CompareTo(m2.From));
                    break;
            }
        }
        private void SortDsc(SortFilters filter)
        {
            switch (filter)
            {
                case SortFilters.Subject:
                    messages.Sort((m1, m2) => m1.Subject.CompareTo(m2.Subject));
                    messages.Reverse();
                    break;
                case SortFilters.Date:
                    messages.Sort((m1, m2) => m1.Date.CompareTo(m2.Date));
                    messages.Reverse();
                    break;
                case SortFilters.From:
                    messages.Sort((m1, m2) => m1.From.CompareTo(m2.From));
                    messages.Reverse();
                    break;
            }
        }
        public List<MimeMessage> FilterBy(MessageParts part,SearchFilters search,string filter)
        {
            switch (part)
            {
                case MessageParts.Subject: FilterSubjectBy(search,filter);
                    break;
                case MessageParts.Body: FilterBodyBy(search,filter);
                    break;
                case MessageParts.From: FilterFromBy(search,filter);
                    break;
                case MessageParts.Date: FilterDateBy(search,filter);
                    break;
            }
            return filtred;
        }
        private void FilterSubjectBy(SearchFilters search, string filter)
        {
            switch (search)
            {
                case SearchFilters.Contains:
                    filtred = filtred.Where((m) => m.Subject.Contains(filter)).ToList();
                    break;
                case SearchFilters.Longer:
                    filtred = filtred.Where((m) => m.Subject.Length>Int32.Parse(filter)).ToList();
                    break;
                case SearchFilters.Shorter:
                    filtred = filtred.Where((m) => m.Subject.Length < Int32.Parse(filter)).ToList();
                    break;

            }
        }
        private void FilterBodyBy(SearchFilters search, string filter)
        {
            switch (search)
            {
                case SearchFilters.Contains:
                    filtred = filtred.Where((m) => m.TextBody != null ? m.TextBody.Contains(filter) : m.HtmlBody != null ? m.TextBody.Contains(filter) : false).ToList();
                    break;
                case SearchFilters.Longer:
                    filtred = filtred.Where((m) => m.TextBody!=null?m.TextBody.Length > Int32.Parse(filter):m.HtmlBody!=null? m.HtmlBody.Length > Int32.Parse(filter):false).ToList();
                    break;
                case SearchFilters.Shorter:
                    // filtred = filtred.Where((m) => m.TextBody.Length < Int32.Parse(filter)).ToList();
                    filtred = filtred.Where((m) =>
                         m.TextBody != null ? m.TextBody.Length < Int32.Parse(filter) : m.HtmlBody != null ? m.HtmlBody.Length < Int32.Parse(filter) : false).ToList();
                    break;
                case SearchFilters.HasAttachments:
                    filtred = filtred.Where((m) => m.Attachments.LongCount()>0).ToList();
                    break;
            }
        }
        private void FilterFromBy(SearchFilters search, string filter)
        {
            switch (search)
            {
                case SearchFilters.Contains:
                    filtred = filtred.Where((m) =>GetString(m.From).Contains(filter)).ToList();
                    break;
            }
        }
        private void FilterDateBy(SearchFilters search, string filter)
        {
            switch (search)
            {
                case SearchFilters.After:
                    filtred = filtred.Where((m)=>m.Date.Date>DateTime.Parse(filter)).ToList();
                    break;
                case SearchFilters.Before:
                    filtred = filtred.Where((m) => m.Date.Date < DateTime.Parse(filter)).ToList();
                    break;
            }
        }
        private string GetString(InternetAddressList addresses)
        {
            var froms = addresses.Mailboxes;
            StringBuilder sb = new StringBuilder();
            foreach (var item in froms)
            {
                 sb.Append(item.Address);
            }
            return sb.ToString();
        }
    }

}
