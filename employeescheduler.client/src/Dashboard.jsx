import React, { useEffect, useState } from 'react';
import './App.css';

export default function Dashboard() {
    const user = JSON.parse(localStorage.getItem('user'));
    const userId = user?.id;

    const [nextShift, setNextShift] = useState(null);
    const [isClockedIn, setIsClockedIn] = useState(false);
    const [remainingShifts, setRemainingShifts] = useState(0);
    const [hoursScheduled, setHoursScheduled] = useState(0);
    const [hoursWorked, setHoursWorked] = useState(0);
    const [clockedInEmployees, setClockedInEmployees] = useState([]);

    useEffect(() => {
        if (!userId) return;

        const fetchDashboardData = async () => {
            try {
                const now = new Date();
                const startOfWeek = new Date(now);
                startOfWeek.setDate(now.getDate() - now.getDay());
                startOfWeek.setHours(0, 0, 0, 0);
                const endOfWeek = new Date(startOfWeek);
                endOfWeek.setDate(startOfWeek.getDate() + 6);
                endOfWeek.setHours(23, 59, 59, 999);

                const shiftRes = await fetch(`/api/shiftmanagement/user/${userId}`, {
                    credentials: 'include',
                });
                if (!shiftRes.ok) throw new Error("Failed to load shifts");
                const allShifts = await shiftRes.json();

                const clockRes = await fetch(`/api/clock/user/${userId}`, {
                    credentials: 'include',
                });
                if (!clockRes.ok) throw new Error("Failed to load clock entries");
                const clockEntries = await clockRes.json();

                const clockedShiftIds = clockEntries
                    .filter(e => e.clockInTime != null)
                    .map(e => e.shiftId);

                const statusRes = await fetch(`/api/clock/user/${userId}/active`, {
                    credentials: 'include',
                });
                let isActive = false;
                if (statusRes.ok) {
                    isActive = await statusRes.json();
                    setIsClockedIn(isActive);
                }

                const next = allShifts
                    .filter(shift => {
                        const start = new Date(shift.startTime);
                        const end = new Date(shift.endTime);
                        const isClocked = clockedShiftIds.includes(shift.id);

                        const isUpcoming = start > now;
                        const isActiveNow = start <= now && end >= now;

                        if (isClocked) {
                            return isUpcoming;
                        } else {
                            return isActiveNow || isUpcoming;
                        }
                    })
                    .sort((a, b) => new Date(a.startTime) - new Date(b.startTime))[0];

                setNextShift(next || null);

                const weeklyShifts = allShifts.filter(shift => {
                    const start = new Date(shift.startTime);
                    return start >= startOfWeek && start <= endOfWeek;
                });

                const remainingThisWeek = weeklyShifts.filter(shift => new Date(shift.startTime) > now);
                setRemainingShifts(remainingThisWeek.length);

                const totalScheduledHours = weeklyShifts.reduce((total, shift) => {
                    const start = new Date(shift.startTime);
                    const end = new Date(shift.endTime);
                    return total + (end - start) / (1000 * 60 * 60);
                }, 0);
                setHoursScheduled(totalScheduledHours.toFixed(1));

                const weeklyWorkedHours = clockEntries.reduce((total, entry) => {
                    const clockIn = new Date(entry.clockInTime);
                    const clockOut = entry.clockOutTime ? new Date(entry.clockOutTime) : null;

                    if (clockOut && clockIn >= startOfWeek && clockIn <= endOfWeek) {
                        total += (clockOut - clockIn) / (1000 * 60 * 60);
                    }
                    return total;
                }, 0);
                setHoursWorked(weeklyWorkedHours.toFixed(1));

            } catch (err) {
                console.error("Dashboard load error:", err);
            }
        };

        const fetchClockedInUsers = async () => {
            try {
                const res = await fetch('/api/clock/active-users', {
                    credentials: 'include'
                });
                if (res.ok) {
                    const names = await res.json();
                    setClockedInEmployees(names);
                } else {
                    console.error("Failed to fetch active users");
                }
            } catch (err) {
                console.error("Error loading clocked-in users:", err);
            }
        };

        fetchDashboardData();
        fetchClockedInUsers();
    }, [userId]);

    const handleClockClick = async () => {
        if (!nextShift) return alert("No upcoming shift available");

        const endpoint = isClockedIn
            ? `/api/clock/out/`
            : `/api/clock/in/${nextShift.id}`;

        try {
            const res = await fetch(endpoint, {
                method: 'POST',
                credentials: 'include',
            });
            if (!res.ok) throw new Error("Clock action failed");

            setIsClockedIn(!isClockedIn);
        } catch (err) {
            console.error("Clock error:", err);
        }
    };

    return (
        <div className="dashboard-container">
            <h2>Welcome, {user?.name || 'Employee'}!</h2>

            <div className="dashboard-grid">
                <div className="card highlight">
                    <h3>Next Shift</h3>
                    {nextShift ? (
                        <>
                            <p>{new Date(nextShift.startTime).toLocaleDateString(undefined, { weekday: 'long', month: 'short', day: 'numeric' })}</p>
                            <p>{new Date(nextShift.startTime).toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' })} – {new Date(nextShift.endTime).toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' })}</p>
                        </>
                    ) : (
                        <p>No upcoming shifts</p>
                    )}
                </div>

                <div className="card clock-status-card">
                    <h3>Clock Status</h3>
                    <p className="clock-status-text">
                        You are currently <strong>{isClockedIn ? "Clocked In" : "Clocked Out"}</strong>
                    </p>
                    <button className="clock-btn" onClick={handleClockClick}>
                        {isClockedIn ? "Clock Out" : "Clock In"}
                    </button>
                </div>

                <div className="card">
                    <h3>This Week</h3>
                    <ul className="stats-list">
                        <li>Remaining Shifts: <strong>{remainingShifts}</strong></li>
                        <li>Hours Scheduled: <strong>{hoursScheduled}</strong></li>
                        <li>Hours Worked: <strong>{hoursWorked}</strong></li>
                    </ul>
                </div>

                <div className="card">
                    <h3>Who’s Clocked In</h3>
                    <ul className="stats-list">
                        {clockedInEmployees.length > 0 ? (
                            clockedInEmployees.map((name, index) => (
                                <li key={index}>{name}</li>
                            ))
                        ) : (
                            <li>No one is clocked in.</li>
                        )}
                    </ul>
                </div>
            </div>
        </div>
    );
}
