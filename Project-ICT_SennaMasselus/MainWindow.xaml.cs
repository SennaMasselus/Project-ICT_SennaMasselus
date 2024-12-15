using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace Project_ICT_SennaMasselus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        SerialPort port = new SerialPort(); // Maak een nieuwe seriële poort
        DispatcherTimer timer = new DispatcherTimer();
        StartFlow startFlow = new StartFlow();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            ShowImage_EmptyGlass();
            ConnectionTextBlock.Text = "Disconnected"; // Change the text
            ConnectionTextBlock.Foreground = new SolidColorBrush(Colors.Red); // Change the foreground color

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;

            port.DataReceived += SerialPort_DataReceived;
            string[] ports = SerialPort.GetPortNames(); // Haal alle poorten op
            foreach (string poort in ports) lstports.Items.Add(poort); // Voeg alle poorten toe aan de lijst
            port.PortName = "COM"; // Stel de naam van de poort in op COM, dit is de standaardwaarde als er geen poort is geopend
        }
        private void lstports_SelectionChanged(object sender, SelectionChangedEventArgs e) // Wordt uitgevoerd wanneer een item in de lijst wordt geselecteerd
        {
            if (lstports.SelectedIndex == -1) return; // Controleer of er een item is geselecteerd
            if (lstports.SelectedItem.ToString() == port.PortName) return; // Controleer of het geselecteerde item al de huidige poort is
            port.Close(); // Sluit de huidige poort
            port.PortName = lstports.SelectedItem.ToString(); // Stel de nieuwe poortnaam in
            try
            {
                port.Open(); // Open de nieuwe poort
                ConnectionTextBlock.Text = "Connected"; // Change the text
                ConnectionTextBlock.Foreground = new SolidColorBrush(Colors.Green); // Change the foreground color
            }
            catch
            {
                // Toon een foutmelding als het openen van de poort mislukt
                MessageBox.Show("Mislukt, selecteer een andere poort", "Fout");
                Disconnect(); // Verbreek de verbinding
            }
        }
        private void Disconnect_btn_Click(object sender, RoutedEventArgs e)
        {
            if (port.IsOpen)
            {
                Disconnect();
                RemoveImage();
                ShowImage_EmptyGlass();
                MessageBox.Show("Succesfully Disconnected.");
            }
            else
            {
                MessageBox.Show("Already Disconnected");
            }
        }

        private void Disconnect() // Methode om de verbinding te verbreken
        {
                // Try-catch niet nodig, omdat er geen fouten kunnen optreden bij het sluiten van de poort
                timer.Stop(); pbTijd.Value = 0;
                port.Write("\u0000");
                port.Close(); // Sluit de poort
                port.PortName = "COM"; // Stel de poortnaam in op COM, dit is de standaardwaarde
                lstports.SelectedIndex = -1; // Dit zorgt ervoor dat er niets is geselecteerd
                ConnectionTextBlock.Text = "Disconnected"; // Change the text
                ConnectionTextBlock.Foreground = new SolidColorBrush(Colors.Red); // Change the foreground color
        }



        private void FlowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                startFlow.Start_Flow(port);
                if (port.IsOpen)
                {
                    timer.Start();
                }
                else
                {
                    MessageBox.Show("Not Connected.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while sending the signal: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
                pbTijd.Value++;
                if (pbTijd.Value < 4)
                {
                    RemoveImage();
                    ShowImage_EmptyGlass();
                }

                if (pbTijd.Value > 3 && pbTijd.Value < 14)
                {
                    RemoveImage();
                    ShowImage_MidGlass();
                }
                if (pbTijd.Value > 13 && pbTijd.Value <= 15)
                {
                    RemoveImage();
                    ShowImage_FullGlass();
                }

                if (pbTijd.Value == 15)
                {
                    timer.Stop();
                    port.Write("\u0000");
                    RemoveImage();
                    ShowImage_EmptyGlass();
                    pbTijd.Value = 0;
                    if (MessageBox.Show("The glass has been filled.") == MessageBoxResult.OK) ;
                }
        }

        private void ShowImage_EmptyGlass()
        {
            // Create a new Image control
            Image myImage = new Image();

            // Set the image source
            myImage.Source = new BitmapImage(new Uri("C:\\Users\\masse\\Desktop\\Project-ICT_SennaMasselus\\Project-ICT_SennaMasselus\\Project-ICT_SennaMasselus\\Resources\\Glass_Empty.png"));

            // Position the image in the Canvas
            Canvas.SetLeft(myImage, 38);
            Canvas.SetTop(myImage, 80);

            // Add the image to the Canvas
            Interface.Children.Add(myImage);
        }
        private void ShowImage_MidGlass()
        {
            // Create a new Image control
            Image myImage = new Image();

            // Set the image source
            myImage.Source = new BitmapImage(new Uri("C:\\Users\\masse\\Desktop\\Project-ICT_SennaMasselus\\Project-ICT_SennaMasselus\\Project-ICT_SennaMasselus\\Resources\\Glass_Mid.png"));

            // Position the image in the Canvas
            Canvas.SetLeft(myImage, 40);
            Canvas.SetTop(myImage, 77);

            // Add the image to the Canvas
            Interface.Children.Add(myImage);
        }
        private void ShowImage_FullGlass()
        {
            // Create a new Image control
            Image myImage = new Image();

            // Set the image source
            myImage.Source = new BitmapImage(new Uri("C:\\Users\\masse\\Desktop\\Project-ICT_SennaMasselus\\Project-ICT_SennaMasselus\\Project-ICT_SennaMasselus\\Resources\\Glass_Full.png"));

            // Position the image in the Canvas
            Canvas.SetLeft(myImage, 44.5);
            Canvas.SetTop(myImage, 78.5);

            // Add the image to the Canvas
            Interface.Children.Add(myImage);
        }
        private void RemoveImage()
        {
            // Find and remove the image from the Canvas
            var imageToRemove = Interface.Children.OfType<Image>().FirstOrDefault();

            if (imageToRemove != null)
            {
                Interface.Children.Remove(imageToRemove);
            }
        }


        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = port.ReadLine().Replace("\r", "").Replace("\n", "");
            Dispatcher.Invoke(() =>
            {
                if (data == "Sensor_Covered") FlowButton.IsEnabled = true;
                else FlowButton.IsEnabled = false;
            });
        }
    }
}