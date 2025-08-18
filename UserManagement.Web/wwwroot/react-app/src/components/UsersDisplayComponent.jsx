import UserDataComponent from './UserDataComponent.jsx';
import UserActiveFilterComponent from './UserActiveFilterComponent.jsx';
import { useState, useEffect } from 'react';

export default function UsersDisplayComponent() {
    const [displayIsActiveTrue, setDisplayIsActiveTrue] = useState(true);
    const [displayIsActiveFalse, setDisplayIsActiveFalse] = useState(true);

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
                                />
                            ))

    return (
        <>
            <div className="user-active-filter-class">
                <UserActiveFilterComponent
                    displayActive={setDisplayIsActiveTrue}
                    displayInactive={setDisplayIsActiveFalse}
                    showActiveUsers={displayIsActiveTrue}
                    showInactiveUsers={displayIsActiveFalse}
                />
            </div>
            <div className="user-data-grid-class">
                <span className="table-header-class">Email</span>
                <span className="table-header-class">Surname</span>
                <span className="table-header-class">Forename</span>
                <span className="table-header-class">Date of Birth</span>
                <span className="table-header-class">isActive</span>
                {userDataDisplay}
            </div>
        </>
    )
}