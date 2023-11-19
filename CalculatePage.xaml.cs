using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Coursework;

public partial class CalculatePage : Page, INotifyPropertyChanged
{
    private readonly List<Tuple<TextBox, TextBox>> groupTextBoxes = new();
    private int wallButtonClickCount;
    private int textBoxCount = 0;
    private Worker _worker;
    private Siding selectedSiding;
    private readonly double totalResult = 0;
    private double totalLengthSum;
    private double totalSum;
    private double countSiding;
    private double sidingCount;
    private double starterPlanksCount;
    private double finishPlanksCount;

    private Visibility _isVisibleSelected = Visibility.Collapsed;

    public Siding SelectedSiding
    {
        get => selectedSiding;
        set
        {
            selectedSiding = value;
            IsVisibleSelected = Visibility.Visible;
            OnPropertyChanged();
        }
    }

    public Visibility IsVisibleSelected
    {
        get => _isVisibleSelected;
        set
        {
            _isVisibleSelected = value;
            OnPropertyChanged();
        }
    }

    public CalculatePage(Worker worker)
    {
        InitializeComponent();
        _worker = worker;
        UpdateList();
        DataContext = this;
    }

    private void CalculatePage_OnLoaded(object sender, RoutedEventArgs e)
    {
        //  
    }

    private void UpdateList()
    {
        using var context = new MyDbContext();
        var b = new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\picture.png"));
        var project = context.Sidings.Include(p => p.FeaturesMaterials).ToList();

        SelectedSiding = project.First();
        lvDataBinding.ItemsSource = project
            .Select(p => new Siding
            {
                SidingId = p.SidingId,
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                Image = p.Image ?? b
            }).ToList();
    }

    private void AddWallButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (wallButtonClickCount < 6)
        {
            var group = CreateElementGroup();
            elementStackPanel.Children.Add(group);
            wallButtonClickCount++;
        }
        else
        {
            MessageBox.Show("Ограничение на стены");
        }
    }

    private UIElement CreateElementGroup()
    {
        var grid = new Grid();

        grid.Margin = new Thickness(30);
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = new GridLength(150) }); // Fixed width for the second column
        grid.ColumnDefinitions.Add(new ColumnDefinition()); // New column for the first text box

        for (var i = 0; i < 7; i++) grid.RowDefinitions.Add(new RowDefinition());

        // Label 1
        var label1 = new Label { Content = "Длина (м):", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(label1, 0);
        Grid.SetColumn(label1, 1);
        grid.Children.Add(label1);

        // TextBox 1
        var textBox1 = new TextBox();
        Grid.SetRow(textBox1, 1);
        Grid.SetColumn(textBox1, 1);
        grid.Children.Add(textBox1);

        // Label 2
        var label2 = new Label { Content = "Ширина (м):", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(label2, 3);
        Grid.SetColumn(label2, 1);
        grid.Children.Add(label2);

        // TextBox 2
        var textBox2 = new TextBox();
        Grid.SetRow(textBox2, 4);
        Grid.SetColumn(textBox2, 1);
        grid.Children.Add(textBox2);

        // Result 2
        var resultLabel2 = new Label { Content = "Результат:", VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(resultLabel2, 5);
        Grid.SetColumn(resultLabel2, 1);
        grid.Children.Add(resultLabel2);

        // Event handlers for automatic calculation
        textBox1.TextChanged += (sender, e) =>
        {
            TextBox_TextChanged(textBox1);
            CalculateResult(textBox1, textBox2, resultLabel2);
        };
        textBox2.TextChanged += (sender, e) =>
        {
            TextBox_TextChanged(textBox2);
            CalculateResult(textBox1, textBox2, resultLabel2);
        };


        // Button
        var deleteButton = new Button { Content = "Удалить стену" };
        deleteButton.Click += DeleteGroup_Click;
        Grid.SetRow(deleteButton, 6);
        Grid.SetColumn(deleteButton, 1); // Moved to the second column
        grid.Children.Add(deleteButton);

        return grid;
    }

    private void TextBox_TextChanged(TextBox textBox)
    {
        if (textBox.Text.StartsWith("."))
        {
            textBox.Text = "0" + textBox.Text;
            textBox.CaretIndex = textBox.Text.Length;
        }
    }

    private double CalculateTotal()
    {
        totalSum = groupTextBoxes.Sum(tuple =>
        {
            double value1, value2;
            return double.TryParse(tuple.Item1.Text, out value1) && double.TryParse(tuple.Item2.Text, out value2)
                ? value1 * value2
                : 0;
        });

        txtResult.Text = $"Общая сумма: {totalSum:F2} м²";
        return totalSum;
    }

    private double CalculateTotalLength()
    {
        var totalLength = 0.0;

        foreach (var tuple in groupTextBoxes)
        {
            double length;
            if (double.TryParse(tuple.Item1.Text, out length)) totalLength += length;
        }

        return totalLength;
    }

    private void CalculateResult(TextBox textBox1, TextBox textBox2, Label resultLabel)
    {
        double value1, value2;

        if (!string.IsNullOrWhiteSpace(textBox1.Text) && double.TryParse(textBox1.Text, out value1) &&
            !string.IsNullOrWhiteSpace(textBox2.Text) && double.TryParse(textBox2.Text, out value2))
        {
            var result = value1 * value2;
            resultLabel.Content = $"Результат: {result:F2} м²";
            UpdateResult();

            var tupleToUpdate =
                groupTextBoxes.FirstOrDefault(tuple => tuple.Item1 == textBox1 && tuple.Item2 == textBox2);
            if (tupleToUpdate != null)
            {
                tupleToUpdate.Item1.Text = textBox1.Text;
                tupleToUpdate.Item2.Text = textBox2.Text;
            }
            else
            {
                groupTextBoxes.Add(new Tuple<TextBox, TextBox>(textBox1, textBox2));
            }

            CalculateTotal();
        }
        else
        {
            resultLabel.Content = "Неверный ввод. Введите числовые значения.";
        }
    }

    private void DeleteGroup_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element)
        {
            var group = element.Parent as UIElement;

            if (elementStackPanel.Children.Contains(group))
            {
                elementStackPanel.Children.Remove(group);
                wallButtonClickCount--;

                var tupleToRemove = groupTextBoxes.FirstOrDefault(tuple => tuple.Item1.Parent == group);
                if (tupleToRemove != null) groupTextBoxes.Remove(tupleToRemove);

                CalculateTotal();
            }
        }
    }

    private void UpdateResult()
    {
        txtResult.Text = $"{totalResult:F2} s";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void MakeCalculation_OnClick(object sender, RoutedEventArgs e)
    {
        GetMakeCaculation();
        
        DisplaySidingInfo(getExectResult);
        GetSlats(getExectResult);
        GetSlatsFinish(getExectResult);
        GetFilm(getExectResult);
        ShowCalculationResults();
    }

    private async Task GetMakeCaculation()
    {
        try
        {
            totalLengthSum = CalculateTotalLength();
            var context = new MyDbContext();
            var result = context.FeaturesMaterials
                .Where(fm => fm.SidingId == selectedSiding.SidingId)
                .Include(fm => fm.Features)
                .Select(fm => new
                {
                    FeaturesMaterials = fm.Value,
                    Features = fm.Features.Title
                }).ToList();
            var serializedResult = JsonConvert.SerializeObject(result);

            var featuresMaterialsValue = JsonConvert.DeserializeObject<List<dynamic>>(serializedResult)
                .Select(item => item.FeaturesMaterials)
                .ToList();

            decimal length = Math.Round(Convert.ToDecimal(featuresMaterialsValue[0]), 3);
            decimal width = Math.Round(Convert.ToDecimal(featuresMaterialsValue[1]), 3);

            if (length != 0 && width != 0)
            {
                var squareSiding = length * width;
                var totalWallArea = CalculateTotal();

                if (squareSiding != 0)
                {
                    countSiding = Convert.ToDouble(totalWallArea) / Convert.ToDouble(squareSiding);
                    MessageBox.Show(countSiding.ToString());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }

    private void DisplaySidingInfo(StackPanel dockPanel)
    {
        dockPanel.HorizontalAlignment = HorizontalAlignment.Center;
        if (SelectedSiding != null)
        {
            var sidingImage = new Image
            {
                Source = SelectedSiding.Image,
                Width = 300,
                Height = 300
            };
            var sidingNameTextBlock = new TextBlock
            {
                Text = SelectedSiding.Title,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Left,
                FontSize = 26,
                FontFamily = new FontFamily("Arial"),
                Margin = new Thickness(5)
            };
            var sidingPriceTextBlock = new TextBlock
            {
                Text = $"Цена за штуку: {SelectedSiding.Price:C}",
                TextAlignment = TextAlignment.Left,
                FontSize = 20,
                FontFamily = new FontFamily("Arial"),
                Margin = new Thickness(5)
            };

            double totalSidingPrice = Convert.ToDouble(SelectedSiding.Price) * countSiding;

            var countSlats = new TextBlock()
            {
                Text = $"Необходимо: {Math.Ceiling(countSiding)} шт",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var totalSidingPriceTextBlock = new TextBlock
            {
                Text = $"Итоговая цена: {totalSidingPrice:C}",
                TextAlignment = TextAlignment.Left,
                FontSize = 20,
                FontFamily = new FontFamily("Arial"),
                Margin = new Thickness(5)
            };

            var imageStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            imageStackPanel.Children.Add(sidingImage);

            var infoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(40)
            };
            infoStackPanel.Children.Add(sidingNameTextBlock);
            infoStackPanel.Children.Add(sidingPriceTextBlock);
            infoStackPanel.Children.Add(countSlats);
            infoStackPanel.Children.Add(totalSidingPriceTextBlock);

            var allStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            allStackPanel.Children.Add(imageStackPanel);
            allStackPanel.Children.Add(infoStackPanel);

            dockPanel.Children.Add(allStackPanel);
        }
    }

    private double slatsCount;
    private void GetSlats(StackPanel dockPanel)
    {
        if (SelectedSiding != null)
        {
            var imageSlats = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/planka.png")),
                Width = 300,
                Height = 300
            };
            var sidingSlatsTextBlock = new TextBlock
            {
                Text = "Планка стартовая универсальная",
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold,
                FontSize = 26,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5)
            };
            var sidingSlatsPriceBlock = new TextBlock
            {
                Text = $"Цена за штуку: {420:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left, Margin = new Thickness(5)
            };
            
            slatsCount = totalLengthSum;
            var slatsPrice = slatsCount * 420;

            var countSlats = new TextBlock()
            {
                Text = $"Необходимо: {slatsCount} шт",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var totalSlatsPriceTextBlock = new TextBlock
            {
                Text = $"Итого: {slatsPrice:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var imageSlatsStackPanel = new StackPanel();
            imageSlatsStackPanel.Children.Add(imageSlats);

            var infoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(40)
            };

            infoStackPanel.Children.Add(sidingSlatsTextBlock);
            infoStackPanel.Children.Add(sidingSlatsPriceBlock);
            infoStackPanel.Children.Add(countSlats);
            infoStackPanel.Children.Add(totalSlatsPriceTextBlock);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stackPanel.Children.Add(imageSlatsStackPanel);
            stackPanel.Children.Add(infoStackPanel);

            dockPanel.Children.Add(stackPanel);
        }
    }

    //финишная планка
    private void GetSlatsFinish(StackPanel dockPanel)
    {
        if (SelectedSiding != null)
        {
            var imageSlats = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/finish.png")),
                Width = 300,
                Height = 300
            };
            var sidingSlatsTextBlock = new TextBlock
            {
                Text = "Планка финишная универсальная",
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold,
                FontSize = 26,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5)
            };
            var sidingSlatsPriceBlock = new TextBlock
            {
                Text = $"Цена за штуку: {420:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left, Margin = new Thickness(5)
            };

            var slatsCount = totalLengthSum;
            var slatsPrice = slatsCount * 420;

            var countSlats = new TextBlock()
            {
                Text = $"необходимо: {slatsCount} шт",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };


            var totalSlatsPriceTextBlock = new TextBlock
            {
                Text = $"Итого: {slatsPrice:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var imageSlatsStackPanel = new StackPanel();
            imageSlatsStackPanel.Children.Add(imageSlats);

            var infoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(40)
            };

            infoStackPanel.Children.Add(sidingSlatsTextBlock);
            infoStackPanel.Children.Add(sidingSlatsPriceBlock);
            infoStackPanel.Children.Add(countSlats);
            infoStackPanel.Children.Add(totalSlatsPriceTextBlock);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stackPanel.Children.Add(imageSlatsStackPanel);
            stackPanel.Children.Add(infoStackPanel);

            dockPanel.Children.Add(stackPanel);
        }
    }

    //ветрозащитная плёнка
    private void GetFilm(StackPanel dockPanel)
    {
        if (SelectedSiding != null)
        {
            var imageSlats = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/film.png")),
                Width = 300,
                Height = 300
            };
            var sidingSlatsTextBlock = new TextBlock
            {
                Text = "Пленка ветро-влагозащитная Grand Line Facade (75м2)",
                FontFamily = new FontFamily("Arial"),
                FontWeight = FontWeights.Bold,
                FontSize = 26,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(5)
            };
            var sidingSlatsPriceBlock = new TextBlock
            {
                Text = $"Цена за штуку: {5200:C}",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left, Margin = new Thickness(5)
            };

            var slatsCount = totalLengthSum;
            var slatsPrice = slatsCount * 420;
            int quantity = (int)Math.Ceiling(totalSum / 75.0);

            var totalSlatsPriceTextBlock = new TextBlock
            {
                Text = $"Необходимо: {quantity} шт",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            int sum = quantity * 5200;
            var totalSumPriceTextBlock = new TextBlock
            {
                Text = $"Итого: {sum:C} ",
                FontFamily = new FontFamily("Arial"),
                FontSize = 20,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5)
            };

            var imageSlatsStackPanel = new StackPanel();
            imageSlatsStackPanel.Children.Add(imageSlats);

            var infoStackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(40)
            };

            infoStackPanel.Children.Add(sidingSlatsTextBlock);
            infoStackPanel.Children.Add(sidingSlatsPriceBlock);
            infoStackPanel.Children.Add(totalSlatsPriceTextBlock);
            infoStackPanel.Children.Add(totalSumPriceTextBlock);

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stackPanel.Children.Add(imageSlatsStackPanel);
            stackPanel.Children.Add(infoStackPanel);

            dockPanel.Children.Add(stackPanel);
        }
    }

    
    private void ShowCalculationResults()
    {
        exectResult.Children.Clear();

        var sectionLabel = new Label
        {
            Content = "Счёт/заказ",
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 16 // Размер шрифта
        };
        exectResult.Children.Add(sectionLabel);

        foreach (var tuple in groupTextBoxes)
        {
            double value1, value2;
            if (double.TryParse(tuple.Item1.Text, out value1) && double.TryParse(tuple.Item2.Text, out value2))
            {
                var result = value1 * value2;

                var wallAreaLabel = new Label
                {
                    Content = $"Площадь стены ({value1} м x {value2} м): {result:F2} м²",
                    Margin = new Thickness(0, 5, 5, 0),
                    FontSize = 14 // Размер шрифта
                };
                exectResult.Children.Add(wallAreaLabel);
            }
        }

        var additionalInfoLabel = new Label
        {
            Content = "Дополнительная информация",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 16 // Размер шрифта
        };
        exectResult.Children.Add(additionalInfoLabel);

        var totalWallLength = groupTextBoxes.Sum(tuple =>
        {
            double value;
            return double.TryParse(tuple.Item1.Text, out value) ? value : 0;
        });

        double sidingLength = 3;
        sidingCount = totalWallLength / sidingLength;

        starterPlanksCount = totalWallLength / 3;
        finishPlanksCount = starterPlanksCount;
        var nProfilesCount = sidingCount;

        var sidingCountLabel = new Label
        {
            Content = $"Количество сайдинга: {Math.Ceiling(countSiding)} шт.",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 14 // Размер шрифта
        };
        exectResult.Children.Add(sidingCountLabel);

        var starterPlanksLabel = new Label
        {
            Content = $"Количество стартовых планок: {slatsCount} шт.",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 14 // Размер шрифта
        };
        exectResult.Children.Add(starterPlanksLabel);

        var finishPlanksLabel = new Label
        {
            Content = $"Количество финишных планок: {slatsCount} шт.",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 14 // Размер шрифта
        };
        exectResult.Children.Add(finishPlanksLabel);

        var nProfilesLabel = new Label
        {
            Content = $"Количество Н-профилей: {nProfilesCount:F2} шт.",
            Margin = new Thickness(0, 5, 5, 0),
            FontSize = 14 // Размер шрифта
        };
        exectResult.Children.Add(nProfilesLabel);
    }
}