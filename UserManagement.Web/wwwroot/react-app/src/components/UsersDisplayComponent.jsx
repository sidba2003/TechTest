import UserDataComponent from './UserDataComponent.jsx';
import UserActiveFilterComponent from './UserActiveFilterComponent.jsx';
import AddUserPopupComponent from './AddUserPopupComponent.jsx';
import { useState, useEffect } from 'react';

export default function UsersDisplayComponent() {
    const [displayIsActiveTrue, setDisplayIsActiveTrue] = useState(true);
    const [displayIsActiveFalse, setDisplayIsActiveFalse] = useState(true);
    const [displayAddUserPopup, setDisplayAddUserPopup] = useState(false);
    const [users, setUsers] = useState([]);

    useEffect(
        () => {
            const fetchUsers = async () => {
                try {
                    const res = await fetch("/api/users");
                    if (!res.ok) throw new Error(`HTTP error: ${res.status}`);
                    const data = await res.json();

                    setUsers(data);
                } catch (err) {
                    console.error("Failed to fetch users:", err);
                }
            }

            fetchUsers();
        }, []
    );

    const userDataDisplay = users
                            .filter(data => {
                                return (
                                    (displayIsActiveTrue && data.isActive) || (displayIsActiveFalse &&  !data.isActive)
                                )
                            })
                            .map(data => (
                                <UserDataComponent 
                                    {...data}
                                    setUsers={setUsers}
                                />
                            ))
    
    const popupButtonStyle = {
        backgroundColor: displayAddUserPopup ? "#cf5757ff" : "#e0c466ff"
    }

    return (
        <>
            <div className="user-options-class">
                <UserActiveFilterComponent
                    displayActive={setDisplayIsActiveTrue}
                    displayInactive={setDisplayIsActiveFalse}
                    showActiveUsers={displayIsActiveTrue}
                    showInactiveUsers={displayIsActiveFalse}
                />

                <button 
                    onClick={() => setDisplayAddUserPopup(prev => !prev)} 
                    className="add-user-button-class"
                    style={popupButtonStyle}
                >
                    {!displayAddUserPopup ? "Add User" : "Cancel"}
                </button>
            </div>

            {displayAddUserPopup ? <AddUserPopupComponent setUsers={setUsers} togglePopup={setDisplayAddUserPopup}/> : null}

            <div className="user-data-grid-class">
                <span className="table-header-class">Email</span>
                <span className="table-header-class">Surname</span>
                <span className="table-header-class">Forename</span>
                <span className="table-header-class">Date of Birth</span>
                <span className="table-header-class">isActive</span>
                <span className="table-header-class">Options</span>
                {userDataDisplay}
            </div>
        </>
    )
}