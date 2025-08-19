import UserDataComponent from './UserDataComponent.jsx';
import UserActiveFilterComponent from './UserActiveFilterComponent.jsx';
import AddUserPopupComponent from './popups/AddUserPopupComponent.jsx';
import EditUserPopupComponent from './popups/EditUserPopupComponent.jsx';
import { useState, useEffect } from 'react';

export default function UsersDisplayComponent() {
    const [displayIsActiveTrue, setDisplayIsActiveTrue] = useState(true);
    const [displayIsActiveFalse, setDisplayIsActiveFalse] = useState(true);
    const [displayAddUserPopup, setDisplayAddUserPopup] = useState(false);
    const [displayEditUserPopup, setDisplayEditUserPopup] = useState(false);
    const [editUserPopupId, setEditUserPopupId] = useState(-1);
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

    function showEditUserPopupWithId(id){
        console.log("just tried to edit the user", {...users.filter(user => user.id === id)[0]});
        setEditUserPopupId(id);
        setDisplayEditUserPopup(prev => !prev);
    }

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
                                    enableEditPopup={showEditUserPopupWithId}
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

            {displayEditUserPopup ? 
                <EditUserPopupComponent 
                    {...users.filter(user => user.id === editUserPopupId)[0]}
                    togglePopup={setDisplayEditUserPopup}
                    setUsers={setUsers}
                /> : null
            }

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