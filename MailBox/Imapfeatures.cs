using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MimeKit;
namespace MailBox
{
    ///
    ///Enums use for sorting and filtering
    ///
    #region Enums
    /// <summary>
    /// parts of message that message should be sorted by
    /// </summary>
    enum SortFilters
    {
        Subject = 0, Date = 1, From = 2
    }
    /// <summary>
    /// message parts that message should be filtred by
    /// </summary>
    enum MessageParts
    {
        Subject = 0, Body = 1, Attachments = 2, From = 4, Date = 8
    }
    /// <summary>
    /// filters that can be applied to filtring
    /// </summary>
    enum SearchFilters
    {
        After = 0, Before = 1, Contains = 2, Longer = 4, Shorter = 8, Send = 16, From = 32, HasAttachments = 64
    }
    /// <summary>
    /// Types of order that should be taken while sorting
    /// </summary>
    enum Order
    {
        ASC = 0, DSC = 1
    }

    #endregion
    // class MessageSort : IComparer
    //{
    //    int IComparer<T>.Compare(T first , T second)
    //    {

    //        //car c1 = (car)a;
    //        //car c2 = (car)b;
    //        //if (c1.year > c2.year)
    //        //    return 1;
    //        //if (c1.year < c2.year)
    //        //    return -1;
    //        //else
    //        //    return 0;
    //    }
    //}
    class Imapfeatures
    {
        HashSet<int> uids = new HashSet<int>();
        List<MimeMessage> messages = new List<MimeMessage>();
        List<MimeMessage> filtred = new List<MimeMessage>();
        /// <summary>
        /// Main construcotr of imapfeatures class declare messages and filtred list from parameter
        /// </summary>
        /// <param name="messages"> list of messages from inbox</param>
        public Imapfeatures(List<MimeMessage> messages)//, HashSet<int> uids)
        {
            this.messages = messages;
            this.filtred = messages;
           // this.uids = uid
        }
      


        #region Sorting
        /// <summary>
        /// Sorts messages by two filters
        /// </summary>
        /// <param name="filter"> object that method sorts by</param>
        /// <param name="sort"> order of sorting</param>
        /// <returns> List of sorted messages</returns>
        public List<MimeMessage> SortBy(SortFilters filter, Order sort)
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
        /// <summary>
        /// sorts list ascending
        /// </summary>
        /// <param name="filter"> object that method sorts by</param>
        private void SortAsc(SortFilters filter)
        {
            switch (filter)
            {
                case SortFilters.Subject:
                    messages.Sort((m1, m2) => m1.Subject.CompareTo(m2.Subject));
                    break;
                case SortFilters.Date:
                    messages.Sort((m1, m2) => m1.Date.CompareTo(m2.Date));
                    break;
                case SortFilters.From:
                    messages.Sort((m1, m2) => m1.From.CompareTo(m2.From));
                    break;
            }
        }
        //public static IComparer Sort()
        //{
        //    return (IComparer)new sortYearAscendingHelper();
        //}
        /// <summary>
        /// sorts list descending
        /// </summary>
        /// <param name="filter"> object that method sorts by</param>
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
        #endregion


        #region Filtering

        /// <summary>
        /// reset the filtred list to initial state
        /// </summary>
        public void ResetFilters()
        {
            filtred = messages;
        }
        /// <summary>
        /// searchs in messages for specified filter, applies it to filtred list and returns it
        /// </summary>
        /// <param name="part">part of mail message within the search will take place  </param>
        /// <param name="search"> kind of search we want to filter</param>
        /// <param name="filter">filter that the method will search by</param>
        /// <returns></returns>
        public List<MimeMessage> FilterBy(MessageParts part, SearchFilters search, string filter)
        {
            switch (part)
            {
                case MessageParts.Subject:
                    FilterSubjectBy(search, filter);
                    break;
                case MessageParts.Body:
                    FilterBodyBy(search, filter);
                    break;
                case MessageParts.From:
                    FilterFromBy(search, filter);
                    break;
                case MessageParts.Date:
                    FilterDateBy(search, filter);
                    break;
            }
            return filtred;
        }
        /// <summary>
        /// Searches subjects by specfied filter
        /// </summary>
        /// <param name="search"></param>
        /// <param name="filter"></param>
        private void FilterSubjectBy(SearchFilters search, string filter)
        {
            switch (search)
            {
                case SearchFilters.Contains:
                    filtred = filtred.Where((m) => m.Subject.Contains(filter)).ToList();
                    break;
                case SearchFilters.Longer:
                    filtred = filtred.Where((m) => m.Subject.Length > Int32.Parse(filter)).ToList();
                    break;
                case SearchFilters.Shorter:
                    filtred = filtred.Where((m) => m.Subject.Length < Int32.Parse(filter)).ToList();
                    break;

            }
        }
        /// <summary>
        /// Searches body by specfied filter
        /// </summary>
        /// <param name="search"></param>
        /// <param name="filter"></param>
        private void FilterBodyBy(SearchFilters search, string filter)
        {
            switch (search)
            {
                case SearchFilters.Contains:
                    filtred = filtred.Where((m) => m.TextBody != null ? m.TextBody.Contains(filter) : m.HtmlBody != null ? m.TextBody.Contains(filter) : false).ToList();
                    break;
                case SearchFilters.Longer:
                    filtred = filtred.Where((m) => m.TextBody != null ? m.TextBody.Length > Int32.Parse(filter) : m.HtmlBody != null ? m.HtmlBody.Length > Int32.Parse(filter) : false).ToList();
                    break;
                case SearchFilters.Shorter:
                    // filtred = filtred.Where((m) => m.TextBody.Length < Int32.Parse(filter)).ToList();
                    filtred = filtred.Where((m) =>
                         m.TextBody != null ? m.TextBody.Length < Int32.Parse(filter) : m.HtmlBody != null ? m.HtmlBody.Length < Int32.Parse(filter) : false).ToList();
                    break;
                case SearchFilters.HasAttachments:
                    filtred = filtred.Where((m) => m.Attachments.LongCount() > 0).ToList();
                    break;
            }
        }
        /// <summary>
        /// Searches addreses by specfied filter
        /// </summary>
        /// <param name="search"></param>
        /// <param name="filter"></param>
        private void FilterFromBy(SearchFilters search, string filter)
        {
            switch (search)
            {
                case SearchFilters.Contains:
                    filtred = filtred.Where((m) => GetString(m.From).Contains(filter)).ToList();
                    break;
            }
        }
        /// <summary>
        /// Searches dates by specfied filter
        /// </summary>
        /// <param name="search"></param>
        /// <param name="filter"></param>
        private void FilterDateBy(SearchFilters search, string filter)
        {
            switch (search)
            {
                case SearchFilters.After:
                    filtred = filtred.Where((m) => m.Date.Date > DateTime.Parse(filter)).ToList();
                    break;
                case SearchFilters.Before:
                    filtred = filtred.Where((m) => m.Date.Date < DateTime.Parse(filter)).ToList();
                    break;
            }
        }
        #endregion


        /// <summary>
        /// returns mailaddreses mailbox list as a single string
        /// </summary>
        /// <param name="addresses"></param>
        /// <returns></returns>
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
