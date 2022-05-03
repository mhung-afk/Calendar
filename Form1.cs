using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Calendar
{
    public partial class Form1 : Form
    {
        private List<List<Button>> matrix;
        private List<string> dateOfWeek = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        private string filePath = Application.StartupPath + "\\data.xml";
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }
        public PlanData Job { get => job; set => job = value; }

        private PlanData job;
        private int appTime;
        private int quantityOfJob = 0;

        public Form1()
        {
            InitializeComponent();

            RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software\\Calendar");
            //mo registry khoi dong cung win
            RegistryKey regstart = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            string keyvalue = "1";
            //string subkey = "Software\\ManhQuyen";
            try
            {
                //chen gia tri key
                regkey.SetValue("Index", keyvalue);
                //regstart.SetValue("taoregistrytronghethong", "E:\\Studing\\Bai Tap\\CSharp\\Channel 4\\bai temp\\tao registry trong he thong\\tao registry trong he thong\\bin\\Debug\\tao registry trong he thong.exe");
                regstart.SetValue("Calendar", Application.StartupPath + "\\Calendar.exe");
                ////dong tien trinh ghi key
                //regkey.Close();
            }
            catch (System.Exception ex)
            {
            }


            LoadMatrix();
            appTime = 0;
            timNotify.Start();
            this.Enabled = false;
            try
            {
                Job = DeserializeFromXML(filePath) as PlanData;
                this.Enabled = true;
                if(Job != null && Job.ListItems != null)
                    quantityOfJob = Job.ListItems.Count;
            }
            catch 
            {  
                SetDefauleItem();
                this.Enabled = true;
                quantityOfJob = 0;
            }
        }

        private void SetDefauleItem()
        {
            Job = new PlanData();
            Job.ListItems = new List<PlanItem>();
        }

        private void LoadMatrix()
        {
            Matrix = new List<List<Button>>();
            Button oldBtn = new Button() { Height = 0, Width = 0,Location=new Point(-Cons.btnMargin,0) };
            for(int i = 0; i < Cons.DayRow; i++)
            {
                Matrix.Add(new List<Button>());
                for(int j =0; j < Cons.DayCol; j++)
                {
                    Button btn = new Button()
                    {
                        Height = Cons.DayHeight,
                        Width = Cons.DayWidth,
                        Location = new Point(oldBtn.Location.X + oldBtn.Width+Cons.btnMargin, oldBtn.Location.Y),
                    };
                    btn.Click += Btn_Click;
                    Matrix[i].Add(btn);
                    pnMatrix.Controls.Add(btn);
                    oldBtn = btn;
                }
                oldBtn = new Button() { Height = 0, Width = 0, Location = new Point(-Cons.btnMargin, oldBtn.Location.Y + Cons.DayHeight) };
            }
            SetDefaultDate();
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((sender as Button).Text)) return;
            DailyPlan dailyPlan = new DailyPlan(new DateTime(dtpkDate.Value.Year,dtpkDate.Value.Month,Convert.ToInt32((sender as Button).Text)),Job);
            dailyPlan.ShowDialog();
            //SerializeToXML(Job, filePath);
            //this.Enabled = false;
            //try
            //{
            //    Job = DeserializeFromXML(filePath) as PlanData;
            //    this.Enabled = true;
            //}
            //catch
            //{
            //    SetDefauleItem();
            //    this.Enabled = true;
            //}
        }

        private void AddNumberIntoMatrixByDate(DateTime choosedDate)
        {
            ClearMatrix();
            DateTime tempDate = new DateTime(choosedDate.Year, choosedDate.Month, 1);
            int row = 0;
            int col;
            for(int i = 1; i<= DaysInMonth(choosedDate); i++)
            {
                col = dateOfWeek.IndexOf(tempDate.DayOfWeek.ToString());
                Button btn = Matrix[row][col];
                btn.Text = i.ToString();
                if (isSameDate(choosedDate, tempDate)) btn.BackColor = Color.Aqua;
                if (isSameDate(tempDate, DateTime.Today))
                {
                    btn.BackColor = Color.Yellow;
                }
                tempDate = tempDate.AddDays(1);
                if (col >= 6) row++;
            }
        }

        private bool isSameDate(DateTime date1, DateTime date2)
        {
            return (date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day);
        }

        private void ClearMatrix()
        {
            for(int i = 0; i < Matrix.Count; i++)
            {
                for(int j = 0; j < Matrix[i].Count; j++)
                {
                    Button btn = Matrix[i][j];
                    btn.Text = "";
                    btn.BackColor = Color.WhiteSmoke;
                }
            }
        }

        private void SetDefaultDate()
        {
            dtpkDate.Value = DateTime.Now;
        }

        private int DaysInMonth(DateTime date)
        {
            switch (date.Month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 31;
                case 2:
                    if ((date.Year % 4 == 0 && date.Year % 100 != 0) || date.Year % 400 == 0)
                        return 29;
                    else return 28;
                default:
                    return 30;
            }
        }

        private void dtpkDate_ValueChanged(object sender, EventArgs e)
        {
            AddNumberIntoMatrixByDate((sender as DateTimePicker).Value);
        }

        private void btnPreviours_Click(object sender, EventArgs e)
        {
            dtpkDate.Value = dtpkDate.Value.AddMonths(-1);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            dtpkDate.Value = dtpkDate.Value.AddMonths(1);
        }

        private void btnToDay_Click(object sender, EventArgs e)
        {
            SetDefaultDate();
        }

        private void SerializeToXML(object data, string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            XmlSerializer sr = new XmlSerializer(typeof(PlanData));
            sr.Serialize(fs, data);
            fs.Close();
        }

        private object DeserializeFromXML(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                XmlSerializer sr = new XmlSerializer(typeof(PlanData));
                object obj = sr.Deserialize(fs);
                fs.Close();
                return obj;
            }
            catch
            {
                fs.Close();
                throw new NotImplementedException();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            while(Job.ListItems.Count < quantityOfJob)
            {
                PlanItem item = new PlanItem() { Date = dtpkDate.Value.AddDays(-300) };
                Job.ListItems.Add(item);
            }
            SerializeToXML(Job, filePath);
        }

        private void timNotify_Tick(object sender, EventArgs e)
        {
            if (!ckbNotify.Checked) return;

            appTime++;

            if (appTime < Cons.notifyTimeleft) return;

            if (Job == null || Job.ListItems == null) return;

            DateTime current = DateTime.Now;
            List<PlanItem> todayItems = Job.ListItems.Where((p) =>
                p.Date.Year == current.Year
                && p.Date.Month == current.Month
                && p.Date.Day == current.Day
                && p.Status != "DONE"
            ).ToList();
            Notify.ShowBalloonTip(Cons.notifyTimeout, "Calendar", string.Format("Hôm này bạn còn {0} việc phải làm",todayItems.Count), ToolTipIcon.Info);

            appTime = 0;
        }

        private void ckbNotify_CheckedChanged(object sender, EventArgs e)
        {
            nmNotify.Enabled = ckbNotify.Checked;
        }

        private void nmNotify_ValueChanged(object sender, EventArgs e)
        {
            Cons.notifyTimeleft = (int)nmNotify.Value;
        }
    }
}
