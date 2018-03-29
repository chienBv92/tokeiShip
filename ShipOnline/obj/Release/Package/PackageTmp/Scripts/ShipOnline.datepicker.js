$(function () {
    $(".datefield").datepicker(getCalendarOption());
    $(".datefield-only-month-year").datepicker(getCalendarOnlyMonthYearOption());

    $(".initialDate").datepicker(getInitialCalendar());
});
