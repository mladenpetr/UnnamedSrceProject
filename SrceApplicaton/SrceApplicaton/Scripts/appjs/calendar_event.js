function init() {
    $('#calendar').fullCalendar({
        theme: true,
        slotLabelFormat: 'H(:mm)',
        timeFormat: 'H(:mm)', // time format for 24-hour clock
        prev: 'arrowthick-1-w',
        next: 'arrowthick-2-w',
        header: {
            left: 'prev,next,today',
            center: 'title',
            right: 'agendaWeek,month'
        },
        defaultView: 'agendaWeek', // default view week
        firstDay: 6, // first day in calendar is saturday
        editable: true,
        allDaySlot: false,
        selectable: true,
        events: [
            {
                title: 'event1',
                start: '2017-08-22'
            },
            {
                title: 'event2',
                start: '2017-08-23',
                end: '2017-08-24'
            },
            {
                title: 'event3',
                start: '2017-08-24T12:30:00',
                allDay: false // will make the time show
            }
        ]
    });
}


$(document).ready( //when document is ready, full calendar will be initialize
    init()
);



