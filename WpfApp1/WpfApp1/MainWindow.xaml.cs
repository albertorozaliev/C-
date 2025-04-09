using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private StringBuilder builder = new StringBuilder();

        private bool DisplayResult = false;

        private bool DisplayError = false;
        private string value;
        private double baseValue = 0;
        private bool waitingForPower = false;
        private bool isNewNumber = true;


        public MainWindow()
        {
            InitializeComponent();
            ClearAll();
           
        }

        private void ClearAll()
        {
            builder.Clear();
            DisplayError = false;
            DisplayResult = false;
            ExpressionTextBlock.Text = "";
            ResultTextBlock.Text = "0";
        }

        private void Clear()
        {
            if (DisplayResult)
            {
                ClearAll();
            }
            else
            {
                ResultTextBlock.Text = "0";
            }

        }


        private void AddExpressions()
        {
            if (DisplayResult || DisplayError)
            {
                builder.Clear();
                DisplayError = false;
                DisplayResult = false;
            }
            builder.Append(value);
        }

        private void UpdateToExpressionsDisplay()
        {
            ExpressionTextBlock.Text = builder.ToString();
        }

        private void UpdateToExpressions(string value)
        {
            ResultTextBlock.Text = value;
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обработка x^y отдельно
                if (waitingForPower)
                {
                    double exponent = double.Parse(ResultTextBlock.Text, CultureInfo.InvariantCulture);
                    double powerResult = Math.Pow(baseValue, exponent);
                    ResultTextBlock.Text = powerResult.ToString(CultureInfo.InvariantCulture);
                    ExpressionTextBlock.Text = $"{baseValue} ^ {exponent} =";
                    waitingForPower = false;
                    DisplayResult = true;
                    builder.Clear();
                    return;
                }

                string expression = builder.ToString().Trim();

                if (expression.EndsWith("+") || expression.EndsWith("-") || expression.EndsWith("*") || expression.EndsWith("/"))
                {
                    expression += ResultTextBlock.Text;
                }

                expression = expression.Replace(",", ".");

                var result = new System.Data.DataTable().Compute(expression, null);

                ResultTextBlock.Text = result.ToString();
                ExpressionTextBlock.Text = expression + " =";
                DisplayResult = true;
                builder.Clear();
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = "Ошибка";
                DisplayError = true;
                MessageBox.Show(ex.Message);
            }
        }







        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string op = button.Content.ToString();

            if (DisplayResult)
            {
                builder.Clear();
                builder.Append(ResultTextBlock.Text + " " + op + " ");
                DisplayResult = false;
            }
            else
            {
                string trimmed = builder.ToString().TrimEnd();

                
                if (trimmed.EndsWith("+") || trimmed.EndsWith("-") || trimmed.EndsWith("*") || trimmed.EndsWith("/"))
                {
                    builder.Remove(builder.Length - 3, 3); 
                }

                builder.Append(ResultTextBlock.Text + " " + op + " ");
            }

            UpdateToExpressionsDisplay();
            isNewNumber = true;
        }










        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string digit = button.Content.ToString();

            if (DisplayResult || isNewNumber || ResultTextBlock.Text == "0")
            {
                if (digit == "." && ResultTextBlock.Text.Contains("."))
                    return;

                ResultTextBlock.Text = digit == "." ? "0." : digit;

                DisplayResult = false;
                isNewNumber = false;
            }
            else
            {
                if (digit == "." && ResultTextBlock.Text.Contains("."))
                    return;

                ResultTextBlock.Text += digit;
            }
        }




        private void ParenthesisButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string paren = button.Content.ToString();

            if (DisplayResult || DisplayError)
            {
                ClearAll();
            }

            builder.Append(paren);
            UpdateToExpressionsDisplay();
        }


        private void FunctionButton_Click(Object sender, RoutedEventArgs e)
        {
            var Button = (Button)sender;
            string function = Button.Content.ToString();

            try
            {
                double value = double.Parse(ResultTextBlock.Text, CultureInfo.InvariantCulture);
                double result = 0;

                switch (function)
                {
                    case "sin":
                        result = Math.Sin(value * Math.PI / 180);
                        break;
                    case "PI":
                        result = Math.PI;
                        break;
                    case "e":
                        result = Math.E;
                        break;
                    case "CE":
                        ClearAll();
                        return;
                    case "Exit":
                        Application.Current.Shutdown();
                        return;
                    case "x^2":
                        result = Math.Pow(value, 2);
                        break;
                    case "1/X":
                        if (value != 0)
                        {
                            result = 1 / value;
                        }
                        else
                        {
                            result = double.NaN;
                        }
                        break;
                    case "|x|":
                    case "abs":
                    case "Abs":
                    case "ABS":
                        result = Math.Abs(value);
                        break;
                    case "cos":
                        result = Math.Cos(value * Math.PI / 180);
                        break;
                    case "tg":
                        result = Math.Tan(value * Math.PI / 180);
                        break;
                    case "sqrt^2":
                        if (value >= 0)
                        {
                            result = Math.Sqrt(value);
                        }
                        else
                        {
                            result = double.NaN;
                        }
                        break;
                    case "n!":
                        result = Factorial((int)value);
                        break;
                    case "x^y":

                        baseValue = value;
                        waitingForPower = true;
                        ResultTextBlock.Text = "0";
                        return;
                    case "10^x":
                        result = Math.Pow(10, value);
                        break;
                    case "log":
                        if (value > 0)
                        {
                            result = Math.Log10(value);
                        }
                        else
                        {
                            result = double.NaN;
                        }
                        break;
                    case "ln":
                        if (value > 0)
                        {
                            result = Math.Log(value);
                        }
                        else
                        {
                            result = double.NaN;
                        }
                        break;
                    case "+":
                        result = value + value;
                        break;
                    case "-":
                        result = value - value;
                        break;
                    case "*":
                        result = value * value; 
                        break;
                    case "/":
                        if (value != 0)
                        {
                            result = value / value; 
                        }
                        else
                        {
                            result = double.NaN; 
                        }
                        break;
                    case "+/-":
                        result = -value;
                        break;
                    default:
                        MessageBox.Show("Ошибка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                ResultTextBlock.Text = result.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private double Factorial(int n)
        {
            if (n < 0) return double.NaN; 
            if (n == 0 || n == 1) return 1;

            double result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }

        
    }
}
