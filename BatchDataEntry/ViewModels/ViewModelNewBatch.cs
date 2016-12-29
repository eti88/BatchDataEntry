using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatchDataEntry.Models;
using NLog;

namespace BatchDataEntry.ViewModels
{
    class ViewModelNewBatch : ViewModelBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ViewModelNewBatch()
        {
            
        }

        public ViewModelNewBatch(Batch batch)
        {
            
        }
    }
}
