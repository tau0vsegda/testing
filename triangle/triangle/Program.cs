if (args.Length != 3)
{
    Console.WriteLine("Неизвестная ошибка");
    return;
}

List<double> sides = new();
try
{
    foreach (string arg in args)
    {
        if (!double.TryParse(arg, out double number) || Double.IsPositiveInfinity(number))
        {
            throw new Exception("Неизвестная ошибка");
        }
        else
        {
            sides.Add(number);
        }
    }
    sides.Sort();
    if (sides[0] < 0) { throw new Exception("Не треугольник"); }
    if (sides[0] == 0) { throw new Exception("Не треугольник"); }
    if (sides[2] - sides[1] >= sides[0]) { throw new Exception("Не треугольник"); }
    if (sides[0] == sides[2]) Console.WriteLine("Равносторонний");
    else if (sides[0] == sides[1] || sides[1] == sides[2]) Console.WriteLine("Равнобедренный");
    else Console.WriteLine("Треугольник");
}
catch(Exception e)
{
    Console.WriteLine(e.Message);
}