import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function ChangePassword({ user }) {
    const navigate = useNavigate();

    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmNewPassword, setConfirmNewPassword] = useState('');

    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();

        setError('');
        setSuccess('');

        if (!currentPassword || !newPassword || !confirmNewPassword) {
            setError('Please fill in all fields.');
            return;
        }

        if (newPassword !== confirmNewPassword) {
            setError('New passwords do not match.');
            return;
        }

        try {
            if (!user || !user.id) {
                setError('User not logged in.');
                return;
            }

            const response = await fetch(`/api/UserAccount/${user.id}/change-password`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    currentPassword,
                    newPassword,
                    confirmNewPassword,
                }),
            });

            if (response.ok) {
                setSuccess('Password changed successfully!');
                setCurrentPassword('');
                setNewPassword('');
                setConfirmNewPassword('');

                setTimeout(() => {
                    navigate('/profile');
                }, 1500);
            } else {
                const data = await response.json();
                setError(data.message || 'Failed to change password.');
            }
        } catch {
            setError('An unexpected error occurred.');
        }
    };

    return (
        <div
            className="change-password-container"
            style={{
                padding: '40px 20px 20px 20px',
                maxWidth: '480px',
                margin: '60px auto 30px auto',
                background: 'white',
                borderRadius: '12px',
                boxShadow: '0 8px 20px rgba(0, 0, 0, 0.1)',
                fontFamily: "'Segoe UI', Tahoma, Geneva, Verdana, sans-serif",
                color: '#333',
                position: 'relative',
                textAlign: 'center',
            }}
        >
            <button
                onClick={() => navigate('/profile')}
                aria-label="Back to profile"
                style={{
                    position: 'absolute',
                    top: '5px',
                    left: '12px',
                    background: 'none',
                    border: 'none',
                    fontSize: '28px',
                    cursor: 'pointer',
                    color: '#0366d6',
                    padding: 0,
                    lineHeight: 1,
                    width: '28px',
                    height: '28px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                }}
            >
                ←
            </button>

            <h2
                style={{
                    margin: '0 0 40px 0',
                    fontWeight: 700,
                    fontSize: '2rem',
                    color: '#222',
                }}
            >
                Change Password
            </h2>

            <form onSubmit={handleSubmit} style={{ textAlign: 'left' }}>
                <label htmlFor="currentPassword">Current Password</label>
                <input
                    id="currentPassword"
                    type="password"
                    value={currentPassword}
                    onChange={(e) => setCurrentPassword(e.target.value)}
                    className="text-input"
                    autoComplete="current-password"
                />

                <label htmlFor="newPassword" style={{ marginTop: '20px' }}>
                    New Password
                </label>
                <input
                    id="newPassword"
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    className="text-input"
                    autoComplete="new-password"
                />

                <label htmlFor="confirmNewPassword" style={{ marginTop: '20px' }}>
                    Confirm New Password
                </label>
                <input
                    id="confirmNewPassword"
                    type="password"
                    value={confirmNewPassword}
                    onChange={(e) => setConfirmNewPassword(e.target.value)}
                    className="text-input"
                    autoComplete="new-password"
                />

                <button
                    type="submit"
                    className="save-btn"
                    style={{
                        marginTop: '30px',
                        width: '100%',
                        padding: '10px',
                        fontSize: '1.1rem',
                        fontWeight: 'bold',
                        borderRadius: '6px',
                        border: 'none',
                        backgroundColor: '#0366d6',
                        color: 'white',
                        cursor: 'pointer',
                    }}
                >
                    Change Password
                </button>
            </form>

            {error && (
                <div
                    className="message error"
                    style={{ marginTop: '20px', textAlign: 'center', color: '#d73a49' }}
                >
                    {error}
                </div>
            )}
            {success && (
                <div
                    className="message success"
                    style={{ marginTop: '20px', textAlign: 'center', color: '#28a745' }}
                >
                    {success}
                </div>
            )}
        </div>
    );
}
