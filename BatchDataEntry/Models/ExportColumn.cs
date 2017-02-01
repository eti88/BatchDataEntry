using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Models
{
    public class ExportColumn : BaseModel
    {
        private bool m_checked;
        private String m_text;

        public bool IsChecked
        {
            get { return m_checked; }
            set
            {
                if (m_checked == value) return;
                m_checked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public String Text
        {
            get { return m_text; }
            set
            {
                if (m_text == value) return;
                m_text = value;
                OnPropertyChanged("Text");
            }
        }

        public ExportColumn(String text)
        {
            m_text = text;
            IsChecked = true;
        }

        public ExportColumn(String text, bool status)
        {
            m_text = text;
            IsChecked = status;
        }
    }
}
