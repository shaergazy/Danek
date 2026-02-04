using System;

namespace Danek.BLL.Extesions;

public static class IntExtension
{
    public static string ToExcelColumn(this int x)
    {
        ++x;
        var name = string.Empty;
        while (x > 0)
        {
            var modulo = (x - 1) % 26;
            name = Convert.ToChar('A' + modulo) + name;
            x = (x - modulo) / 26;
        }

        return name;
    }
}
