using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace EV3Controller
{
    public partial class MainWindow : Window
    {
        // Initialise objects and variables
        Brick brick;
        string COMPort;
        int driveSpeed, turnSpeed, aM1Speed, aM2Speed;
        bool run, fwd, bwd, left, right, aM1, aM1i, aM2, aM2i, eStop;
        OutputPort leftDrive, rightDrive, aMotor1, aMotor2;
        OutputPort[] ports = { OutputPort.A, OutputPort.B, OutputPort.C, OutputPort.D };

        // Initialise keybinds
        Key keyF = Key.W;
        Key keyB = Key.S;
        Key keyL = Key.A;
        Key keyR = Key.D;
        Key keyaM1 = Key.U;
        Key keyaM1i = Key.I;
        Key keyaM2 = Key.J;
        Key keyaM2i = Key.K;

        // Key HELL (there's gotta be a better way but i cbf rn)
        private void FWD_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void BWD_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void Left_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void Right_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void AM1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void AM1i_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void AM2_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void AM2i_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }

        // Left drive port select
        private void LeftDrivePort_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            leftDrive = ports[LeftDrivePort.SelectedIndex];
        }
        // Right drive port select
        private void RightDrivePort_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            rightDrive = ports[RightDrivePort.SelectedIndex];
        }
        // Additional motor 1 port select
        private void AMotor1Port_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            aMotor1 = ports[AMotor1Port.SelectedIndex];
        }
        // Additional motor 2 port select
        private void AMotor2Port_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            aMotor2 = ports[AMotor2Port.SelectedIndex];
        }

        // Turn program on/off
        private void On_Off_Click(object sender, RoutedEventArgs e)
        {
            if (run)
            {
                run = false;
                On_Off.Background = Brushes.Red;

            }
            else
            {
                run = true;
                On_Off.Background = Brushes.Green;
                Connect_EV3();
            }
        }

        // Emergency stop
        private async void E_Stop_Click(object sender, RoutedEventArgs e)
        {
            if (eStop)
            {
                eStop = false;
                E_Stop.Background = Brushes.Red;
            }
            else
            {
                eStop = true;
                E_Stop.Background = Brushes.Green;
                await brick.DirectCommand.StopMotorAsync(OutputPort.All, true);
                await brick.DirectCommand.TurnMotorAtPowerAsync(OutputPort.All, 0);
            }
            
        }

        // Connect to EV3
        private async void Connect_EV3()
        {
            try
            {
                brick = new Brick(new BluetoothCommunication(COMPort));
                await brick.ConnectAsync();

                await brick.DirectCommand.PlayToneAsync(10, 400, 300);
                System.Threading.Thread.Sleep(300);
                await brick.DirectCommand.PlayToneAsync(10, 800, 300);
            }
            catch (System.IO.IOException)
            {
                run = false;
            }
        }

        // Change drive speed
        private void Drive_Speed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                driveSpeed = int.Parse(Drive_Speed.Text);
                Drive_Speed.Background = Brushes.White;
            }
            catch (System.FormatException)
            {
                Drive_Speed.Background = Brushes.Red;

            }
        }

        // Change turn speed
        private void Turn_Speed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                turnSpeed = int.Parse(Turn_Speed.Text);
                Turn_Speed.Background = Brushes.White;
            }
            catch (System.FormatException)
            {
                Turn_Speed.Background = Brushes.Red;
            }
        }

        // Change additional motor 1 speed
        private void AM1_Speed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                aM1Speed = int.Parse(AM1_Speed.Text);
                AM1_Speed.Background = Brushes.White;
            }
            catch (System.FormatException)
            {
                AM1_Speed.Background = Brushes.Red;
            }
        }

        // Change additional motor 2 speed
        private void AM2_Speed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                aM2Speed = int.Parse(AM2_Speed.Text);
                AM2_Speed.Background = Brushes.White;
            }
            catch (System.FormatException)
            {
                AM2_Speed.Background = Brushes.Red;
            }
        }

        // Change COM port
        private void COM_Port_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                COMPort = "COM" + int.Parse(COM_Port.Text).ToString();
                COM_Port.Background = Brushes.White;
            }
            catch (System.FormatException)
            {
                COM_Port.Background = Brushes.Red;
            }
        }

        // Initialise window
        public MainWindow()
        {
            InitializeComponent();
        }

        // Drive commands
        private async void Drive()
        {
            if (run && !eStop)
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
            }
        }
        
        // Additional motor commands
        private async void MotorControl()
        {
            if (run && !eStop)
            {
                // Additional Motor 1
                if (aM1 && !aM1i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor1, aM1Speed);
                }
                // Additional Motor 1 (inverse)
                if (!aM1 && aM1i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor1, -aM1Speed);
                }
                // Additional Motor 2
                if (aM2 && !aM2i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor2, aM2Speed);
                }
                // Additional Motor 2 (inverse)
                if (!aM2 && aM2i)
                {
                    await brick.DirectCommand.TurnMotorAtPowerAsync(aMotor2, -aM2Speed);
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
            Drive();
            MotorControl();
        }
    }
}
