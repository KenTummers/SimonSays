using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SimonSays
{
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();

            Init();

            while (idle)
            {
                Idlestance();
            }

            while (game)
            {
                Game();
            }
        }

        // Assigns the pins to the LEDs and buttons

        private const int REDLED_PIN = 5;
        private const int YELLOWLED_PIN = 6;
        private const int GREENLED_PIN = 13;
        private const int BLUELED_PIN = 19;
        private const int REDBUTTON_PIN = 12;
        private const int YELLOWBUTTON_PIN = 16;
        private const int GREENBUTTON_PIN = 20;
        private const int BLUEBUTTON_PIN = 21;

        private GpioPin redledPin;
        private GpioPin yellowledPin;
        private GpioPin greenledPin;
        private GpioPin blueledPin;
        private GpioPin redbuttonPin;
        private GpioPin yellowbuttonPin;
        private GpioPin greenbuttonPin;
        private GpioPin bluebuttonPin;

        // Makes lists for the code
        List<int> computercode = new List<int>();
        List<int> usercode = new List<int>();
        List<int> leds = new List<int>(new int[] { REDLED_PIN, YELLOWLED_PIN, GREENLED_PIN, BLUELED_PIN });

        bool idle = true;
        bool game = false;

        private void Init()
        {
            var gpio = GpioController.GetDefault();

            redledPin = gpio.OpenPin(REDLED_PIN);
            yellowledPin = gpio.OpenPin(YELLOWLED_PIN);
            greenledPin = gpio.OpenPin(GREENLED_PIN);
            blueledPin = gpio.OpenPin(BLUELED_PIN);
            redbuttonPin = gpio.OpenPin(REDBUTTON_PIN);
            yellowbuttonPin = gpio.OpenPin(YELLOWBUTTON_PIN);
            greenbuttonPin = gpio.OpenPin(GREENBUTTON_PIN);
            bluebuttonPin = gpio.OpenPin(BLUEBUTTON_PIN);

            redledPin.SetDriveMode(GpioPinDriveMode.Output);
            yellowledPin.SetDriveMode(GpioPinDriveMode.Output);
            greenledPin.SetDriveMode(GpioPinDriveMode.Output);
            blueledPin.SetDriveMode(GpioPinDriveMode.Output);
            redbuttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            yellowbuttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            greenbuttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            bluebuttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);

            redbuttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            yellowbuttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            greenbuttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            bluebuttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            redbuttonPin.ValueChanged += ButtonPressed;
            yellowbuttonPin.ValueChanged += ButtonPressed;
            greenbuttonPin.ValueChanged += ButtonPressed;
            bluebuttonPin.ValueChanged += ButtonPressed;

            redbuttonPin.ValueChanged += Input;
            yellowbuttonPin.ValueChanged += Input;
            greenbuttonPin.ValueChanged += Input;
            bluebuttonPin.ValueChanged += Input;


        }

        private void Idlestance()
        {
            // Loops the idle stance until a button is pressed
            redledPin.Write(GpioPinValue.Low);
            Task.Delay(500).Wait();
            redledPin.Write(GpioPinValue.High);
            yellowledPin.Write(GpioPinValue.Low);
            Task.Delay(500).Wait();
            yellowledPin.Write(GpioPinValue.High);
            greenledPin.Write(GpioPinValue.Low);
            Task.Delay(500).Wait();
            greenledPin.Write(GpioPinValue.High);
            blueledPin.Write(GpioPinValue.Low);
            Task.Delay(500).Wait();
            blueledPin.Write(GpioPinValue.High);
        }

        private void ButtonPressed(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            // Turns off the idle stance when any button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                idle = false;
                game = true;
                usercode.Clear();
            }
        }

        private void Blink()
        {
            // All LEDs blink 5 times
            for (int i = 0; i < 5; i++)
            {
                redledPin.Write(GpioPinValue.Low);
                yellowledPin.Write(GpioPinValue.Low);
                greenledPin.Write(GpioPinValue.Low);
                blueledPin.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                redledPin.Write(GpioPinValue.High);
                yellowledPin.Write(GpioPinValue.High);
                greenledPin.Write(GpioPinValue.High);
                blueledPin.Write(GpioPinValue.High);
                Task.Delay(200).Wait();
            }
        }

        private void RandomLED()
        {
            // A random LED gets chosen and this one will light up
            Random random = new Random();
            int randomNumber = random.Next(0, 4);
            if (leds[randomNumber] == 5)
                redledPin.Write(GpioPinValue.Low);
                Task.Delay(1500).Wait();
                redledPin.Write(GpioPinValue.High);

            if (leds[randomNumber] == 6)
                yellowledPin.Write(GpioPinValue.Low);
                Task.Delay(1500).Wait();
                yellowledPin.Write(GpioPinValue.High);

            if (leds[randomNumber] == 13)
                greenledPin.Write(GpioPinValue.Low);
                Task.Delay(1500).Wait();
                greenledPin.Write(GpioPinValue.High);

            if (leds[randomNumber] == 19)
                blueledPin.Write(GpioPinValue.Low);
                Task.Delay(1500).Wait();
                blueledPin.Write(GpioPinValue.High);

            computercode.Add(leds[randomNumber]);
        }

        private void Input(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (sender == redbuttonPin)
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    usercode.Add(REDLED_PIN);
                }

            if (sender == yellowbuttonPin)
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    usercode.Add(YELLOWLED_PIN);
                }

            if (sender == greenbuttonPin)
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    usercode.Add(GREENLED_PIN);
                }

            if (sender == bluebuttonPin)
                if (e.Edge == GpioPinEdge.FallingEdge)
                {
                    usercode.Add(BLUELED_PIN);
                }
        }

        private void CheckInput()
        {
            // Checks if the input from the user is the same as the code from the computer
            if (computercode == usercode)
            {
                redledPin.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                redledPin.Write(GpioPinValue.High);
                yellowledPin.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                yellowledPin.Write(GpioPinValue.High);
                greenledPin.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                greenledPin.Write(GpioPinValue.High);
                blueledPin.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                blueledPin.Write(GpioPinValue.High);
                greenledPin.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                greenledPin.Write(GpioPinValue.High);
                yellowledPin.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                yellowledPin.Write(GpioPinValue.High);
                redledPin.Write(GpioPinValue.Low);
                Task.Delay(200).Wait();
                redledPin.Write(GpioPinValue.High);
            }

            else
            {
                for (int i = 0; i < 3; i++)
                {
                    redledPin.Write(GpioPinValue.Low);
                    yellowledPin.Write(GpioPinValue.Low);
                    greenledPin.Write(GpioPinValue.Low);
                    blueledPin.Write(GpioPinValue.Low);
                    Task.Delay(200).Wait();
                    redledPin.Write(GpioPinValue.High);
                    yellowledPin.Write(GpioPinValue.High);
                    greenledPin.Write(GpioPinValue.High);
                    blueledPin.Write(GpioPinValue.High);
                    Task.Delay(200).Wait();
                }
                computercode.Clear();
                usercode.Clear();
                game = false;
                idle = true;
            }
        }

        private void Game()
        {
            Blink();
            RandomLED();
            CheckInput();
        }
    }
}
