import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import {
    LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer
} from 'recharts';
import {
    startOfWeek, addWeeks, subWeeks,
    startOfMonth, addMonths, subMonths,
    startOfYear, addYears, subYears,
    format
} from 'date-fns';
import './App.css';

const STAT_TYPES = ['hours', 'overtime', 'late'];

export default function UserStats() {
    const { userId } = useParams();
    const [range, setRange] = useState('week');
    const [selectedStat, setSelectedStat] = useState('hours');
    const [data, setData] = useState([]);
    const [userName, setUserName] = useState('');

    const today = new Date();
    const [rangeStart, setRangeStart] = useState(startOfWeek(today));

    const statLabels = {
        hours: 'Hours Worked',
        overtime: 'Overtime Hours',
        late: 'Clocked In Late',
    };

    const formatRangeLabel = () => {
        if (range === 'week') return `Week of ${format(rangeStart, 'MMM d, yyyy')}`;
        if (range === 'month') return format(rangeStart, 'MMMM yyyy');
        if (range === 'year') return format(rangeStart, 'yyyy');
        return '';
    };

    const adjustRange = (direction) => {
        if (range === 'week') {
            setRangeStart(prev => direction === 'prev' ? subWeeks(prev, 1) : addWeeks(prev, 1));
        } else if (range === 'month') {
            setRangeStart(prev => direction === 'prev' ? subMonths(prev, 1) : addMonths(prev, 1));
        } else if (range === 'year') {
            setRangeStart(prev => direction === 'prev' ? subYears(prev, 1) : addYears(prev, 1));
        }
    };

    const handleRangeChange = (newRange) => {
        setRange(newRange);
        if (newRange === 'week') setRangeStart(startOfWeek(today));
        if (newRange === 'month') setRangeStart(startOfMonth(today));
        if (newRange === 'year') setRangeStart(startOfYear(today));
    };

    useEffect(() => {
        const fetchStats = async () => {
            try {
                const startParam = encodeURIComponent(rangeStart.toISOString());
                const [hoursRes, otRes, lateRes] = await Promise.all([
                    fetch(`/api/clock/hours-worked/${userId}?range=${range}&start=${startParam}`, {
                        credentials: 'include'
                    }),
                    fetch(`/api/clock/overtime-hours/${userId}?range=${range}&start=${startParam}`, {
                        credentials: 'include'
                    }),
                    fetch(`/api/clock/clocked-in-late/${userId}?range=${range}&start=${startParam}`, {
                        credentials: 'include'
                    }),
                ]);

                const hoursData = await hoursRes.json();
                const otData = await otRes.json();
                const lateData = await lateRes.json();

                const merged = hoursData.map((entry, i) => ({
                    date: entry.date,
                    hours: entry.value,
                    overtime: otData[i]?.value || 0,
                    late: lateData[i]?.value || 0
                }));

                setData(merged);
            } catch (err) {
                console.error('Failed to fetch stats:', err);
            }
        };

        const fetchUserName = async () => {
            try {
                const res = await fetch(`/api/useraccount/${userId}`, {
                    credentials: 'include'
                });
                if (res.ok) {
                    const user = await res.json();
                    setUserName(user.name || '');
                }
            } catch (err) {
                console.error('Failed to fetch user name:', err);
            }
        };

        fetchUserName();
        fetchStats();
    }, [range, rangeStart, userId]);

    return (
        <div className="manage-users-container" style={{ maxHeight: '100vh', overflowY: 'auto', paddingBottom: 0 }}>
            <div className="manage-users-header" style={{ marginBottom: '10px' }}>
                <h2 style={{ fontSize: '1.6rem', marginBottom: '0' }}>{userName}'s Stats</h2>
                <div style={{ display: 'flex', gap: '6px', flexWrap: 'wrap' }}>
                    <button
                        className={`add-btn ${range === 'week' ? '' : 'cancel-btn'}`}
                        onClick={() => handleRangeChange('week')}
                    >
                        Week
                    </button>
                    <button
                        className={`add-btn ${range === 'month' ? '' : 'cancel-btn'}`}
                        onClick={() => handleRangeChange('month')}
                    >
                        Month
                    </button>
                    <button
                        className={`add-btn ${range === 'year' ? '' : 'cancel-btn'}`}
                        onClick={() => handleRangeChange('year')}
                    >
                        Year
                    </button>
                </div>
            </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', margin: '10px 0' }}>
                <button className="cancel-btn" onClick={() => adjustRange('prev')}>← Prev</button>
                <strong>{formatRangeLabel()}</strong>
                <button className="cancel-btn" onClick={() => adjustRange('next')}>Next →</button>
            </div>

            <div className="dashboard-grid" style={{ marginBottom: '10px', gap: '10px' }}>
                {STAT_TYPES.map(stat => (
                    <div
                        key={stat}
                        className={`card ${selectedStat === stat ? 'highlight' : ''}`}
                        style={{
                            cursor: 'pointer',
                            padding: '10px 12px',
                            minHeight: '60px',
                            justifyContent: 'center'
                        }}
                        onClick={() => setSelectedStat(stat)}
                    >
                        <h3 style={{ margin: 0, fontSize: '0.9rem', fontWeight: 500 }}>{statLabels[stat]}</h3>
                        <p style={{ margin: 0, fontSize: '1.1rem', fontWeight: 'bold' }}>
                            {data.reduce((sum, d) => sum + d[stat], 0)}
                        </p>
                    </div>
                ))}
            </div>

            <div className="card" style={{ padding: '14px' }}>
                <h3 style={{ fontSize: '1.1rem', marginBottom: '12px' }}>
                    {statLabels[selectedStat]} Over {formatRangeLabel()}
                </h3>
                <ResponsiveContainer width="100%" height={300}>
                    <LineChart data={data}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="date" />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Line
                            type="monotone"
                            dataKey={selectedStat}
                            stroke="#0366d6"
                            strokeWidth={3}
                            dot={{ r: 3 }}
                            activeDot={{ r: 5 }}
                        />
                    </LineChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
}
