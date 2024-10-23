using System;
using System.Drawing;
using System.Windows.Forms;

namespace PS
{
    public partial class Form1 : Form
    {
        public enum state
        {
            On,
            Off,
            P1,
            P2,
            P3,
            P1P2,
            P1P3,
            P2P3,
            Alarm
        }
        state status = state.Off;

        public int counter = 0;
        public int flow = 0;
        public int level = 0;
        public int pump_flow = 2;
        public int start_counter = 0;
        public int start_alarm = 0;
        public int exstate = 0;

        public int usage_p1 = 0;
        public int usage_p2 = 0;
        bool color = true;
        bool malP1 = false;
        bool malP2 = false;

        public int b1 = 5;
        public int b2 = 10;
        public int b3 = 20;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Open a new console window for logging
            AllocConsole();
            Console.WriteLine("Monitoring and Control System Console");
            Console.WriteLine("-------------------------------------");
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (level >= b1)
                B1.BackColor = Color.Green;
            else B1.BackColor = Color.Red;
            if (level >= b2)
                B2.BackColor = Color.Green;
            else B2.BackColor = Color.Red;
            if (level >= b3)
                B3.BackColor = Color.Green;
            else B3.BackColor = Color.Red;

            switch (status)
            {
                case state.On:
                    level = level + flow;
                    if (level >= b2)
                    {
                        start_counter = counter;
                        if (malP1 && malP2)
                            status = state.P3;
                        else
                        {
                            if (malP1)
                                status = state.P2;
                            else
                            {
                                if (malP2)
                                    status = state.P1;
                                else
                                {
                                    if (usage_p1 < usage_p2)
                                        status = state.P1;
                                    else
                                        status = state.P2;
                                }
                            }
                        }
                    }
                    P1.BackColor = Color.Red;
                    P2.BackColor = Color.Red;
                    P3.BackColor = Color.Red;
                    K1.BackColor = Color.Green;
                    Console.WriteLine($"[INFO] System turned ON. Level: {level}, Flow: {flow}");
                    break;
                case state.P1:
                    usage_p1++;
                    level = level - pump_flow + flow;
                    if (level <= b1)
                        status = state.On;
                    if (counter - start_counter == 6)
                    {
                        if (malP2)
                            status = state.P1P3;
                        else
                            status = state.P1P2;
                    }
                    P1.BackColor = Color.Green;
                    P2.BackColor = Color.Red;
                    P3.BackColor = Color.Red;
                    Console.WriteLine($"[INFO] Pump P1 running. Level: {level}, Usage P1: {usage_p1}");
                    break;
                case state.P2:
                    usage_p2++;
                    level = level - pump_flow + flow;
                    if (level <= b1)
                        status = state.On;
                    if (counter - start_counter == 6)
                    {
                        if (malP1)
                            status = state.P2P3;
                        else
                            status = state.P1P2;
                    }
                    P2.BackColor = Color.Green;
                    P3.BackColor = Color.Red;
                    P1.BackColor = Color.Red;
                    Console.WriteLine($"[INFO] Pump P2 running. Level: {level}, Usage P2: {usage_p2}");
                    break;
                case state.P3:
                    P3.BackColor = Color.Green;
                    P2.BackColor = Color.Red;
                    P1.BackColor = Color.Red;
                    level = level - pump_flow + flow;
                    if (level >= b3)
                    {
                        status = state.Alarm;
                        start_alarm = counter;
                        exstate = 1;
                        Console.WriteLine($"[ALERT] High level detected! Initiating Alarm. Level: {level}");
                    }
                    if (level <= b1)
                        status = state.On;
                    break;
                case state.P1P2:
                    usage_p1++;
                    usage_p2++;
                    P1.BackColor = Color.Green;
                    P2.BackColor = Color.Green;
                    P3.BackColor = Color.Red;
                    level = level - 2 * pump_flow + flow;
                    if (level <= b1)
                        status = state.On;
                    if (level >= b3)
                    {
                        status = state.Alarm;
                        start_alarm = counter;
                        exstate = 2;
                        Console.WriteLine($"[ALERT] High level detected! Initiating Alarm. Level: {level}");
                    }
                    Console.WriteLine($"[INFO] Pumps P1 and P2 running. Level: {level}, Usage P1: {usage_p1}, Usage P2: {usage_p2}");
                    break;
                case state.P1P3:
                    if (color)
                    {
                        P2.BackColor = Color.Green;
                    }
                    else
                    {
                        P2.BackColor = Color.Red;
                    }
                    color = !color;
                    usage_p1++;
                    P3.BackColor = Color.Green;
                    P1.BackColor = Color.Green;
                    level = level - 2 * pump_flow + flow;
                    if (level <= b1)
                        status = state.On;
                    if (level >= b3)
                    {
                        status = state.Alarm;
                        start_alarm = counter;
                        exstate = 2;
                        Console.WriteLine($"[ALERT] High level detected! Initiating Alarm. Level: {level}");
                    }
                    Console.WriteLine($"[INFO] Pumps P1 and P3 running. Level: {level}, Usage P1: {usage_p1}");
                    break;
                case state.P2P3:
                    if (color)
                    {
                        P1.BackColor = Color.Green;
                    }
                    else
                    {
                        P1.BackColor = Color.Red;
                    }
                    color = !color;
                    usage_p2++;
                    P3.BackColor = Color.Green;
                    P2.BackColor = Color.Green;
                    level = level - 2 * pump_flow + flow;
                    if (level <= b1)
                        status = state.On;
                    if (level >= b3)
                    {
                        status = state.Alarm;
                        start_alarm = counter;
                        exstate = 2;
                        Console.WriteLine($"[ALERT] High level detected! Initiating Alarm. Level: {level}");
                    }
                    Console.WriteLine($"[INFO] Pumps P2 and P3 running. Level: {level}, Usage P2: {usage_p2}");
                    break;
                case state.Alarm:
                    K1.BackColor = Color.Red;
                    Alarm.BackColor = Color.Red;
                    if (counter - start_alarm == 10)
                    {
                        Alarm.BackColor = Color.Black;
                        status = state.On;
                        Console.WriteLine("[INFO] Alarm reset.");
                    }
                    level = level - exstate * pump_flow;
                    Console.WriteLine($"[ALERT] Alarm active! Level: {level}");
                    break;
            }
            counter++;
            level_bar.Value = level;
            level_label.Text = level.ToString();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            status = state.On;
            flow = potentiometer.Value;
            counter = 0;
            flow_prop.Text = flow.ToString();
            timer.Start();
            Console.WriteLine("[INFO] System started.");
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            status = state.Off;
            this.Close();
            Console.WriteLine("[INFO] System stopped.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.BackColor = Color.Green;
            malP1 = true;
            if (status == state.P1 && malP2)
                status = state.P3;
            else if (status == state.P1 && !malP2)
                status = state.P2;
            if (status == state.P1P2)
                status = state.P2P3;
            Console.WriteLine("[INFO] Malfunction in Pump P1 triggered.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.BackColor = Color.Green;
            malP2 = true;
            if (status == state.P2 && malP1)
                status = state.P3;
            else if (status == state.P2 && !malP1)
                status = state.P1;
            if (status == state.P1P2)
                status = state.P1P3;
            Console.WriteLine("[INFO] Malfunction in Pump P2 triggered.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.BackColor = Color.Red;
            button2.BackColor = Color.Red;
            malP1 = false;
            malP2 = false;
            if (status == state.P1P3 || status == state.P2P3)
                status = state.P1P2;
            if (status == state.P3)
            {
                if (usage_p1 < usage_p2)
                    status = state.P1;
                else
                    status = state.P2;
            }
            Console.WriteLine("[INFO] Malfunctions reset.");
        }
    }
}
