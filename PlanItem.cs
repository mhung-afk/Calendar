using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar
{
    public class PlanItem
    {
        private DateTime date;
        private string job;
        private Point startTime;
        private Point endTime;
        private string status;
        public static List<string> ListStatus = new List<string>()
        {
            "DONE",
            "DOING",
            "COMING",
            "MISSED"
        };

        public string Job { get => job; set => job = value; }
        public Point StartTime { get => startTime; set => startTime = value; }
        public Point EndTime { get => endTime; set => endTime = value; }
        public string Status { get => status; set => status = value; }
        public DateTime Date { get => date; set => date = value; }

        public enum EPlanItem
        {
            DONE,
            DOING,
            COMING,
            MISSED
        }
    }
}
