import React, { useState, useEffect } from 'react';
import { Calendar, dateFnsLocalizer, Views } from 'react-big-calendar';
import format from 'date-fns/format';
import parse from 'date-fns/parse';
import startOfWeek from 'date-fns/startOfWeek';
import getDay from 'date-fns/getDay';
import enUS from 'date-fns/locale/en-US';
import addWeeks from 'date-fns/addWeeks';
import subWeeks from 'date-fns/subWeeks';
import isBefore from 'date-fns/isBefore';
import isAfter from 'date-fns/isAfter';
import startOfWeekFn from 'date-fns/startOfWeek';
import endOfWeekFn from 'date-fns/endOfWeek';
import differenceInHours from 'date-fns/differenceInHours';

import 'react-big-calendar/lib/css/react-big-calendar.css';
import './App.css';

const locales = { 'en-US': enUS };
const localizer = dateFnsLocalizer({ format, parse, startOfWeek, getDay, locales });

export default function Shifts() {
    const today = new Date();
    const [events, setEvents] = useState([]);
    const [currentDate, setCurrentDate] = useState(today);
    const user = JSON.parse(localStorage.getItem('user'));

    useEffect(() => {
        if (!user?.id) return;

        const fetchShifts = async () => {
            const from = startOfWeekFn(currentDate, { weekStartsOn: 0 });
            const to = endOfWeekFn(currentDate, { weekStartsOn: 0 });

            const url = `/api/ShiftManagement/user/${user.id}/range?from=${from.toISOString()}&to=${to.toISOString()}`;

            try {
                const res = await fetch(url, {
                    credentials: 'include'
                });

                if (!res.ok) throw new Error('Failed to fetch shifts');

                const data = await res.json();

                const formatted = data.map(shift => ({
                    id: shift.id,
                    title: 'Shift',
                    start: new Date(shift.startTime),
                    end: new Date(shift.endTime)
                }));

                setEvents(formatted);
            } catch (err) {
                console.error('Error loading shifts:', err);
            }
        };

        fetchShifts();
    }, [currentDate, user?.id]);

    const goToToday = () => setCurrentDate(today);
    const goToNextWeek = () => setCurrentDate(prev => addWeeks(prev, 1));
    const goToPreviousWeek = () => {
        const previous = subWeeks(currentDate, 1);
        const startOfCurrentWeek = startOfWeekFn(today, { weekStartsOn: 0 });
        if (!isBefore(previous, startOfCurrentWeek)) {
            setCurrentDate(previous);
        }
    };

    const getTotalHoursThisWeek = () => {
        const start = startOfWeekFn(currentDate, { weekStartsOn: 0 });
        const end = endOfWeekFn(currentDate, { weekStartsOn: 0 });

        return events
            .filter(e => e.start >= start && e.start <= end)
            .reduce((total, event) => total + differenceInHours(event.end, event.start), 0);
    };

    const isFutureWeek = isAfter(currentDate, endOfWeekFn(today, { weekStartsOn: 0 }));
    const currentMonthLabel = format(currentDate, 'MMMM yyyy');

    return (
        <div style={{
            padding: '20px 40px 0',
            width: '100%',
            height: '100vh',
            boxSizing: 'border-box',
            display: 'flex',
            flexDirection: 'column',
            overflow: 'hidden'
        }}>
            <div style={{ marginBottom: '10px' }}>
                <h2 style={{ margin: '0 0 5px 0', fontSize: '2rem', color: '#222' }}>My Shifts</h2>
                <div style={{ fontSize: '1.2rem', fontWeight: '600', color: '#444' }}>{currentMonthLabel}</div>
            </div>

            <div style={{
                display: 'flex',
                justifyContent: 'space-between',
                marginBottom: '10px',
                flexWrap: 'wrap',
                gap: '10px'
            }}>
                <div style={{ display: 'flex', gap: '10px' }}>
                    {isFutureWeek && (
                        <button className="clock-btn" onClick={goToPreviousWeek}>⬅ Back Week</button>
                    )}
                    <button className="clock-btn" onClick={goToToday}>Today</button>
                    <button className="clock-btn" onClick={goToNextWeek}>Next Week ➡</button>
                </div>
                <div style={{ fontWeight: '600', fontSize: '1rem', alignSelf: 'center' }}>
                    Total hours this week: {getTotalHoursThisWeek()} hrs
                </div>
            </div>

            <div style={{ flexGrow: 0 }}>
                <Calendar
                    localizer={localizer}
                    events={events}
                    startAccessor="start"
                    endAccessor="end"
                    date={currentDate}
                    onNavigate={newDate => {
                        if (isBefore(newDate, startOfWeekFn(today))) {
                            setCurrentDate(today);
                        } else {
                            setCurrentDate(newDate);
                        }
                    }}
                    views={{ week: true }}
                    defaultView={Views.WEEK}
                    step={60}
                    timeslots={1}
                    style={{ height: '450px' }}
                    toolbar={false}
                />
            </div>
        </div>
    );
}
