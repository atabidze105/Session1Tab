﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session1Tab
{
    internal static class Stats //Статические данные
    {
        public static List<Product> _LboxItems = [ //Список товаров для листбокса. //По умолчанию им не заданы картинки, но их можно добавить в приложении
            new Product("Продукт", 0, 3, "При запуске приложения окно входа – первое, что видит пользователь. На ней пользователю предлагается ввести свой логин и пароль или есть возможность перейти на экран просмотра товаров в роли гостя.", "Производитель", null, 100.1, 25, 6),
            new Product("Образец", 1, 2,"Реализуйте удаление товара. Товар, который присутствует в заказе, удалить нельзя. Товар, у которого есть дополнительные товары, удаляется из базы данных вместе с информацией о дополнительных товарах, если ни один из товаров (даже дополнительный) не заказан", "Производитель", null, 5000, 76, 0),
            new Product("Помидоры",2, 0,"На форме должны быть предусмотрены следующие поля: наименование, категория (выпадающий список), количество на складе, единица измерения, поставщик, стоимость за единицу, изображение и подробное описание (с возможностью многострочного ввода). Стоимость товара может включать сотые части, а также не может быть отрицательной. Минимальное количество также не может принимать отрицательные значения.","Pepsi Co",null,300,654, 3),
            new Product("Полиэтилен",3, 1,"Текст-заполнитель — это текст, который имеет некоторые характеристики реального письменного текста, но является случайным набором слов или сгенерирован иным образом. Его можно использовать для отображения образца шрифтов, создания текста для тестирования или обхода спам-фильтра. ","Samsung",null,323,0, 2),
            new Product("Поридж",4, 4,"Овся́ная ка́ша (или овся́нка) — каша из овсяной крупы (дроблёной или недроблёной, плющеной, овсяных хлопьев или толокна. Это блюдо традиционно было распространено в Шотландии, Скандинавии и на Руси, у восточных славян, которые варили кашу как на воде, так и на молоке. В последние десятилетия набирает популярность и в других странах как сытный и полезный завтрак. Овсяная каша, будучи богата бета-глюканом, медленно отдаёт организму калории и, соответственно, энергию, позволяя дольше чувствовать себя сытым после завтрака.","Microsoft", null,170,2, 0)];
        public static List<Product> _FoundedProducts = []; //Список найденнных товаров
        public static List<User> _Users = [new User("admin", "admin", true, false), new User("user", "user", false, false)]; //Список пользователей
        public static User _UserAutorized = null; //Авторизированный пользователь
        public static Product _SelectedProduct = null; //Выбранный для редактирования товар
        public static List<string> _Cathegories = ["Разное", "Еда", "Не еда", "Для дома", "Для улицы"]; //категории товаров
        public static List<string> _Measurements = ["шт.", "мм", "см", "м", "мл", "л", "г", "кг"]; //единицы измерения

        //Статические поля для сохранения параметров поиска
        public static int _SelectedInex_Price = 0; //Индекс элемента выпадающего списка цены
        public static int _SelectedInex_Supplier = 0; //Индекс элемента выпадающего списка производителей
        public static string _SearchingText = ""; //Поисковая строка
    }
}