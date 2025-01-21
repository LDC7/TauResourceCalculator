using System;
using System.Collections.Immutable;
using System.Linq;

namespace TauResourceCalculator.Domain.ResourceCalculator.Utils;

public static class DateUtils
{
  /// <summary>
  /// Найти следующее по определённому счёту воскресенье.
  /// </summary>
  /// <param name="date">Дата с которой начинаем поиск.</param>
  /// <param name="index">Какое по счёту воскресенье требуется (0 - на этой неделе).</param>
  /// <returns>Дата будущего воскресенья.</returns>
  /// <remarks>Неделя начинается в понедельник.</remarks>
  public static DateOnly FindNextSundayByIndex(DateOnly date, int index)
  {
    var dayOfWeekIndex = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
    return date.AddDays(7 - dayOfWeekIndex).AddDays(index * 7);
  }

  /// <summary>
  /// Найти неделю по индексу начиная с определённой даты.
  /// </summary>
  /// <param name="date">Дата начала поиска.</param>
  /// <param name="index">Индекс недели.</param>
  /// <returns>Даты искомой недели.</returns>
  /// <remarks>Неделя начинается в понедельник.</remarks>
  public static ImmutableArray<DateOnly> FindNextWeekByIndex(DateOnly date, int index)
  {
    var dayOfWeekIndex = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
    var monday = date.AddDays(1 - dayOfWeekIndex).AddDays(index * 7);
    return Enumerable.Range(0, 7).Select(monday.AddDays).ToImmutableArray();
  }
}
