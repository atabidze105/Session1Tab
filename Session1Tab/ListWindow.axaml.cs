using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using static Session1Tab.Stats;

namespace Session1Tab;

public partial class ListWindow : Window
{
    private List<string> _Suppliers = []; //список поставщиков
    private string[] _Price = ["По умолчанию", "По возрастанию", "По убыванию"]; //Список элементов для выпадающего списка сортировки по цене

    //<ИНИЦИАЛИЗАЦИЯ>

    public ListWindow()
    {
        InitializeComponent();
        btn_addProduct.IsVisible = _UserAutorized.isAdmin; //Только админ видит кнопку создания товара
        btn_toShoppingCart.IsVisible = !_UserAutorized.isGuest; //Гостю недоступна кнопка перехода в корзину
        LBoxInitialization(_LboxItems); //установление источником листбокса списка товаров

        tblock_user.Text += _UserAutorized.isAdmin == false ? (_UserAutorized.isGuest == true ? "guest" : "user") : "admin"; //Определение сдержимого надписи с информацией о вошедшем пользователе
        CartCountVisibility();//Отображение информации о количестве позиций в корзине

        SupplierUpdate();//Составление списка поставщиков (для выпадающего списка с выборкой поставщиков)
        tbox_search.Text = _SearchingText; //поисковой чтроке задаются статические значения (для сохранения параметров поиска при переходе между окнами)
        ComboBoxInit(); //Установка содержимого выпадающих списков для сортировки
    }

    private void LBoxInitialization(List<Product> listBoxSource) //Метод для обновления листбокса
    {
        LBox.ItemsSource = listBoxSource.Select(x => new //обновление лисбокса, в качестве источника - список, принимаемый методом
        {
            x.pName,
            x.pId,
            x.pDescription,
            x.pSupplier,
            x.pImage,
            x.pPrice,
            x.pQuantity,
            Measurement = " " + _Measurements[x.pMeasurement],
            Admin = _UserAutorized.isAdmin, //Свойство от пользователя, биндится к isVisible у некоторых кнопок
            Guest = _UserAutorized.isGuest, //Св-во пользователя, биндится к isVisible у некоторых кнопок и текстблоков
            Color = x.pQuantity > 0 ? "White" : "Gray" //У товаров с количетсвом 0 будет серый фон
        });
    }

    private void SupplierUpdate() //обновление списка поставщиков
    {
        _Suppliers.Add("Все поставщики");
        foreach (Product product in _LboxItems) //Для каждого товара в списке товаров
        {
            if (!_Suppliers.Contains(product.pSupplier)) //Если с списке поставщиков не содержится такого имени поставщика,
                _Suppliers.Add(product.pSupplier); //То оно добавляется в список
        }
    }

    private void ComboBoxInit() //Установка значений для выпадающих списков для сортривки
    {
        cbox_sortSuppliers.ItemsSource = _Suppliers; //установка источника для выпадающего списка с производителями
        cbox_sortSuppliers.SelectedIndex = _SelectedInex_Supplier;//передача индекса из статического поля (для сохранения параметров поиска при переходе между окнами)
        cbox_sortPrice.ItemsSource = _Price; //установка источника для выпадающего списка с ценой
        cbox_sortPrice.SelectedIndex = _SelectedInex_Price;//аналогично второй строке метода
    }

    private List<CartItem> UsersCartCheck() //Проверка на наличие содержимого вкорзинах пользователей
    {
        foreach (User user in _Users) //Перебор всех пользователей
        {
            if (user.UserCart.Count > 0) //если какая-то корзина не пуста
                return user.UserCart; //возвращается корзина
        }
        return null; //иначе возвращается пустое значение
    }

    //</ИНИЦИАЛИЗАЦИЯ>

    //<ПЕРЕМЕЩЕНИЕ МЕЖДУ ОКНАМИ>

    private void OpenRedWindow() //Переход в окно редактирования/создания товара
    {
        RedWindow redWindow = new();
        redWindow.Show();
        this.Close();
    }

    private void LogOut(object? sender, Avalonia.Interactivity.RoutedEventArgs e) //Разлогинивание пользователя
    {
        _UserAutorized = null; //Очистка статического поля "авторизированный пользователь"
        _SearchingText = ""; //Статическое поле с содержимым поисковыой строки очищается
        _SelectedInex_Price = _SelectedInex_Supplier = 0; //Статические поля с индексами выбранных элементов выпадающего списка сбрасываются
        MainWindow mainWindow = new();
        mainWindow.Show();
        this.Close();
    }

    private void ToCart(object? sender, Avalonia.Interactivity.RoutedEventArgs e) //Переход в корзину
    {
        CartWindow cartWindow = new();
        cartWindow.Show();
        this.Close();
    }

    //</ПЕРЕМЕЩЕНИЕ МЕЖДУ ОКНАМИ>

    private void ProductManipulation(object? sender, Avalonia.Interactivity.RoutedEventArgs e) //Метод для кнопок, связанных с добавлением/редактированием/удалением товаров
    {
        if (UsersCartCheck() == null) //Добавление, редактирование и удаление работают только если корзины пользователей пусты
        {
            var button = (sender as Button)!;
            switch (button.Name)
            {
                case "btn_addProduct": //Добавление
                    {
                        OpenRedWindow();//Переход в окно создания товара
                    }
                    break;
                case "btn_red": //Редактирование
                    {
                        _SelectedProduct = _LboxItems[(int)button!.Tag!];
                        OpenRedWindow();
                    }
                    break;
                case "btn_del": //Удаление
                    {
                        if (_LboxItems[(int)button!.Tag!].pImageSource != null) //Если у удаляемого товара есть картинка, то картинка удаляется вместе с товаром
                        {
                            string imgDel = _LboxItems[(int)button!.Tag!].pImageSource; //Переменной для хранения названия файла с картинкой изображения передается содержимое соответствующего поля удаляемого продукта
                            _LboxItems[(int)button!.Tag!].pImageSource = null; //Удаляемому товару в картинку передается значение null (если этого не сделать то произойдет исключение)
                            File.Delete($"Assets/{imgDel}"); //Непосредственно удаление картинки
                        }
                        _LboxItems.RemoveAt((int)button!.Tag!); //Удаление из списка товаров. Находит по тегу нажатой кнопки, к тэгу биндится ID товара в специальном свойстве (pId)
                        for (int i = 0; i < _LboxItems.Count; i++) //Пересчет ID. После каждого удаления ID (pId) всех товаров меняется
                        {
                            _LboxItems[i].pId = i;
                        }
                        SearchingAndSorting(); //сортировка
                    }
                    break;
            }
        }
    }

    //<ПОИСК>

    private void SearchingActivity(object? sender, Avalonia.Input.KeyEventArgs e) //Поиск товаров по имени
    {
        _SearchingText = tbox_search.Text; //Статическому полю с содержимым поисковыой строки задается введенное значение
        SearchingAndSorting(); //Поиск
    }

    private void SelectionChanging(object? sender, Avalonia.Controls.SelectionChangedEventArgs e) //Смена элемента выпадающего списка
    {
        var combobox = (sender as ComboBox)!;
        switch (combobox.Name)
        {
            case "cbox_sortSuppliers": //Поставщики
                _SelectedInex_Supplier = combobox.SelectedIndex; //Передача в статическое поле выбранного индекса
                break;
            case "cbox_sortPrice": //Цена
                _SelectedInex_Price = combobox.SelectedIndex;//Передача в статическое поле выбранного индекса
                break;
        }
        SearchingAndSorting(); //Поиск
    }

    private void SuppliersSorting() //Выборка по поставщику
    {
        _FoundedProducts.Clear(); //Очистка найденных товаров
        if (cbox_sortSuppliers.SelectedIndex != 0 && cbox_sortSuppliers.SelectedIndex != -1) //Если выбранный индекс не равен 0 и не равен -1
        {
            foreach (Product product in _LboxItems) //перебор товаров из общего списка
            {
                if (product.pSupplier == _Suppliers[cbox_sortSuppliers.SelectedIndex]) //Если поставщик товара совпал с выбранным элементом выпадающего списка
                {
                    _FoundedProducts.Add(product); //добавляется в найденные товары  
                }
            }
        }
        LBoxInitialization(cbox_sortSuppliers.SelectedIndex == 0 ? _LboxItems : _FoundedProducts); //Если выбрвнный элемент в впадающем списке "все поставщики", то источником листбокса становится список всех товаров, иначе найденные 
    }

    private void Searching() //Строчный поиск по всем параметрам
    {
        List<Product> searching = []; //Список для найденныхх товаров
        searching.AddRange(cbox_sortSuppliers.SelectedIndex == 0 ? _LboxItems : _FoundedProducts); //Из-за приравнивания списка одного к другому работа идет некорректно, поэтому в свежий список добавляются элементы из найденных товаров или из всех
        if (tbox_search.Text != "") //Поиск выполняется если поисковая строка содержит символы
        {
            _FoundedProducts.Clear(); //Очистка списка найденных товаров
            string[] keywords = tbox_search.Text.Split(';');//Поисковая строка может искать товары по трем вводимым параметрам: по названию, категории и описанию. Здесь создается массив ключевый символов, по которым выполняется поиск. Раздлитель - ";"
            foreach (Product product in searching) //для каждого товара в списке товаров
            {
                if (product.pName.Trim().ToLower().Contains(keywords[0].Trim().ToLower())) //имя товара и написанное в строке (первый элемент массива) поиска теряют пробелы в начале и в конце, обращаются в лоуеркейс и сравнивается друг с другом. Если сошлось, то:
                {
                    if (keywords.Length == 1) //Если поиск по одному элементу (имя)
                    {
                        _FoundedProducts.Add(product); //добавление товара в список найденных товаров
                    }
                    else if (product.pDescription.Trim().ToLower().Contains(keywords[1].Trim().ToLower())) //Переход на следующий этап. Второй элемент массива - описание. Те же операции, что и в первом условии. Если сошлось, то:
                    {
                        if (keywords.Length == 2) //Если поиск по двум элементам (имя и описание)
                        {
                            _FoundedProducts.Add(product);//добавление товара в список найденных товаров
                        }
                        else if (product.pSupplier.Trim().ToLower().Contains(keywords[2].Trim().ToLower())) //Третий элемент массива - производитель. Если сошлось, то:
                        {
                            if (keywords.Length == 3) //если поиск по трем элементам (имя, описание и производитель)
                            {
                                _FoundedProducts.Add(product); //добавление товара в список найденных товаров
                            }
                            else if (Convert.ToString(product.pPrice).Trim().ToLower().Contains(keywords[3].Trim().ToLower())) //Четвертый элемент массива - цена. Если сошлось, то:
                            {
                                if (keywords.Length == 4) //если поиск по четырем элементам (имя, описание, производитель и цена)
                                {
                                    _FoundedProducts.Add(product); //добавление товара в список найденных товаров
                                }
                                else if (Convert.ToString(product.pQuantity).Trim().ToLower().Contains(keywords[4].Trim().ToLower())) //Пятый элемент массива - количество. Если сошлось, то:
                                {
                                    if (keywords.Length == 5) //если поиск по пяти элементам (имя, описание, производитель, цена и количество)
                                    {
                                        _FoundedProducts.Add(product); //добавление товара в список найденных товаров
                                    }
                                    else if (_Measurements[product.pMeasurement].Trim().ToLower().Contains(keywords[5].Trim().ToLower())) //Шестой элемент массива - единица измерения. Если сошлось, то:
                                    {
                                        if (keywords.Length == 6) //если поиск по шести элементам (имя, описание, производитель, цена, количество и единица измерения)
                                            _FoundedProducts.Add(product);//добавление товара в список найденных товаров
                                    }
                                }
                            }
                        }
                    }
                }
            }
            LBoxInitialization(_FoundedProducts);//обновление лисбокса, в качестве источника список найденных товаров
        }
        else
        {
            LBoxInitialization(searching);//обновление листбокса, в качестве истоника список товаров 
        }
    }

    private void FoundInfo() //вывод количетсва найденных товаров (с соотношением количества всех товаров)
    {
        if (cbox_sortSuppliers.SelectedIndex != 0 || tbox_search.Text != "") //Если выбран не "Все поставщики" или поисковая строка не пуста
        {
            tblock_searchCount.Text = $"{_FoundedProducts.Count} из {_LboxItems.Count}"; //текстблоку задается значение, содржащее количества найденных товаров и всех в целом
            tblock_searchCount.IsVisible = true; //текстблок становится видимым
        }
        else
        {
            tblock_searchCount.IsVisible = false; //текстблок становится невидимым
        }
    }

    private void SearchingAndSorting() //Поиск
    {
        SuppliersSorting(); //выборка по производителям
        Searching(); //выборка по ключевым словам (поисковая строка)
        BubbleSorting(cbox_sortPrice.SelectedIndex); //Сортировка пузырьком (по возрастанию, убыванию цены)
        FoundInfo(); //Вывод количетсва найденных товаров
    }

    private void BubbleSorting(int selectedOption) //Сортировка пузырьком
    {
        List<Product> bubble = []; //Список для сортировки пузырьком
        bubble.AddRange(cbox_sortSuppliers.SelectedIndex != 0 || tbox_search.Text != "" ? _FoundedProducts : _LboxItems); //Добавление в пустой список для сортировки список с товарами (если выбран не "Все поставщики" или поисковая строка не пуста - источник найденные товары, иначе - все товары)
        switch (selectedOption)
        {
            case 1: //По возрастанию
                {
                    for (int i = 0; i < bubble.Count; i++)
                        for (int j = 0; j < bubble.Count - i - 1; j++)
                        {
                            if (bubble[j].pPrice > bubble[j + 1].pPrice)
                            {
                                Product temp = bubble[j];
                                bubble[j] = bubble[j + 1];
                                bubble[j + 1] = temp;
                            }
                        }
                }
                break;
            case 2: //По убыванию
                {
                    for (int i = 0; i < bubble.Count; i++)
                        for (int j = 0; j < bubble.Count - i - 1; j++)
                        {
                            if (bubble[j].pPrice < bubble[j + 1].pPrice)
                            {
                                Product temp = bubble[j];
                                bubble[j] = bubble[j + 1];
                                bubble[j + 1] = temp;
                            }
                        }
                }
                break;
        }
        LBoxInitialization(bubble);
    }

    //</ПОИСК>

    //<КОРЗИНА>

    private void CartCountVisibility() //Метод для отображения количетсва позиций в корзине
    {
        tblock_cartCount.Text = "Позиций в корзине: " + Convert.ToString(_UserAutorized.UserCart.Count); //Передача количества позиций в корзине в текст на окне с товарами
        tblock_cartCount.IsVisible = _UserAutorized.UserCart.Count > 0 ? true : false; //Установка видимости этого текста в зависимости от наличия позиций в корзине
    }

    private void AddToCart(Product product) //Добавление товара в корзину
    {
        for (int i = 0; i < _UserAutorized.UserCart.Count; i++) //Если корзина не пуста, то происходит перебор объектов в ней
        {
            if (_UserAutorized.UserCart[i].prId == product.pId) //Соответствие между товарами из списка товаров и товарами в корзине устанавливается по их общему полю, обозначающему идентификатор товара из спика. Если соответствующий товар есть в корзине,
            {
                _UserAutorized.UserCart[i].cQuantity++; //то его количество увеличивается на 1,
                product.pQuantity--; //а количетсво товара из списка соотвественно уменьшается на 1
                _UserAutorized.UserCart[i].cPrice = _UserAutorized.UserCart[i].cPrice + product.pPrice; //Увеличение цены у товара в корзине (стоимость увеличивается весте с количеством)
                return; //Выход из метода
            }
        }
        _UserAutorized.UserCart.Add(new CartItem(_UserAutorized.UserCart.Count, product.pId, product.pName, product.pPrice, 1)); //Создается новый товар (для корзины) и добавляется в корзину
        product.pQuantity--; //Уменьшение количества у товара в общем списке 
    }

    private void RemoveFromCart(Product product) //Удаление товара из корзины
    {
        for (int i = 0; i < _UserAutorized.UserCart.Count; i++) //Перебор элементов корзины
        {
            if (_UserAutorized.UserCart[i].prId == product.pId) //Если идентификатор соответствует идентифиикатору товара, к которому применен  метод, то
            {
                _UserAutorized.UserCart[i].cQuantity--; //кол-во товара в корзине уменьшается на 1
                _UserAutorized.UserCart[i].cPrice = _UserAutorized.UserCart[i].cPrice - product.pPrice; //Снижение стоимости товара в корзине
                product.pQuantity++; //кол-во товара увеличивается на 1
                if (_UserAutorized.UserCart[i].cQuantity == 0) //Если количество у товара в корзине равно 0,
                {
                    _UserAutorized.UserCart.RemoveAt(i); //то товар удаляется из корзины
                    for (int j = 0; j < _UserAutorized.UserCart.Count; j++) //Пересчет ID. После каждого удаления ID (cId) всех товаров в корзине меняется
                    {
                        _UserAutorized.UserCart[j].cId = j;
                    }
                }
                return; //Выход из метода
            }
        }
    }

    private void CartManipulation(object? sender, Avalonia.Interactivity.RoutedEventArgs e) //Манипуляции с корзиной
    {
        var button = (sender as Button)!;
        switch (button.Name)
        {
            case "btn_cartAdd": //Добавление в корзину
                {
                    if (_LboxItems[(int)button!.Tag!].pQuantity > 0) //Если количетсво добавляемого товара больше 0
                    {
                        AddToCart(_LboxItems[(int)button!.Tag!]); //Метод для добавления товара в корзину
                    }
                }
                break;
            case "btn_cartDel": //Удаление из корзины
                {
                    if (_UserAutorized.UserCart.Count > 0) //если корзина не пуста
                    {
                        RemoveFromCart(_LboxItems[(int)button!.Tag!]); //Метод для удаления уменьшнения количества товара в корзине
                    }
                }
                break;
        }
        SearchingAndSorting(); //Поиск
        CartCountVisibility(); //Отображение количества позиций в корзине (отображается не каждый товар, а количетсво объектов в списке корзины)
    }

    //</КОРЗИНА>
}