using System;
using System.Windows;
using System.Windows.Input;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace EV3Controller
{
    public partial class MainWindow : Window
    {
        // Initialise objects and variables
        Brick brick;
        string COMPort;
        int driveSpeed;
        int turnSpeed;
        bool run, fwd, bwd, left, right, aM1, aM1i, aM2, aM2i, mStop;

        Key keyF = Key.W;
        Key keyB = Key.S;
        Key keyL = Key.A;
        Key keyR = Key.D;
        Key keyaM1 = Key.U;
        Key keyaM1i = Key.I;
        Key keyaM2 = Key.J;
        Key keyaM2i = Key.K;
        Key keyStop = Key.Space;

        OutputPort leftDrive = OutputPort.B;
        OutputPort rightDrive = OutputPort.C;
        OutputPort aMotor1 = OutputPort.A;
        OutputPort aMotor2 = OutputPort.D;

        // Turn program on/off
        private void On_Off_Click(object sender, RoutedEventArgs e)
        {
            if (run)
            {
                run = false;
                Console.WriteLine("Program is OFF");
            }
            else
            {
                run = true;
                Console.WriteLine("Program is ON");
                Connect_EV3();
            }
        }

        // Connect to EV3
        private async void Connect_EV3()
        {
            try
            {
                brick = new Brick(new BluetoothCommunication(COMPort));
                await brick.ConnectAsync();
                System.Console.WriteLine("Brick connection SUCCESSFUL");

                await brick.DirectCommand.PlayToneAsync(10, 400, 300);
                System.Threading.Thread.Sleep(300);
                await brick.DirectCommand.PlayToneAsync(10, 800, 300);
            }
            catch (System.IO.IOException)
            {
                run = false;
                Console.WriteLine("Brick connection FAILED");
            }
        }

        // Change drive speed
        private void Drive_Speed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            driveSpeed = int.Parse(Drive_Speed.Text);
        }

        // Change turn speed
        private void Turn_Speed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            turnSpeed = int.Parse(Turn_Speed.Text);
        }

        // Change COM port
        private void COM_Port_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            COMPort = "COM" + COM_Port.Text;
        }

        // Initialise window
        public MainWindow()
        {
            InitializeComponent();
        }

        // Drive commands
        private async void Drive()
        {
            if (run)
            {
                // Forward
                if (fwd && !bwd && !left && !right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive | rightDrive, driveSpeed);
                }
                // Backward
                if (!fwd && bwd && !left && !right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive | rightDrive, -driveSpeed);
                }
                // Pivot Left
                if (!fwd && !bwd && left && !right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, -turnSpeed);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, turnSpeed);
                }
                // Pivot Right
                if (!fwd && !bwd && !left && right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, turnSpeed);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, -turnSpeed);
                }
                // Curve Left Forwards
                if (fwd && !bwd && left && !right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, driveSpeed - turnSpeed);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, driveSpeed);
                }
                // Curve Right Forwards
                if (fwd && !bwd && !left && right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, driveSpeed);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, driveSpeed - turnSpeed);
                }
                // Curve Left Backwards
                if (!fwd && bwd && !left && right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, -driveSpeed + turnSpeed);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, -driveSpeed);
                }
                // Curve Right Backwards
                if (!fwd && bwd && left && !right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive, -driveSpeed);
                    await brick.DirectCommand.TurnMotorAtPowerAsync(rightDrive, -driveSpeed + turnSpeed);
                }
                // No Drive
                if (!fwd && !bwd && !left && !right)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(leftDrive | rightDrive, 0);
                }
                // Stop Motors
                if (mStop)
                {
                    Console.WriteLine("Motors stopped");
                    await brick.DirectCommand.StopMotorAsync(OutputPort.All, true);
                }
            }
        }
        
        // Additional motor commands
        private async void MotorControl()
        {
            if (run)
            {
                // Additional Motor 1
                if (aM1 && !aM1i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor1, turnSpeed);
                }
                // Additional Motor 1 (inverse)
                if (!aM1 && aM1i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor1, -turnSpeed);
                }
                // Additional Motor 2
                if (aM2 && !aM2i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor2, turnSpeed);
                }
                // Additional Motor 2 (inverse)
                if (!aM2 && aM2i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor2, -turnSpeed);
                }
                // No Additional Motor 1
                if (!aM1 && !aM1i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor1, 0);
                }
                // No Additional Motor 2
                if (!aM2 && !aM2i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor2, 0);
                }
            }
        }

        // Key pressed down
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Forward
            if (e.Key == keyF)
            {
                fwd = true;
            }
            // Backward
            if (e.Key == keyB)
            {
                bwd = true;
            }
            // Left
            if (e.Key == keyL)
            {
                left = true;
            }
            // Right
            if (e.Key == keyR)
            {
                right = true;
            }
            // Additional Motor 1
            if (e.Key == keyaM1)
            {
                aM1 = true;
            }
            // Additional Motor 1 (inverse)
            if (e.Key == keyaM1i)
            {
                aM1i = true;
            }
            // Additional Motor 2
            if (e.Key == keyaM2)
            {
                aM2 = true;
            }
            // Additional Motor 2 (inverse)
            if (e.Key == keyaM2i)
            {
                aM2i = true;
            }
            // Stop all
            if (e.Key == keyStop)
            {
                mStop = true;
            }
            Drive();
            MotorControl();
        }

        // Key pressed up
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Forward
            if (e.Key == keyF)
            {
                fwd = false;
            }
            // Backward
            if (e.Key == keyB)
            {
                bwd = false;
            }
            // Left
            if (e.Key == keyL)
            {
                left = false;
            }
            // Right
            if (e.Key == keyR)
            {
                right = false;
            }
            // Additional Motor 1
            if (e.Key == keyaM1)
            {
                aM1 = false;
            }
            // Additional Motor 1 (inverse)
            if (e.Key == keyaM1i)
            {
                aM1i = false;
            }
            // Additional Motor 2
            if (e.Key == keyaM2)
            {
                aM2 = false;
            }
            // Additional Motor 2 (inverse)
            if (e.Key == keyaM2i)
            {
                aM2i = false;
            }
            // Stop all
            if (e.Key == keyStop)
            {
                mStop = false;
            }
            Drive();
            MotorControl();
        }
    }
}
