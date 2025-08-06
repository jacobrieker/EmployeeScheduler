import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import defaultProfile from './assets/profile_picture.svg';

export default function Profile({ user, setUser }) {
    const navigate = useNavigate();

    const [preview, setPreview] = useState(null);
    const [selectedFile, setSelectedFile] = useState(null);
    const [editingField, setEditingField] = useState(null);
    const [name, setName] = useState(user?.name || '');
    const [email, setEmail] = useState(user?.email || '');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    useEffect(() => {
        if (selectedFile) {
            setPreview(URL.createObjectURL(selectedFile));
        } else if (user?.profilePictureUrl) {
            const url = user.profilePictureUrl.startsWith('/')
                ? `https://localhost:7294${user.profilePictureUrl}`
                : `https://localhost:7294/${user.profilePictureUrl}`;
            setPreview(url);
        } else {
            setPreview(defaultProfile);
        }
    }, [selectedFile, user?.profilePictureUrl]);

    const startEditing = (field) => {
        setError('');
        setSuccess('');
        setEditingField(field);
    };

    const cancelEditing = () => {
        setError('');
        setSuccess('');
        setEditingField(null);
        setName(user?.name || '');
        setEmail(user?.email || '');
        setSelectedFile(null);
    };

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            setSelectedFile(file);
        }
    };

    const saveProfilePic = async () => {
        if (!selectedFile) {
            setError('Please choose a file first.');
            return;
        }

        try {
            const formData = new FormData();
            formData.append('file', selectedFile);

            const response = await fetch(`/api/UserAccount/${user.id}/uploadProfilePic`, {
                method: 'POST',
                body: formData
            });

            if (!response.ok) throw new Error('Failed to upload image.');

            const updatedUser = await response.json();
            setUser(updatedUser);
            localStorage.setItem('user', JSON.stringify(updatedUser));
            setSelectedFile(null);
            setSuccess('Profile picture updated!');
            setEditingField(null);
        } catch (err) {
            setError(err.message || 'Something went wrong.');
        }
    };

    const saveName = async () => {
        if (!name.trim()) {
            setError('Name cannot be empty');
            return;
        }

        try {
            const response = await fetch(`/api/UserAccount/${user.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, email: null })
            });

            if (!response.ok) throw new Error('Failed to update name');

            const updatedUser = await response.json();
            setUser(updatedUser);
            localStorage.setItem('user', JSON.stringify(updatedUser));
            setSuccess('Name updated successfully!');
            setEditingField(null);
        } catch (err) {
            setError(err.message || 'Something went wrong.');
        }
    };

    const saveEmail = async () => {
        if (!email.trim()) {
            setError('Email cannot be empty');
            return;
        }

        try {
            const response = await fetch(`/api/UserAccount/${user.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name: null, email })
            });

            if (!response.ok) throw new Error('Failed to update email');

            const updatedUser = await response.json();
            setUser(updatedUser);
            localStorage.setItem('user', JSON.stringify(updatedUser));
            setSuccess('Email updated successfully!');
            setEditingField(null);
        } catch (err) {
            setError(err.message || 'Something went wrong.');
        }
    };

    if (!user) return <p>Loading...</p>;

    return (
        <div className="profile-container">
            <h2>Profile</h2>

            <div className="profile-pic-container">
                <img src={preview} alt="Profile" className="profile-pic" />
                {editingField === 'picture' ? (
                    <>
                        <input type="file" accept="image/*" onChange={handleFileChange} className="file-input" />
                        <div className="button-group">
                            <button onClick={saveProfilePic} className="save-btn">Save</button>
                            <button onClick={cancelEditing} className="cancel-btn">Cancel</button>
                        </div>
                    </>
                ) : (
                    <button className="change-pic-button" onClick={() => startEditing('picture')}>
                        Change Picture
                    </button>
                )}
            </div>

            <div className="profile-item">
                <div className="profile-label">Name:</div>
                {editingField === 'name' ? (
                    <>
                        <input value={name} onChange={(e) => setName(e.target.value)} className="text-input" />
                        <div className="button-group">
                            <button onClick={saveName} className="save-btn">Save</button>
                            <button onClick={cancelEditing} className="cancel-btn">Cancel</button>
                        </div>
                    </>
                ) : (
                    <>
                        <div className="profile-value">{user.name}</div>
                        <div className="profile-actions">
                            <button onClick={() => startEditing('name')} className="change-btn">Change</button>
                        </div>
                    </>
                )}
            </div>

            <div className="profile-item">
                <div className="profile-label">Email:</div>
                {editingField === 'email' ? (
                    <>
                        <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} className="text-input" />
                        <div className="button-group">
                            <button onClick={saveEmail} className="save-btn">Save</button>
                            <button onClick={cancelEditing} className="cancel-btn">Cancel</button>
                        </div>
                    </>
                ) : (
                    <>
                        <div className="profile-value">{user.email}</div>
                        <div className="profile-actions">
                            <button onClick={() => startEditing('email')} className="change-btn">Change</button>
                        </div>
                    </>
                )}
            </div>

            <div className="profile-item">
                <div className="profile-label">Password:</div>
                <div className="profile-value password-dots">••••••••</div>
                <div className="profile-actions">
                    <button onClick={() => navigate('/changepassword')} className="change-btn">Change</button>
                </div>
            </div>

            {error && <div className="message error">{error}</div>}
            {success && <div className="message success">{success}</div>}
        </div>
    );
}
