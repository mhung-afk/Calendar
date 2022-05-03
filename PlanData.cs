using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    [Serializable]
    public class PlanData
    {
        private List<PlanItem> listItems;

        public List<PlanItem> ListItems { get => listItems; set => listItems = value; }

    }
}
