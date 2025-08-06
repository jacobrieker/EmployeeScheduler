import React, { useEffect, useState } from 'react';
import './App.css';

export default function TimeLogs() {
    const currentUser = JSON.parse(localStorage.getItem('user'));
    const isAdmin = currentUser?.role === 'admin' || currentUser?.role === 'owner';
    const [activeTab, setActiveTab] = useState('mine');

    const [myLogs, setMyLogs] = useState([]);
    const [allLogs, setAllLogs] = useState([]);
    const [userNames, setUserNames] = useState({});

    useEffect(() => {
        if (!currentUser?.id) return;

        fetch(`/api/clock/user/${currentUser.id}`, {
            credentials: 'include'
        })
            .then(res => res.json())
            .then(data => {
                const sorted = [...data].sort((a, b) =>
                    new Date(b.clockOutTime || b.clockInTime) - new Date(a.clockOutTime || a.clockInTime)
                );
                setMyLogs(sorted);
            })
            .catch(err => console.error('Failed to load personal clock entries', err));

        if (isAdmin) {
            fetch(`/api/clock/all`, {
                credentials: 'include'
            })
                .then(res => res.json())
                .then(async (data) => {
                    const sorted = [...data].sort((a, b) =>
                        new Date(b.clockOutTime || b.clockInTime) - new Date(a.clockOutTime || a.clockInTime)
                    );
                    setAllLogs(sorted);

                    const uniqueIds = [...new Set(data.map(log => log.userId))];
                    const nameMap = {};
                    await Promise.all(
                        uniqueIds.map(async (id) => {
                            try {
                                const res = await fetch(`/api/UserAccount/${id}`, {
                                    credentials: 'include'
                                });
                                if (res.ok) {
                                    const user = await res.json();
                                    nameMap[id] = user.name;
                                } else {
                                    nameMap[id] = `User ${id}`;
                                }
                            } catch {
                                nameMap[id] = `User ${id}`;
                            }
                        })
                    );

                    setUserNames(nameMap);
                })
                .catch(err => console.error('Failed to load all clock entries', err));
        }
    }, [currentUser?.id, isAdmin]);

    const formatTime = (time) => {
        if (!time) return '—';
        return new Date(time).toLocaleString(undefined, {
            weekday: 'short',
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
        });
    };

    return (
        <div className="manage-users-container">
            <div className="manage-users-header">
                <h2>Time Logs</h2>
                {isAdmin && (
                    <div style={{ display: 'flex', gap: '12px' }}>
                        <button
                            className={`add-btn ${activeTab === 'mine' ? '' : 'cancel-btn'}`}
                            onClick={() => setActiveTab('mine')}
                        >
                            My Clock Entries
                        </button>
                        <button
                            className={`add-btn ${activeTab === 'all' ? '' : 'cancel-btn'}`}
                            onClick={() => setActiveTab('all')}
                        >
                            All Clock Entries
                        </button>
                    </div>
                )}
            </div>

            <div className="user-table-wrapper">
                <table className="user-table">
                    <thead>
                        <tr>
                            {activeTab === 'all' && <th>Employee</th>}
                            <th>Shift ID</th>
                            <th>Clock In</th>
                            <th>Clock Out</th>
                        </tr>
                    </thead>
                    <tbody>
                        {(isAdmin && activeTab === 'all' ? allLogs : myLogs).map((log) => (
                            <tr key={log.id}>
                                {activeTab === 'all' && <td>{userNames[log.userId] || `User ${log.userId}`}</td>}
                                <td>{log.shiftId}</td>
                                <td>{formatTime(log.clockInTime)}</td>
                                <td>{formatTime(log.clockOutTime)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
