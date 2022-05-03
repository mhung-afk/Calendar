using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    public partial class AJob : UserControl
    {
        private PlanItem item;
        public PlanItem Item { get => item; set => item = value; }

        private event EventHandler deleted;
        public event EventHandler Deleted
        {
            add { deleted += value; }
            remove { deleted -= value; }
        }

        public AJob(PlanItem item)
        {
            InitializeComponent();
            Item = item;
            cbStatus.DataSource = PlanItem.ListStatus;
            ShowInfo();
        }

        private void ShowInfo()
        {
            txbJob.Text = Item.Job;
            nmFromHours.Value = Item.StartTime.X;
            nmFromMinute.Value = Item.StartTime.Y;
            nmToHours.Value = Item.EndTime.X;
            nmToMinute.Value = Item.EndTime.Y;
            cbStatus.SelectedIndex = PlanItem.ListStatus.IndexOf(Item.Status);
            ckbDone.Checked = PlanItem.ListStatus.IndexOf(Item.Status) == (int)PlanItem.EPlanItem.DONE ? true : false;
        }

        private void ckbDone_CheckedChanged(object sender, EventArgs e)
        {
            cbStatus.SelectedIndex = ckbDone.Checked ? (int)PlanItem.EPlanItem.DONE : PlanItem.ListStatus.IndexOf(Item.Status);
            if (ckbDone.Checked == true)
            {
                panel1.BackColor = Color.Aqua;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Item.Job = txbJob.Text;
            Item.StartTime = new Point((int)nmFromHours.Value, (int)nmFromMinute.Value);
            Item.EndTime = new Point((int)nmToHours.Value, (int)nmToMinute.Value);
            if(cbStatus.SelectedIndex != -1)
                Item.Status = cbStatus.SelectedItem.ToString();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(deleted != null)
            {
                deleted(this, new EventArgs());
            }
        }

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbStatus.SelectedIndex == (int)PlanItem.EPlanItem.MISSED)
            {
                panel1.BackColor = Color.Red;
            }
            else if(cbStatus.SelectedIndex == (int)PlanItem.EPlanItem.DONE)
            {
                ckbDone.Checked = true;
            }
            else if (cbStatus.SelectedIndex == (int)PlanItem.EPlanItem.DOING)
            {
                panel1.BackColor = Color.LightGreen;
            }
            else
            {
                panel1.BackColor = Color.White;
            }
        }
    }
}
