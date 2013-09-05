namespace Qorpent.Dot {
	/// <summary>
	/// Типы форм узлов
	/// </summary>
	public enum NodeShapeType {
		/// <summary>
		/// Прямоугольник
		/// </summary>
		Box,
        /// <summary>
        /// Многоугольник, но нужно доразбираться как задавать ему колличество углов, так как по умолчанию видимо аналогичен Box
        /// </summary>
        Polygon,
        /// <summary>
        /// Эллипс
        /// </summary>
        Ellipse,
        /// <summary>
        /// Овал
        /// </summary>
        Oval,
        /// <summary>
        /// Круг
        /// </summary>
        Circle,
        /// <summary>
        /// Точка 
        /// </summary>
        Point,
        /// <summary>
        /// Яйцо 
        /// </summary>
        Egg,
        /// <summary>
        /// Треугольник
        /// </summary>
        Triangle,
        /// <summary>
        /// Просто текст без рамки
        /// </summary>
        Plaintext,
        /// <summary>
        /// Ромб
        /// </summary>
        Diamond,
        /// <summary>
        /// Трапеция
        /// </summary>
        Trapezium,
        /// <summary>
        /// Параллелограм
        /// </summary>
        Parallelogram,
        /// <summary>
        /// Домик (прямоуг.+треугольн.)
        /// </summary>
        House,
        /// <summary>
        /// Пятиугольник 
        /// </summary>
        Pentagon,
        /// <summary>
        /// Шестиугольник
        /// </summary>
        Hexagon,
        /// <summary>
        /// Семиугольник 
        /// </summary>
        Septagon,
        /// <summary>
        /// Восьмиугольник 
        /// </summary>
        Octagon,
        /// <summary>
        /// Двойной круг 
        /// </summary>
        Doublecircle,
        /// <summary>
        /// Двойной восьмиугольник 
        /// </summary>
        Doubleoctagon,
        /// <summary>
        /// Тройной восьмиугольник 
        /// </summary>
        Tripleoctagon,
        /// <summary>
        /// Перевернутый треугольник 
        /// </summary>
        Invtriangle,
        /// <summary>
        /// Перевернутая трапеция 
        /// </summary>
        Invtrapezium,
        /// <summary>
        /// Перевернутый домик 
        /// </summary>
        Invhouse,
        /// <summary>
        /// Прямоугольник в ромбе 
        /// </summary>
        Mdiamond,
        /// <summary>
        /// Ромб в квадрате 
        /// </summary>
        Msquare,
        /// <summary>
        /// Круг с двумя полосками внутри - сверху и снизу 
        /// </summary>
        Mcircle,
        /// <summary>
        /// Прямоугольник (? так и не поняла чем отличается от Box) 
        /// </summary>
        Rect,
        /// <summary>
        /// Прямоугольник (? так и не поняла чем отличается от Box)  
        /// </summary>
        Rectangle,
        /// <summary>
        /// Квадрат 
        /// </summary>
        Square,
        /// <summary>
        /// Звезда 
        /// </summary>
        Star,
        /// <summary>
        /// Ничего. Простой текст 
        /// </summary>
        None,
        /// <summary>
        /// Заметка - прямоугольник с загнутым уголком справа верху 
        /// </summary>
        Note,
        /// <summary>
        /// Закладка - прямоугольник + маленький прямоугольник слева 
        /// </summary>
        Tab,
        /// <summary>
        /// Папака 
        /// </summary>
        Folder,
        /// <summary>
        /// Кирпич трехмерный 
        /// </summary>
        Box3d,
        /// <summary>
        /// Календарь перекидной 
        /// </summary>
        Component,
        /// <summary>
        /// Промоутер - стрелка в виде буквы Г, стоящая на линии 
        /// </summary>
        Promoter,
        /// <summary>
        /// Указатель вправо
        /// </summary>
        Cds,
        /// <summary>
        /// Терминатор - в виде буквы Т, стоящей на линии 
        /// </summary>
        Terminator,
        /// <summary>
        /// Половина восьмиугольника на линии 
        /// </summary>
        Utr,
        /// <summary>
        /// Полстрелки вправо на линии 
        /// </summary>
        Primersite,
        /// <summary>
        /// Ограничение - ступенька на линии 
        /// </summary>
        Restrictionsite,
        /// <summary>
        /// Навес слева 
        /// </summary>
        Fivepoverhang,
        /// <summary>
        /// Навес справа 
        /// </summary>
        Threepoverhang,
        /// <summary>
        /// Четыре кирпичика 
        /// </summary>
        Noverhang,
        /// <summary>
        /// Два кирпичика 
        /// </summary>
        Assembly,
        /// <summary>
        /// Подпись - как бланк с местом для подписи
        /// </summary>
        Signature,
        /// <summary>
        /// Изолятор - двойной квадратик на линии
        /// </summary>
        Insulator,
        /// <summary>
        /// Крестик на пунктирной ножке, на линии
        /// </summary>
        Ribosite,
        /// <summary>
        /// Кружек на пунктирной ножке, на линии
        /// </summary>
        Rnastab,
        /// <summary>
        /// Крестик на сплошной ножке, на линии
        /// </summary>
        Proteasesite,
        /// <summary>
        /// Кружек на сплошной ножке, на линии
        /// </summary>
        Proteinstab,
        /// <summary>
        /// Стрелка вправо в виде буквы Г
        /// </summary>
        Rpromoter,
        /// <summary>
        /// Стрелка влево в виде буквы Г 
        /// </summary>
        lpromoter,
        /// <summary>
        /// Стрелка вправо
        /// </summary>
        Rarrow,
        /// <summary>
        /// Стрелка влево
        /// </summary>
        Larrow,
        /// <summary>
        /// Область которую можно сделать таблицей. По умолчанию прямоугольник 
        /// </summary>
        Record,
        /// <summary>
        /// Область которую можно сделать таблицей с закругленными углами
        /// </summary>
        Mrecord,




	}
}