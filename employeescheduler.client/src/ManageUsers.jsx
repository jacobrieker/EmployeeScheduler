import React, { useState, useEffect } from 'react';
import './App.css';
import { useNavigate } from 'react-router-dom';
import defaultProfile from './assets/profile_picture.svg';

export default function ManageUsers() {
    const navigate = useNavigate();
    const currentUser = JSON.parse(localStorage.getItem('user'));

    const [users, setUsers] = useState([]);
    const [showUnemployed, setShowUnemployed] = useState(false);

    useEffect(() => {
        fetch('/api/UserAccount')
            .then(res => res.json())
            .then(data => setUsers(data))
            .catch(err => console.error('Failed to load users', err));
    }, []);

    const activeUsers = users.filter(
        u => u.role !== 'unemployed' && u.id !== currentUser.id
    );

    const unemployedUsers = users.filter(
        u => u.role === 'unemployed' && u.id !== currentUser.id
    );

    const updateRole = async (id, newRole) => {
        try {
            const res = await fetch(`/api/UserAccount/${id}/role`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newRole)
            });

            if (res.ok) {
                const updated = await res.json();
                setUsers(prev => prev.map(u => (u.id === id ? updated : u)));
            }
        } catch (err) {
            console.error('Failed to update role', err);
        }
    };

    const handleDeleteUser = (id) => {
        updateRole(id, 'unemployed');
    };

    const handleAddEmployee = (id) => {
        updateRole(id, 'employee');
        setShowUnemployed(false);
    };

    const getProfileImageUrl = (user) => {
        if (user.profilePictureUrl) {
            return user.profilePictureUrl.startsWith('/')
                ? `https://localhost:7294${user.profilePictureUrl}`
                : `https://localhost:7294/${user.profilePictureUrl}`;
        }
        return defaultProfile;
    };

    return (
        <div className="manage-users-container">
            <div className="manage-users-header">
                <h2>Manage Users</h2>
                <button className="add-btn" onClick={() => setShowUnemployed(!showUnemployed)}>
                    {showUnemployed ? 'Close' : 'Add Employee'}
                </button>
            </div>

            {showUnemployed && (
                <div className="unemployed-list">
                    <h3>Unemployed Accounts</h3>
                    <ul>
                        {unemployedUsers.map(user => (
                            <li key={user.id}>
                                <img
                                    src={getProfileImageUrl(user)}
                                    alt="Profile"
                                    className="user-profile-icon"
                                    style={{ width: '24px', height: '24px', borderRadius: '50%', marginRight: '8px', verticalAlign: 'middle' }}
                                />
                                {user.name} ({user.email})
                                <button onClick={() => handleAddEmployee(user.id)}>Add</button>
                            </li>
                        ))}
                    </ul>
                </div>
            )}

            <div className="user-table-wrapper">
                <table className="user-table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Role</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {activeUsers.map(user => (
                            <tr key={user.id}>
                                <td>
                                    <div style={{ display: 'flex', alignItems: 'center' }}>
                                        <img
                                            src={getProfileImageUrl(user)}
                                            alt="Profile"
                                            className="user-profile-icon"
                                            style={{ width: '24px', height: '24px', borderRadius: '50%', marginRight: '8px' }}
                                        />
                                        {user.name}
                                    </div>
                                </td>
                                <td>{user.email}</td>
                                <td>
                                    {currentUser.role === 'owner' ? (
                                        <select
                                            value={user.role}
                                            onChange={(e) => updateRole(user.id, e.target.value)}
                                        >
                                            <option value="employee">Employee</option>
                                            <option value="admin">Admin</option>
                                        </select>
                                    ) : (
                                        user.role.charAt(0).toUpperCase() + user.role.slice(1)
                                    )}
                                </td>
                                <td>
                                    <button onClick={() => navigate(`/userstats/${user.id}`)}>
                                        View Stats
                                    </button>
                                    {(currentUser.role === 'owner' || user.role === 'employee') && (
                                        <button onClick={() => handleDeleteUser(user.id)} className="delete-btn">
                                            Delete User
                                        </button>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
