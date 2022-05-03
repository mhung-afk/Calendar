using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    public partial class DailyPlan : Form
    {
        DateTime date;
        PlanData job;
        FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();

        public DailyPlan(DateTime date, PlanData job)
        {
            InitializeComponent();
            
            pnlJob.Controls.Add(flowLayoutPanel);
            flowLayoutPanel.Width = pnlJob.Width;
            flowLayoutPanel.Height = pnlJob.Height;
            Job = job;
            Date = date;
            dtpkDate.Value = Date;
        }

        public DateTime Date { get => date; set => date = value; }
        public PlanData Job { get => job; set => job = value; }

        private void ShowJobInDay(DateTime date)
        {
            flowLayoutPanel.Controls.Clear();
            if (Job != null && Job.ListItems != null)
            {
                List<PlanItem> lst = GetJobInDay(date);
                for (int i = 0; i < lst.Count; i++)
                {
                    AddJobItem(lst[i]);
                }
            }
        }

        private void AddJobItem(PlanItem jobItem)
        {
            AJob item = new AJob(jobItem);
            item.Deleted += Item_Deleted;
            flowLayoutPanel.Controls.Add(item);
        }

        private void Item_Deleted(object sender, EventArgs e)
        {
            AJob aJob = sender as AJob;
            PlanItem us = aJob.Item;
            flowLayoutPanel.Controls.Remove(aJob);
            Job.ListItems.Remove(us);
        }

        List<PlanItem> GetJobInDay(DateTime date)
        {
            return Job.ListItems.Where(
                p => p.Date.Year == date.Year
                && p.Date.Month == date.Month
                && p.Date.Day == date.Day
            ).ToList();
        }

        private void dtpkDate_ValueChanged(object sender, EventArgs e)
        {
            ShowJobInDay((sender as DateTimePicker).Value);
        }

        private void btnPrevioursDay_Click(object sender, EventArgs e)
        {
            dtpkDate.Value = dtpkDate.Value.AddDays(-1);
        }

        private void btnNextDay_Click(object sender, EventArgs e)
        {
            dtpkDate.Value = dtpkDate.Value.AddDays(1);
        }

        private void mnsiAddJob_Click(object sender, EventArgs e)
        {
            PlanItem item = new PlanItem() { Date = dtpkDate.Value };
            Job.ListItems.Add(item);
            AddJobItem(item);
        }

        private void mnsiToDay_Click(object sender, EventArgs e)
        {
            dtpkDate.Value = DateTime.Today;
        }
    }
}
