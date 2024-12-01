using System.ComponentModel.Design.Serialization;
using System.Security.Cryptography;
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
using System.Windows.Threading;
using System.Xml.Serialization;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        string combinedString;
        Random rnd = new Random();
        string[] colors = { "rood", "geel", "oranje", "blauw", "wit", "groen" };
        string[] tittleCheck = new string[4];
        private DispatcherTimer timer = new DispatcherTimer();
        bool[] isCorrect = new bool[4];
        int timerTick = 0;
        int attempt = 0;
        int penaltyPoints = 0;
        bool allCorrect = true;
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            GameMesage();
            GenerateCode();
            this.KeyDown += toggleDebug;
        }
        private void codeCheck_Click(object sender, RoutedEventArgs e)
        {
            if (attempt >= 10) { GameMesage(); return; }
            addAttempt();
            StopCountDown();
            StartCountDown();
            ComboBox[] comboBoxes = { comboBox1, comboBox2, comboBox3, comboBox4 };
            Label[] colorLabels = { colorLabel1, colorLabel2, colorLabel3, colorLabel4 };
            StackPanel newStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            bool[] isCorrect = new bool[4];
            for (int i = 0; i < comboBoxes.Length; i++)
            {
                ComboBox comboBox = comboBoxes[i];
                Label colorLabel = colorLabels[i];
                ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    penaltyLabel.Content = $"Strafpunten: {penaltyPoints}";
                    string selectedColor = selectedItem.Content.ToString().Trim();
                    if (selectedColor == tittleCheck[i]) 
                    { 
                        colorLabel.BorderBrush = new SolidColorBrush(Colors.Wheat); 
                        colorLabel.BorderThickness = new Thickness(3); 
                        isCorrect[i] = true;
                    }
                    else if (tittleCheck.Contains(selectedColor)) 
                    { 
                        penaltyPoints += 1; colorLabel.BorderBrush = new SolidColorBrush(Colors.LightBlue); 
                        colorLabel.BorderThickness = new Thickness(3); 
                    }
                    else 
                    { 
                        penaltyPoints += 2; 
                        colorLabel.BorderBrush = new SolidColorBrush(Colors.DarkRed); 
                        colorLabel.BorderThickness = new Thickness(3);
                        isCorrect[i] = false;
                    }
                    
                    Label newLabel = new Label 
                    { 
                        Background = colorLabel.Background, 
                        BorderBrush = colorLabel.BorderBrush, 
                        BorderThickness = colorLabel.BorderThickness, 
                        Width = 94, Height = 10, 
                        HorizontalContentAlignment = HorizontalAlignment.Center, 
                        VerticalContentAlignment = VerticalAlignment.Center,
                        
                    };
                    newStackPanel.Children.Add(newLabel);
                }
                for (int j = 0; j < isCorrect.Length; j++)
                {
                    if (!isCorrect[i])
                    {
                        allCorrect = false;
                        break; 
                    }
                }
                if (allCorrect)
                {
                    MessageBoxResult result = MessageBox.Show($"code is gekraakt in {attempt} pogingen. Wil je nog eens?", "Winner", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes) {
                        ResetGame();
                        GenerateCode();
                    } else { this.Close(); }
                }
                

            }
            historyList.Items.Add(newStackPanel);
        }
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox[] comboBoxes = { comboBox1, comboBox2, comboBox3, comboBox4 };
            Label[] colorLabels = { colorLabel1, colorLabel2, colorLabel3, colorLabel4 };
            for (int i = 0; i < comboBoxes.Length; i++)
            {
                ComboBox comboBox = comboBoxes[i];
                Label colorLabel = colorLabels[i];
                ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    string selectedColor = item.Content.ToString().Trim();
                    switch (selectedColor)
                    {
                        case "wit": 
                            colorLabel.Background = new SolidColorBrush(Colors.White); 
                            break;
                        case "groen": 
                            colorLabel.Background = new SolidColorBrush(Colors.Green); 
                            break;
                        case "blauw": 
                            colorLabel.Background = new SolidColorBrush(Colors.Blue); 
                            break;
                        case "rood": 
                            colorLabel.Background = new SolidColorBrush(Colors.Red); 
                            break;
                        case "geel": 
                            colorLabel.Background = new SolidColorBrush(Colors.Yellow); 
                            break;
                        case "oranje": 
                            colorLabel.Background = new SolidColorBrush(Colors.Orange); 
                            break;
                        default: 
                            colorLabel.Background = null; 
                            break;
                    }
                }
            }
        }
        private void addAttempt()
        {
            if (attempt < 10) 
            { 
                attempt++; 
                guessLabel.Content = $"Poging: " + $"{attempt.ToString()}/10"; 
            }
            else if (attempt > 9) 
            { 
                GameMesage(); 
            }
        }
        private void toggleDebug(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F12) 
            { 
                codeBlock.Visibility = Visibility.Visible; 
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timerTick++;
            timerBlock.Text = timerTick.ToString();
            if (timerTick == 10) 
            { 
                attempt++; 
                StopCountDown(); 
                StartCountDown(); 
            }
        }
        private void StartCountDown() 
        { 
            timerTick = 0; 
            timer.Start(); 
        }
        private void StopCountDown() 
        { 
            timer.Stop(); 
            timerTick = 0; 
            guessLabel.Content = $"Poging: " + $"{attempt.ToString()}/10";
        }
        private void ResetGame()
        {
            attempt = 0;
            timerTick = 0;
            if (attempt == 0) { historyList.Items.Clear(); }
        }
        private void GameMesage()
        {
            if (attempt > 9)
            {
                MessageBoxResult result = MessageBox.Show($"you failed the correct code was {combinedString},\rNog eens proberen?", "FAILED", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) { ResetGame(); }
                else { this.Close(); }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Weet je zeker dat je de applicatie wilt afsluiten?je zit op poging {attempt}/10",
                                                      "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
        private void GenerateCode() 
        {
            Array.Clear(tittleCheck, 0, tittleCheck.Length);
            for (int i = 0; i < 4; i++) { tittleCheck[i] += colors[rnd.Next(0, 6)]; }
            combinedString = string.Join(",", tittleCheck);
            codeBlock.Text = combinedString;
        }
    }
}
