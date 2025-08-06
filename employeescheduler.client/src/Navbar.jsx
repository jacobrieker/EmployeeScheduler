import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import defaultProfile from './assets/profile_picture.svg';
import dashboardIcon from './assets/dashboard.png';
import shiftsIcon from './assets/shifts.png';
import timeLogsIcon from './assets/timelogs.png';
import profileIcon from './assets/profile.png';
import manageUsersIcon from './assets/users.png';
import shiftManagementIcon from './assets/schedule.png';
import logoutIcon from './assets/logout.png';
import statsIcon from './assets/stats.png';

export default function Navbar({ user, setUser }) {
    const navigate = useNavigate();

    const handleLogout = () => {
        setUser(null);
        navigate('/login');
    };

    const getProfileImageUrl = () => {
        if (user?.profilePictureUrl) {
            return user.profilePictureUrl.startsWith('/')
                ? `https://localhost:7294${user.profilePictureUrl}`
                : `https://localhost:7294/${user.profilePictureUrl}`;
        }
        return defaultProfile;
    };

    const formatRole = (role) => {
        if (!role) return 'Employee';
        return role.charAt(0).toUpperCase() + role.slice(1);
    };

    return (
        <nav className="sidebar">
            {user && (
                <>
                    <div className="profile-section">
                        <img src={getProfileImageUrl()} alt="Profile" className="sidebar-profile-pic" />
                        <div className="sidebar-user-info">
                            <div className="sidebar-name">{user.name || 'User'}</div>
                            <div className="sidebar-role">{formatRole(user.role)}</div>
                        </div>
                    </div>

                    <ul className="sidebar-links">
                        <li>
                            <Link to="/" className="sidebar-link">
                                <img src={dashboardIcon} alt="Dashboard" className="sidebar-icon" />
                                <span>Dashboard</span>
                            </Link>
                        </li>

                        {(user.role === 'admin' || user.role === 'owner') && (
                            <>
                                <li>
                                    <Link to="/shiftmanagement" className="sidebar-link">
                                        <img src={shiftManagementIcon} alt="Shift Manager" className="sidebar-icon" />
                                        <span>Shift Manager</span>
                                    </Link>
                                </li>
                                <li>
                                    <Link to="/manageusers" className="sidebar-link">
                                        <img src={manageUsersIcon} alt="Manage Users" className="sidebar-icon" />
                                        <span>Manage Users</span>
                                    </Link>
                                </li>
                            </>
                        )}

                        <li>
                            <Link to="/myshifts" className="sidebar-link">
                                <img src={shiftsIcon} alt="Shifts" className="sidebar-icon" />
                                <span>Shifts</span>
                            </Link>
                        </li>
                        <li>
                            <Link to="/mytimelogs" className="sidebar-link">
                                <img src={timeLogsIcon} alt="Time Logs" className="sidebar-icon" />
                                <span>Time Logs</span>
                            </Link>
                        </li>

                        <li>
                            <Link to={`/userstats/${user.id}`} className="sidebar-link">
                                <img src={statsIcon} alt="Stats" className="sidebar-icon" />
                                <span>Stats</span>
                            </Link>
                        </li>

                        <li>
                            <Link to="/profile" className="sidebar-link">
                                <img src={profileIcon} alt="Profile" className="sidebar-icon" />
                                <span>Profile</span>
                            </Link>
                        </li>

                        {user.role === 'unemployed' && (
                            <li><span className="nav-message">You are not employed here</span></li>
                        )}

                        <li>
                            <button onClick={handleLogout} className="sidebar-link">
                                <img src={logoutIcon} alt="Logout" className="sidebar-icon" />
                                <span>Logout</span>
                            </button>
                        </li>
                    </ul>
                </>
            )}
        </nav>
    );
}
