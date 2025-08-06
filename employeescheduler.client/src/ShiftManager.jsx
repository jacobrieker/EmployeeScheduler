import React, { useState, useEffect } from 'react';
import { Calendar, dateFnsLocalizer } from 'react-big-calendar';
import format from 'date-fns/format';
import parse from 'date-fns/parse';
import startOfWeek from 'date-fns/startOfWeek';
import getDay from 'date-fns/getDay';
import isBefore from 'date-fns/isBefore';
import isAfter from 'date-fns/isAfter';
import enUS from 'date-fns/locale/en-US';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import './App.css';

const locales = { 'en-US': enUS };
const localizer = dateFnsLocalizer({ format, parse, startOfWeek, getDay, locales });

export default function ShiftManager() {
    const today = new Date();
    const [events, setEvents] = useState([]);
    const [users, setUsers] = useState([]);
    const [selectedEmployee, setSelectedEmployee] = useState('');
    const [start, setStart] = useState('');
    const [end, setEnd] = useState('');
    const [error, setError] = useState('');
    const [currentDate, setCurrentDate] = useState(new Date());
    const [popupDate, setPopupDate] = useState(null);

    useEffect(() => {
        fetch('/api/UserAccount', {
            credentials: 'include'
        })
            .then(res => res.json())
            .then(data => setUsers(data.filter(u => u.role !== 'unemployed')))
            .catch(err => console.error('Failed to load users', err));
    }, []);

    useEffect(() => {
        if (users.length === 0) return;

        fetch('/api/ShiftManagement/all', {
            credentials: 'include'
        })
            .then(res => res.json())
            .then(data => {
                const mapped = data.map(s => ({
                    id: s.id,
                    title: '',
                    start: new Date(s.startTime),
                    end: new Date(s.endTime),
                    employee: users.find(u => u.id === s.userId)?.name || `User ${s.userId}`,
                    userId: s.userId,
                }));
                setEvents(mapped);
            });
    }, [users]);



    const handleAddShift = async () => {
        setError('');
        const startDate = new Date(start);
        const endDate = new Date(end);

        if (!selectedEmployee || !start || !end) {
            setError('Please fill in all fields.');
            return;
        }

        if (isBefore(startDate, today.setHours(0, 0, 0, 0))) {
            setError('Start date must be today or later.');
            return;
        }

        if (isAfter(startDate, endDate)) {
            setError('End time cannot be before start time.');
            return;
        }

        const duration = (endDate - startDate) / (1000 * 60 * 60);
        if (duration > 24) {
            setError('Shifts cannot be longer than 24 hours.');
            return;
        }

        const overlap = events.some(existing =>
            existing.employee === selectedEmployee &&
            ((startDate >= new Date(existing.start) && startDate < new Date(existing.end)) ||
                (endDate > new Date(existing.start) && endDate <= new Date(existing.end)) ||
                (startDate <= new Date(existing.start) && endDate >= new Date(existing.end)))
        );

        if (overlap) {
            setError('This employee already has a shift that overlaps with this time.');
            return;
        }

        const user = users.find(u => u.name === selectedEmployee);
        if (!user) {
            setError('Selected employee not found.');
            return;
        }

        const response = await fetch('/api/ShiftManagement', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: 'include',
            body: JSON.stringify({
                userId: user.id,
                startTime: startDate,
                endTime: endDate,
            })
        });

        if (response.ok) {
            const createdShift = await response.json();

            setEvents([...events, {
                id: createdShift.id,
                title: '',
                start: new Date(createdShift.startTime),
                end: new Date(createdShift.endTime),
                employee: selectedEmployee,
                userId: createdShift.userId,
            }]);
            setSelectedEmployee('');
            setStart('');
            setEnd('');
        } else {
            setError('Failed to create shift.');
        }
    };

    const handleNavigate = (newDate) => {
        const thisMonth = new Date();
        thisMonth.setDate(1);
        newDate.setDate(1);

        if (isBefore(newDate, thisMonth)) {
            setCurrentDate(thisMonth);
        } else {
            setCurrentDate(newDate);
        }
    };

    const handleSelectSlot = ({ start }) => {
        setPopupDate(start);
    };

    const closePopup = () => {
        setPopupDate(null);
    };

    const getShiftsForDate = (date) => {
        return events.filter(event =>
            new Date(event.start).toDateString() === date.toDateString()
        ).sort((a, b) => new Date(a.start) - new Date(b.start));
    };

    const deleteShift = async (eventToDelete) => {
        try {
            const res = await fetch(`/api/ShiftManagement/${eventToDelete.id}`, {
                method: 'DELETE',
                credentials: 'include',
            });

            if (!res.ok) {
                throw new Error('Failed to delete shift');
            }

            setEvents(events.filter(e => e.id !== eventToDelete.id));
        } catch (err) {
            console.error(err.message);
            setError('Error deleting shift.');
        }
    };


    return (
        <div style={{ height: '100vh', display: 'flex', flexDirection: 'column', padding: '10px 20px', boxSizing: 'border-box' }}>
            <div style={{ display: 'flex', gap: '10px', alignItems: 'center', marginBottom: '10px', flexWrap: 'wrap' }}>
                <select
                    value={selectedEmployee}
                    onChange={(e) => setSelectedEmployee(e.target.value)}
                    style={{ padding: '6px 10px', fontSize: '0.9rem', borderRadius: '6px', border: '1px solid #ccc', width: '180px' }}
                >
                    <option value="">Select Employee</option>
                    {users.map(user => (
                        <option key={user.id} value={user.name}>{user.name}</option>
                    ))}
                </select>

                <input type="datetime-local" value={start} onChange={(e) => setStart(e.target.value)} style={{ padding: '6px 10px', fontSize: '0.9rem', borderRadius: '6px', border: '1px solid #ccc', width: '200px' }} />
                <input type="datetime-local" value={end} onChange={(e) => setEnd(e.target.value)} style={{ padding: '6px 10px', fontSize: '0.9rem', borderRadius: '6px', border: '1px solid #ccc', width: '200px' }} />

                <button onClick={handleAddShift} style={{ backgroundColor: '#0366d6', color: 'white', padding: '8px 16px', fontWeight: 'bold', border: 'none', borderRadius: '6px', cursor: 'pointer', fontSize: '0.9rem' }}>Add Shift</button>
            </div>

            {error && <div className="error" style={{ marginBottom: '10px' }}>{error}</div>}

            <div style={{ flexGrow: 1 }}>
                <Calendar
                    localizer={localizer}
                    events={[...events].sort((a, b) => new Date(a.start) - new Date(b.start))}
                    startAccessor="start"
                    endAccessor="end"
                    style={{ height: '100%' }}
                    date={currentDate}
                    onNavigate={handleNavigate}
                    min={new Date(today.setHours(0, 0, 0, 0))}
                    views={['month']}
                    toolbar={true}
                    selectable={true}
                    onSelectSlot={handleSelectSlot}
                    components={{
                        event: ({ event }) => (
                            <div style={{ backgroundColor: '#0366d6', borderRadius: '4px', height: '10px', width: '100%', cursor: 'pointer' }} onClick={(e) => { e.stopPropagation(); setPopupDate(new Date(event.start)); }}></div>
                        )
                    }}
                />
            </div>

            {popupDate && (
                <div style={{ position: 'fixed', top: 0, left: 0, right: 0, bottom: 0, backgroundColor: 'rgba(0,0,0,0.4)', display: 'flex', justifyContent: 'center', alignItems: 'center', zIndex: 9999 }} onClick={closePopup}>
                    <div style={{ backgroundColor: 'white', borderRadius: '10px', padding: '20px', minWidth: '300px', maxWidth: '90%', maxHeight: '80%', overflowY: 'auto', boxShadow: '0 4px 12px rgba(0,0,0,0.3)' }} onClick={(e) => e.stopPropagation()}>
                        <h3 style={{ marginTop: 0 }}>Shifts on {format(popupDate, 'PPP')}</h3>
                        {getShiftsForDate(popupDate).length === 0 ? <p>No shifts scheduled.</p> : (
                            <ul style={{ paddingLeft: '20px' }}>
                                {getShiftsForDate(popupDate).map((event, index) => (
                                    <li key={index} style={{ marginBottom: '10px' }}>
                                        <div><strong>{event.employee}</strong>: {format(event.start, 'p')} – {format(event.end, 'p')}</div>
                                        <button onClick={() => deleteShift(event)} style={{ marginTop: '5px', backgroundColor: '#d9534f', color: 'white', border: 'none', padding: '4px 10px', borderRadius: '4px', fontSize: '0.85rem', cursor: 'pointer' }}>Delete Shift</button>
                                    </li>
                                ))}
                            </ul>
                        )}
                        <button onClick={closePopup} style={{ marginTop: '20px', backgroundColor: '#0366d6', color: 'white', border: 'none', padding: '8px 16px', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' }}>Close</button>
                    </div>
                </div>
            )}
        </div>
    );
}