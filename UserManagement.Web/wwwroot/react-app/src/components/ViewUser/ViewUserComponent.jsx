import { useSelector } from "react-redux";
import { useState, useEffect } from 'react';
import LogComponent from "../../misc/LogComponent.jsx";

export default function ViewUserComponent() {
    const [userHistory, setUserHistory] = useState([]);

    const userData = useSelector(state => state.user.selectedUser);
    
    if (!userData) return <p>No user selected...go back and select a user!</p>;

    useEffect(() => {
        const fetchUserLogs = async () => {
            const url = `/api/user-audits/${userData.id}`;
            try {
                const response = await fetch(url);
                if (!response.ok) {
                    throw new Error(`Response status: ${response.status}`);
                }

                const data = await response.json()
                setUserHistory(data);
            } catch (error) {
                console.error(error.message);
            } 
        }

        fetchUserLogs()
    }, []);

    const userOperationsList = userHistory.map(item => (
        <LogComponent {...item} />
    ))

    return (
        <div className="user-data-main-class">
            <div className="user-data-header-class">
                User Information
            </div>
            <div className="user-info-class">
                <span>Email: {userData.email}</span>
                <span>Date of Birth: {userData.dateOfBirth.split("T")[0]}</span>
                <span>Forename: {userData.forename}</span>
                <span>Surname: {userData.surname}</span>
                <span>isActive: {userData.isActive.toString()}</span>
            </div>
            
            <div className="operations-header-class">Previous Operations</div>
            <div className="user-operations-class">
                { userOperationsList.length > 0 ? userOperationsList : <span>User with id {userData.id} does not have any associated operations to show!</span> }
            </div>
        </div>
    );
}