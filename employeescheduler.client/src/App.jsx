import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate, useLocation } from 'react-router-dom';
import Navbar from './Navbar';
import Login from './Login';
import Register from './Register';
import Profile from './Profile';
import ChangePassword from './ChangePassword';
import Dashboard from './Dashboard';
import ManageUsers from './ManageUsers';
import ShiftManager from './ShiftManager';
import MyShifts from './Shifts';
import MyTimeLogs from './TimeLogs';
import UserStats from './UserStats';
import './App.css';

function App() {
    const location = useLocation();

    useEffect(() => {
        localStorage.removeItem('user');
    }, []);

    const [user, setUser] = useState(null);

    const hideNavbar = location.pathname === '/login' || location.pathname === '/register';

    return (
        <div className={`App ${hideNavbar ? 'no-sidebar' : ''}`}>
            {!hideNavbar && <Navbar user={user} setUser={setUser} />}
            <main>
                <Routes>
                    {!user && (
                        <>
                            <Route path="/login" element={<Login setUser={setUser} />} />
                            <Route path="/register" element={<Register setUser={setUser} />} />
                            <Route path="*" element={<Navigate to="/login" replace />} />
                        </>
                    )}

                    {user && (
                        <>
                            <Route path="/" element={<Dashboard />} />
                            <Route path="/myshifts" element={<MyShifts />} />
                            <Route path="/mytimelogs" element={<MyTimeLogs />} />
                            <Route path="/profile" element={<Profile user={user} setUser={setUser} />} />
                            <Route path="/changepassword" element={<ChangePassword user={user} />} />
                            <Route path="/userstats/:userId" element={<UserStats />} />

                            {(user.role === 'admin' || user.role === 'owner') && (
                                <>
                                    <Route path="/manageusers" element={<ManageUsers />} />
                                    <Route path="/shiftmanagement" element={<ShiftManager />} />
                                </>
                            )}

                            <Route path="*" element={<Navigate to="/" replace />} />
                        </>
                    )}
                </Routes>
            </main>
        </div>
    );
}

export default App;
