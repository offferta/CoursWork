using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Coursework.Context;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace Coursework;

public partial class CalculatePage : Page, INotifyPropertyChanged
{
    private List<Tuple<TextBox, TextBox>> groupTextBoxes = new List<Tuple<TextBox, TextBox>>();
    private int wallButtonClickCount = 0;
    private int textBoxCount = 0;
    private Worker _worker;
    private Siding selectedSiding;
    private double totalResult = 0;
    private double totalLengthSum = 0;

    private Visibility _isVisibleSelected = Visibility.Collapsed;

    public Siding SelectedSiding
    {
        get { return selectedSiding; }
        set
        {
            selectedSiding = value;
            IsVisibleSelected = Visibility.Visible;
            OnPropertyChanged();
        }
    }
    public Visibility IsVisibleSelected
    {
        get { return _isVisibleSelected; }
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
            .Select(p => new Siding()
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
            UIElement group = CreateElementGroup();
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
            Grid grid = new Grid();
        
            grid.Margin = new Thickness(30);
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition()
                { Width = new GridLength(150) }); // Fixed width for the second column
            grid.ColumnDefinitions.Add(new ColumnDefinition()); // New column for the first text box

            for (int i = 0; i < 7; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
            }

            // Label 1
            Label label1 = new Label { Content = "Длина (м):", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(label1, 0);
            Grid.SetColumn(label1, 1);
            grid.Children.Add(label1);

            // TextBox 1
            TextBox textBox1 = new TextBox();
            Grid.SetRow(textBox1, 1);
            Grid.SetColumn(textBox1, 1);
            grid.Children.Add(textBox1);

            // Label 2
            Label label2 = new Label { Content = "Ширина (м):", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(label2, 3);
            Grid.SetColumn(label2, 1);
            grid.Children.Add(label2);

            // TextBox 2
            TextBox textBox2 = new TextBox();
            Grid.SetRow(textBox2, 4);
            Grid.SetColumn(textBox2, 1);
            grid.Children.Add(textBox2);

            // Result 2
            Label resultLabel2 = new Label { Content = "Результат:", VerticalAlignment = VerticalAlignment.Center };
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
            Button deleteButton = new Button { Content = "Удалить стену" };
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
       double totalSum = groupTextBoxes.Sum(tuple => 
        {
            double value1, value2;
            return double.TryParse(tuple.Item1.Text, out value1) && double.TryParse(tuple.Item2.Text, out value2) ? value1 * value2 : 0;
        });

        txtResult.Text = $"Общая сумма: {totalSum:F2} м²";
        return totalSum;
    }
    
    private double CalculateTotalLength()
    {
        double totalLength = 0.0;

        foreach (var tuple in groupTextBoxes)
        {
            double length;
            if (double.TryParse(tuple.Item1.Text, out length))
            {
                totalLength += length;
            }
        }
        
        return totalLength;
    }

    private void CalculateResult(TextBox textBox1, TextBox textBox2, Label resultLabel)
    {
        double value1, value2;

        if (!string.IsNullOrWhiteSpace(textBox1.Text) && double.TryParse(textBox1.Text, out value1) &&
            !string.IsNullOrWhiteSpace(textBox2.Text) && double.TryParse(textBox2.Text, out value2))
        {
            double result = value1 * value2;
            resultLabel.Content = $"Результат: {result:F2} м²";
            UpdateResult();
            
            var tupleToUpdate = groupTextBoxes.FirstOrDefault(tuple => tuple.Item1 == textBox1 && tuple.Item2 == textBox2);
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
            UIElement group = element.Parent as UIElement;

            if (elementStackPanel.Children.Contains(group))
            {
                elementStackPanel.Children.Remove(group);
                wallButtonClickCount--;

                var tupleToRemove = groupTextBoxes.FirstOrDefault(tuple => tuple.Item1.Parent == group);
                if (tupleToRemove != null)
                {
                    groupTextBoxes.Remove(tupleToRemove);
                }

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
    private  void MakeCalculation_OnClick(object sender, RoutedEventArgs e)
    {
         GetMakeCaculation();
         ShowCalculationResults();
         DisplaySidingInfo(getExectResult);
         GetSlats(getExectResult);
    }
    
    private async Task GetMakeCaculation()
    {
        try
        {
            totalLengthSum = CalculateTotalLength();
            MyDbContext context = new MyDbContext();
            var result =  context.FeaturesMaterials
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
                decimal squareSiding = length * width;
                double totalWallArea = CalculateTotal();

                if (squareSiding != 0)
                {
                    double countSiding = Convert.ToDouble(totalWallArea) / Convert.ToDouble(squareSiding);
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
            // Создаем изображение сайдинга
            Image sidingImage = new Image
            {
                Source = SelectedSiding.Image, // Используем изображение из SelectedSiding
                Width = 200, // Установите размер изображения по вашему усмотрению
                Height = 200 // Установите размер изображения по вашему усмотрению
            };
            sidingImage.HorizontalAlignment = HorizontalAlignment.Left;
            TextBlock sidingNameTextBlock = new TextBlock { Text = SelectedSiding.Title, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center};
            TextBlock sidingPriceTextBlock = new TextBlock { Text = $"Цена за штуку: {SelectedSiding.Price:C}", TextAlignment = TextAlignment.Center };
        
            // Расчет количества сайдинга
            double sidingCount = 4; // Реализуйте ваш метод вычисления количества сайдинга

            // Расчет общей цены
            double totalSidingPrice = 2 * 2;

            // Создаем текстовой блок для отображения общей цены
            TextBlock totalSidingPriceTextBlock = new TextBlock { Text = $"Итоговая цена: {totalSidingPrice:C}", TextAlignment = TextAlignment.Center};

            sidingImage.HorizontalAlignment = HorizontalAlignment.Left;
            sidingImage.VerticalAlignment = VerticalAlignment.Center;
            
            // Добавляем изображение и информацию о сайдинге в StackPanel
            /*dockPanel.Children.Add(sidingImage);*/
            StackPanel imageStackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            imageStackPanel.Children.Add(sidingImage);

            // Создаем StackPanel для текстовых блоков справа от изображения
            StackPanel infoStackPanel = new StackPanel { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Center};
            infoStackPanel.Children.Add(sidingNameTextBlock);
            infoStackPanel.Children.Add(sidingPriceTextBlock);
            infoStackPanel.Children.Add(totalSidingPriceTextBlock);

            StackPanel allStackPanel = new StackPanel { Orientation = Orientation.Horizontal , HorizontalAlignment = HorizontalAlignment.Left};
            allStackPanel.Children.Add(imageStackPanel);
            allStackPanel.Children.Add(infoStackPanel);
            // Добавляем созданный StackPanel с информацией о сайдинге справа от изображения
            dockPanel.Children.Add(allStackPanel);
        }
    }

    private void GetSlats(StackPanel dockPanel)
    {
        if (SelectedSiding != null)
        {
            Image imageSlats = new Image()
            {
                Source = new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\picture.png")),
                Width = 200,
                Height = 200
            };
            TextBlock sidingSlatsTextBlock = new TextBlock { Text = "Планка стартовая универсальная" };
            TextBlock sidingSlatsPriceBlock = new TextBlock { Text = $"Цена за штуку: {420:C}" };

            double slatsCount = totalLengthSum;
            double slatsPrice = slatsCount * 420;

            TextBlock totalSlatsPriceTextBlock = new TextBlock { Text = $"Итого: {slatsPrice:C}" };

            dockPanel.Children.Add(imageSlats);

            StackPanel infoStackPanel = new StackPanel { Orientation = Orientation.Vertical };
            infoStackPanel.Children.Add(sidingSlatsTextBlock);
            infoStackPanel.Children.Add(sidingSlatsPriceBlock); 

            dockPanel.Children.Add(infoStackPanel);
            dockPanel.Children.Add(totalSlatsPriceTextBlock);
        }
    }

    private void ShowCalculationResults()
    {
        exectResult.Children.Clear();

        Label sectionLabel = new Label { Content = "Счёт/заказ", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 5, 0) };
        exectResult.Children.Add(sectionLabel);

        // Создание и добавление элементов с результатами площадей стен
        foreach (var tuple in groupTextBoxes)
        {
            double value1, value2;
            if (double.TryParse(tuple.Item1.Text, out value1) && double.TryParse(tuple.Item2.Text, out value2))
            {
                double result = value1 * value2;

                // Создание лейбла для отображения площади стены и её размеров
                Label wallAreaLabel = new Label
                {
                    Content = $"Площадь стены ({value1} м x {value2} м): {result:F2} м²",
                    Margin = new Thickness(0, 5, 5, 0)
                };
                exectResult.Children.Add(wallAreaLabel);
            }
        }

        // Добавление дополнительной информации при необходимости
        // Пример добавления нового элемента с информацией
        Label additionalInfoLabel = new Label { Content = "Дополнительная информация", Margin = new Thickness(0, 5, 5, 0) };
        exectResult.Children.Add(additionalInfoLabel);
        
        // Вычисление общей длины стен из введенных пользователем данных
        double totalWallLength = groupTextBoxes.Sum(tuple =>
        {
            double value;
            return double.TryParse(tuple.Item1.Text, out value) ? value : 0;
        });

        // Вычисление количества сайдинга
        double sidingLength = 3; // Длина одной планки сайдинга (в метрах)
        double sidingCount = totalLengthSum / sidingLength;

        // Вычисление количества стартовых планок (предположим, что одна планка - 3 м)
        double starterPlanksCount = totalWallLength / 3;

        // Вычисление количества финишных планок (примерное кол-во, можно заменить на реальные данные)
        int finishPlanksCount = 10; // Замените это значение на реальное количество финишных планок

        // Вычисление количества Н-профилей (предположим, что 1 профиль на каждую планку)
        double nProfilesCount = sidingCount;

        // Добавление информации о количестве сайдинга, планок и Н-профилей в exectResult
        Label sidingCountLabel = new Label { Content = $"Количество сайдинга: {sidingCount:F2} шт.", Margin = new Thickness(0, 5, 5, 0) };
        exectResult.Children.Add(sidingCountLabel);

        Label starterPlanksLabel = new Label { Content = $"Количество стартовых планок: {starterPlanksCount:F2} шт.", Margin = new Thickness(0, 5, 5, 0) };
        exectResult.Children.Add(starterPlanksLabel);

        Label finishPlanksLabel = new Label { Content = $"Количество финишных планок: {finishPlanksCount} шт.", Margin = new Thickness(0, 5, 5, 0) };
        exectResult.Children.Add(finishPlanksLabel);

        Label nProfilesLabel = new Label { Content = $"Количество Н-профилей: {nProfilesCount:F2} шт.", Margin = new Thickness(0, 5, 5, 0) };
        exectResult.Children.Add(nProfilesLabel);
    
    }
}