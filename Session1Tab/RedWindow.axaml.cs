using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using static Session1Tab.Stats;

namespace Session1Tab;

public partial class RedWindow : Window
{
    private string _SelectedProductImageSource = _SelectedProduct != null ? _SelectedProduct.pImageSource : null; //Картинка попадаемого в это окно объекта сохраняется до выхода (чтобы восстановить ее если не сохранены данные при редактировании)
    private string? _SelectedImage = null; //выбранное изображение
    
    public RedWindow()
    {
        InitializeComponent();
        tblock_panelHeader.Text = _SelectedProduct == null ? "Создать товар" : "Редактирование товара"; //Содержание заголовка и кнопки сохранения изменений меняются в зависимости от содержания поля для выбранного продукта
        btn_confirm.Content = _SelectedProduct == null ? "Добавить" : "Сохранить";
        cbox_cathegories.ItemsSource = _Cathegories; //выпадающему списку задается источник (статический список категорий)
        cbox_cathegories.SelectedIndex = 0; //Выбранный элемент списка категорий - "Разное"
        cbox_measurements.ItemsSource = _Measurements; //выпадающему списку задается источник (статический список единиц измерения)
        cbox_measurements.SelectedIndex = 0; //Выбранный элемент списка единиц измерения - "шт."
        ProductCheck(); //проверка на наличие выбранного товара
    }

    private void ProductCheck() //Проверка наличия выбранного товара
    {
        if (_SelectedProduct != null) //Если товар редактируется, то поля с редактируемыми данными заполняются автоматически
        {
            tblock_id.Text = $"ID: {_SelectedProduct.pId}";
            tbox_pName.Text = _SelectedProduct.pName;
            tbox_pSupplier.Text = _SelectedProduct.pSupplier;
            tbox_pPrice.Text = Convert.ToString(_SelectedProduct.pPrice);
            tbox_pQuantity.Text = Convert.ToString(_SelectedProduct.pQuantity);
            tbox_pDescription.Text = _SelectedProduct.pDescription;
            cbox_cathegories.SelectedIndex = _SelectedProduct.pCathegory;
            cbox_measurements.SelectedIndex = _SelectedProduct.pMeasurement;
            if (_SelectedProduct.pImageSource != null) //Если у товара есть изображение, то отображается его превью
            {
                tblock_preview.IsVisible = img_preview.IsVisible = true; //Изображение и название картинки становятся видимыми
                tblock_preview.Text = _SelectedImage = _SelectedProduct.pImageSource; //тексту превью и полю для выбранного изображения передается название файла изображения
                img_preview.Source = new Bitmap($"Assets/{_SelectedProduct.pImageSource}"); //картинке превью передается битмап изображение с названием файла изображения из ассетов в качестве источника
            }
        }
    }

    private void newListWindow() //переход в окно со списком товаров
    {
        _SelectedProduct = null; //Очистка полей редактируемого товара и выбранного изображения
        ListWindow listWindow = new ListWindow();
        listWindow.Show();
        this.Close();
    }

    private void RedActivity(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try //Если в поля для int/double значений введены иные символы, то произойдет исключение. В таком случае окно создания/редактирования закроется, откроется окно списка товаров и изменения не сохранятся
        {
            var button = (sender as Button)!;
            switch (button.Name)
            {
                case "btn_cancel": //Отмена. Возвращение обратно в окно с товарами //Если во время редактирования товара, добавив новое изображение, прервать редактирование, не сохранив изменения, то новая картинка будет удалена из ассетов, при этом сохранится изначальное изображение товара
                    if (_SelectedImage != null && _SelectedImage != _SelectedProductImageSource) //Если поле для выбранного изображения не равно null и выбранное изображение не совпадает со значением из поля, хранящее изначальноне изображение товара
                        File.Delete($"Assets/{_SelectedImage}"); //Файл изображения удаляется из папки ассетов
                    newListWindow();
                    break;
                case "btn_confirm": //Подтверждение
                    if (_SelectedProduct != null) //Если поле редактируемого товара не пустое (т.е. выбранный товар редактируется)
                    {
                        for (int i = 0; i < _LboxItems.Count; i++)//поиск редактируемого товара в списке товаров
                        {
                            if (_LboxItems[i].pId == _SelectedProduct.pId) //если найден:
                            {
                                if (_SelectedImage != _SelectedProductImageSource && _SelectedProductImageSource != null) //Если было установлено новое изображение, 
                                    File.Delete($"Assets/{_SelectedProductImageSource}");

                                _LboxItems[i] = new Product(tbox_pName.Text, _SelectedProduct.pId, cbox_cathegories.SelectedIndex, tbox_pDescription.Text, tbox_pSupplier.Text, _SelectedImage, Convert.ToDouble(tbox_pPrice.Text), Convert.ToInt32(tbox_pQuantity.Text), cbox_measurements.SelectedIndex); //замена товара в списке товаров на новый с обновленными данными
                                newListWindow();
                                break;
                            }
                        }
                    }
                    else //Если поле редактируемого товара пустое (т.е. создается новый товар)
                    {
                        _LboxItems.Add(new Product(tbox_pName.Text, _LboxItems.Count, cbox_cathegories.SelectedIndex, tbox_pDescription.Text, tbox_pSupplier.Text, _SelectedImage, Convert.ToDouble(tbox_pPrice.Text), Convert.ToInt32(tbox_pQuantity.Text), cbox_measurements.SelectedIndex)); //Добавляется новый товар в конец списка товаров
                        newListWindow();
                    }
                    break;
            }
        }
        catch
        {
            if (_SelectedImage != null && _SelectedImage != _SelectedProductImageSource) //если до исключения было выбрано новоне изображение, оно удалится из ассетов. Если у товра уже было изображение, оно останется на месте
                File.Delete($"Assets/{_SelectedImage}");
            newListWindow();
        }
    }

    private readonly FileDialogFilter fileFilter = new() //Фильтр для проводника
    {
        Extensions = new List<string>() { "png", "jpg", "jpeg" }, //доступные расширения, отображаемые в проводнике
        Name = "Файлы изображений" //пояснение
    };

    private string SameName(string filename) //Проверка уникальности имени файла
    {
        string[] withExtentions = Directory.GetFiles("Assets"); //Получение названий всех изображений из ассетов с расширениями
        List<string> withoutExtentions = []; //инициализация нового списка для названий файлов без расширений

        foreach (string file in withExtentions)
            withoutExtentions.Add(Path.GetFileNameWithoutExtension(file)); //В новый список передаются названия файлов без расширений

        foreach (string file1 in withoutExtentions) //перебор каждого названия файла из списка названий
            if (file1 == filename) //если название одного из файлов идентично названию файла заданного в методе
            {
                return filename; //возвращает название файла
            }
        return null; //если такой файл не был найден, возвращает null
    }

    private async void ImageSelection(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var button = (sender as Button)!;
        switch (button.Name){
            case "btn_imgAdd": //Добавление картинки
                OpenFileDialog dialog = new(); //Открытие проводника
                dialog.Filters.Add(fileFilter); //Применение фильтра
                string[] result = await dialog.ShowAsync(this); //Выбор файла
                if (result == null || result.Length == 0)
                    return;//Если закрыть проводник то картинка не будет выбрана

                string imageName = Path.GetFileName(result[0]); //получение имени файла
                string[] extention = imageName.Split('.'); //Название файла делится на название и расширение
                string temp = extention[0]; //В изменяемой переменной хранится название файла. Оно будет меняться в процессе
                int i = 1; //Счетчик
                while(SameName(temp) != null) //Пока метод для проверки уникальности файла возвращает название файла
                {
                    temp = extention[0] + $"{i}"; //Новое имя файла
                    i++;
                }
                imageName = temp + '.' + extention[1]; //Новое имя файла с расширением

                File.Copy(result[0], $"Assets/{imageName}", true); //Копирование файла в папку ассетов

                if (_SelectedImage != null && _SelectedImage != _SelectedProductImageSource) //Если до установки новой картинки была выбрана другая, и при этом выбранная картинка не значение из поля, хранящее изначальноне изображение товара
                    File.Delete($"Assets/{_SelectedImage}"); //Удаление предыдущего изображения из ассетов

                tblock_preview.IsVisible = img_preview.IsVisible = true; //Установление видимости превью (картинки и текстблока с ее названием)
                tblock_preview.Text = _SelectedImage = imageName; //Текстблоку и полю выбранной картинки задается имя файла
                img_preview.Source = new Bitmap($"Assets/{imageName}"); //Установка источника превью-картинки 

                break;
            case "btn_imgDel": //Удаление картинки
                tblock_preview.IsVisible = img_preview.IsVisible = false; //Превью становится невидимым
                _SelectedImage = null;//очистка поля с выбранным изображением

                if (tblock_preview.Text != _SelectedProductImageSource) //Удаление произойдет только если удаляемое изображение не является значением из поля, хранящее изначальноне изображение товара
                    File.Delete($"Assets/{tblock_preview.Text}"); //Удаление файла по названию из превью-текстблока

                break;
        }
    }
}